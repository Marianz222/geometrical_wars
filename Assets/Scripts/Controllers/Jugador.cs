using UnityEngine;
using UnityEngine.Events;

using System.Collections;
using System.Collections.Generic;

using GeometricalWars.Enumeraciones;
using GeometricalWars.Interfaces;
using GeometricalWars.Eventos;

public class Jugador : MonoBehaviour, IDañable
{

    [Header("Estadísticas Base")]
    [SerializeField] private int vidaMaximaBase = 100;
    [SerializeField] private int blindajeMaximoBase = 50;
    [SerializeField] private int combustibleMaximoBase = 100;
    [SerializeField] private float velocidadMovimientoBase = 5.0f;
    [SerializeField] private float fuerzaSalto = 14.0f;
    [SerializeField] private float fuerzaImpulso = 26.0f;
    [SerializeField] private float enfriamientoImpulso = 8.0f;
    [SerializeField] private float enfriamientoSalto = 2.0f;
    [SerializeField] private LayerMask mascaraSalto;

    [Header("Consumo Combustible")]
    [SerializeField] private int consumicionSalto = 3;
    [SerializeField] private int consumicionImpulso = 10;

    [Header("Multiplicadores Base")]
    [SerializeField][Range(0.0f, 1.0f)] private float factorVida = 1.0f;
    [SerializeField][Range(0.0f, 1.0f)] private float factorCombustible = 1.0f;
    [SerializeField][Range(0.0f, 1.0f)] private float factorInvulnerabilidad = 1.0f;
    //[SerializeField][Range(0.0f, 1.0f)] private float factorDaño = 1.0f;
    [SerializeField][Range(0.0f, 1.0f)] private float factorVelocidadMovimiento = 1.0f;
    //[SerializeField][Range(0.0f, 1.0f)] private float factorCadenciaDisparo = 1.0f;

    [Header("Armamento y Mejoras")]
    [SerializeField] private GameObject armaInicial; //Placeholder
    [SerializeField] private GameObject armaSecundaria;
    [SerializeField] private GameObject[] mejorasIniciales; //Placeholder

    [Header("Condicionales")]
    [SerializeField] private bool puedeMoverse = true;
    [SerializeField] private bool puedeSaltar = true;
    [SerializeField] private bool puedeImpulsarse = true;
    [SerializeField] private bool puedeUsarHabilidadActiva = true;
    [SerializeField] private bool consumeCombustible = true;

    [Header("Referencias a Componentes")]
    [SerializeField] private GameObject cuerpo;
    [SerializeField] private Camera camara;
    [SerializeField] private CamaraDinamica controladorCamara;
    [SerializeField] private Transform[] marcadoresArma;

    [Header("Asignacion de Teclas")]
    [SerializeField] private KeyCode teclaImpulso = KeyCode.LeftAlt;
    [SerializeField] private KeyCode teclaSalto = KeyCode.Space;
    [SerializeField] private KeyCode teclaDisparo = KeyCode.Mouse0;
    [SerializeField] private KeyCode teclaMejora = KeyCode.LeftShift;

    [Header("Eventos Disponibles")]
    [SerializeField] private UnityEvent alInicializar;
    [SerializeField] private EventoModificarVida alModificarVida;
    [SerializeField] private EventoModificarBlindaje alModificarBlindaje;
    [SerializeField] private EventoModificarCombustible alModificarCombustible;
    [SerializeField] private EventoRecibirDaño alRecibirDaño;
    [SerializeField] private UnityEvent alMorir;

    //Variables locales
    private int vida;
    private int vida_maxima;
    private int blindaje;
    private int blindaje_maximo;
    private int combustible;
    private int combustible_maximo;
    private float velocidad_movimiento;
    private float enfriamiento_impulso;
    private float enfriamiento_salto;
    private bool dañado_recientemente = false;
    private bool esta_muerto = false;
    private bool tocando_suelo = true;
    private int frames_invulnerabilidad = 0;
    private int frames_ultimo_ataque;
    private float longitud_raycast_salto = 0.48f;
    private Color color_base_malla;
    private Dictionary<string, bool> mejoras;

    //Referencias locales a componentes
    private Rigidbody cuerpo_rigido;
    private Collider colisionador_cuerpo;
    private GameObject arma_instanciada;
    private Arma arma_principal;
    private Arma arma_secundaria;
    private SistemaApuntado sistema_apuntado;
    private TrailRenderer rastro_dash;
    private MeshRenderer malla_cuerpo;

    void Start()
    {
        alInicializar.Invoke();
        sistema_apuntado = GetComponentInChildren<SistemaApuntado>();
        InicializarRecursos();
        InsertarEquipamientoInicial();
        arma_principal = arma_instanciada.gameObject.GetComponent<Arma>();
        cuerpo_rigido = gameObject.GetComponent<Rigidbody>();
        colisionador_cuerpo = GetComponentInChildren<Collider>();
        rastro_dash = GetComponent<TrailRenderer>();
        rastro_dash.emitting = false;
        malla_cuerpo = cuerpo.gameObject.GetComponent<MeshRenderer>();
        color_base_malla = malla_cuerpo.material.color;
    }
    
    void Update()
    {

        ActualizarContadores();
        DispararArmaActual();
        ActualizarOverlayInmunidad();

        if (Input.GetKeyDown(teclaSalto))
        {

            Saltar();

        }

        if (Input.GetKeyDown(teclaImpulso))
        {

            Impulsar(fuerzaImpulso, this.gameObject.transform.forward, false);

        }

        if (Input.GetKeyDown(teclaMejora))
        {
            ActivarHabilidadMejora();
        }

    }
    void FixedUpdate()
    {
        Mover();
        ActualizarEstadoSalto();
    }

    private void InicializarRecursos()
    {
        vida_maxima = (int)(vidaMaximaBase * factorVida);
        vida = vida_maxima;

        blindaje_maximo = blindajeMaximoBase;
        blindaje = blindaje_maximo;

        combustible_maximo = (int)(combustibleMaximoBase * factorCombustible);
        combustible = combustible_maximo;

        velocidad_movimiento = velocidadMovimientoBase * factorVelocidadMovimiento;
    }

    private void ActualizarOverlayInmunidad()
    {
        if (frames_ultimo_ataque == 0) { return; }

        if (frames_invulnerabilidad > 0)
        {
            float tiempo_transicion = 1.0f - ((float)frames_invulnerabilidad / (float)frames_ultimo_ataque);

            Color color_capa = Color.red;
            Color nuevo_color = Color.Lerp(color_base_malla, color_capa, tiempo_transicion);

            malla_cuerpo.material.color = nuevo_color;
        }
        else
        {
            malla_cuerpo.material.color = color_base_malla;
        }
    }

    public void AñadirMejora(string id)
    {
        if (mejoras.ContainsKey(id)) { return; }
        mejoras.Add(id, true);
    }

    private void InsertarEquipamientoInicial()
    {
        arma_instanciada = Instantiate(armaInicial, marcadoresArma[0]);
        Arma arma = arma_instanciada.GetComponent<Arma>();
        sistema_apuntado.alDefinirObjetivo.AddListener(arma.ReceptorDefinirObjetivo);

        VinculoParental vinculo = arma_instanciada.GetComponent<VinculoParental>();
        vinculo.AsignarPadre(gameObject);

        foreach (GameObject iterador in mejorasIniciales)
        {
            //Placeholder
        }
    }

    private void ActualizarEstadoSalto()
    {
        Vector3 origen = transform.position;
        Vector3 destino = transform.position + (longitud_raycast_salto * Vector3.down);
        Vector3 direccion = (destino - origen).normalized;
        Ray rayo = new Ray(origen, direccion);
        if (Physics.Raycast(rayo, out RaycastHit golpe, longitud_raycast_salto, mascaraSalto))
        {
            tocando_suelo = true;
        }
        else
        {
            tocando_suelo = false;
        }
    }

    private void Mover()
    {

        if (!puedeMoverse || esta_muerto)
        {
            return;
        }

        float movimiento_horizontal = Input.GetAxis("Horizontal");
        float movimiento_vertical = Input.GetAxis("Vertical");
        Vector3 direccion = new Vector3(movimiento_horizontal, 0, movimiento_vertical);

        float bonus_velocidad_camara = controladorCamara.ObtenerBonusVelocidadMovimiento();
        Vector3 movimiento = transform.TransformDirection(direccion) * (velocidad_movimiento * bonus_velocidad_camara) * Time.deltaTime;
        cuerpo_rigido.MovePosition(cuerpo_rigido.position + movimiento);

    }

    private void Saltar()
    {
        if (enfriamiento_salto > 0) { return; }
        if (!puedeSaltar || !puedeMoverse || esta_muerto || !tocando_suelo) { return; }
        if (consumeCombustible && combustible < consumicionSalto) { return; }

        Vector3 fuerza = Vector3.up * fuerzaSalto;
        ForceMode modo = ForceMode.Impulse;

        cuerpo_rigido.AddForce(fuerza, modo);
        enfriamiento_salto = enfriamientoSalto * Time.smoothDeltaTime;
        //Disparar evento: Jugador saltó

        ConsumirCombustible((int)(consumicionSalto * factorCombustible));
    }

    private void Impulsar(float fuerza, Vector3 direccion, bool causa_daño)
    {
        if (enfriamiento_impulso > 0) { return; }
        if (!puedeImpulsarse || !puedeMoverse || esta_muerto) { return; }
        if (consumeCombustible && combustible < consumicionImpulso) { return; }

        ForceMode modo = ForceMode.Impulse;

        cuerpo_rigido.velocity = Vector3.zero;
        cuerpo_rigido.AddForce(fuerza * direccion, modo);
        enfriamiento_impulso = enfriamientoImpulso * Time.smoothDeltaTime;
        StartCoroutine("ProcesarImpulso", 0.3f);
        //Disparar evento: Jugador se impulsó (Usó dash)

        ConsumirCombustible((int)(consumicionImpulso * factorCombustible));

    }

    private void ActivarHabilidadMejora()
    {
        if (!puedeUsarHabilidadActiva || esta_muerto) { return; }
        //Placeholder
    }
    private void DispararArmaActual()
    {
        if (esta_muerto) { return; }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (arma_principal != null)
            {
                Vector3 origen = arma_principal.ObtenerPosicionFuente().position;
                Vector3 objetivo = sistema_apuntado.DefinirCoordenadasDisparoJugador();
                arma_principal.disparar(origen, objetivo);
            }

            if (arma_secundaria != null)
            {
                Vector3 origen = arma_secundaria.ObtenerPosicionFuente().position;
                Vector3 objetivo = sistema_apuntado.DefinirCoordenadasDisparoJugador();
                arma_secundaria.disparar(origen, objetivo);
            }
        }
    }

    public bool EstaVivo()
    {
        return !esta_muerto;
    }

    private void ActualizarContadores()
    {
        ActualizarFramesInvulnerabilidad();
        ActualizarEnfriamientos();
        ActualizarDañadoRecientemente();
    }

    private void ActualizarFramesInvulnerabilidad()
    {
        if (frames_invulnerabilidad == 0)
        {
            return;
        }

        frames_invulnerabilidad--;
    }

    private void ActualizarEnfriamientos()
    {
        if (enfriamiento_impulso > 0)
        {
            enfriamiento_impulso--;
        }

        if (enfriamiento_salto > 0)
        {
            enfriamiento_salto--;
        }
    }

    private void ActualizarDañadoRecientemente()
    {
        //Placeholder
    }

    public void RecibirDaño(int cantidad, bool directo, TipoDaño[] tipo, GameObject atacante, float frames)
    {
        if (frames_invulnerabilidad > 0 || esta_muerto)
        {
            return;
        }

        if (blindaje > 0)
        {
            blindaje -= cantidad;
            float nuevo_porcentaje_blindaje = (blindaje - cantidad) / 100.0f;
            alModificarBlindaje.Invoke(nuevo_porcentaje_blindaje);
        }
        else
        {
            vida -= cantidad;
            float nuevo_porcentaje_vida = (vida - cantidad) / 100.0f;
            alModificarVida.Invoke(nuevo_porcentaje_vida);
            alRecibirDaño.Invoke(cantidad, directo, tipo, atacante);
        }

        AplicarInvulnerabilidad(frames);

        if (vida <= 0)
        {
            vida = 0;
            esta_muerto = true;
            alMorir.Invoke();
        }
    }

    public void ConsumirCombustible(int cantidad)
    {
        if (consumeCombustible)
        {
            combustible -= cantidad;
            float porcentaje_consumido = combustible / 100.0f;
            alModificarCombustible.Invoke(combustible);
        }
    }

    public int ObtenerRecurso(RecursoEntidad tipo)
    {
        switch (tipo)
        {
            case RecursoEntidad.VIDA:
                return vida;
            case RecursoEntidad.BLINDAJE:
                return blindaje;
            case RecursoEntidad.COMBUSTIBLE:
                return combustible;
        }

        return 0;
    }

    public void RecuperarRecurso(int cantidad, RecursoEntidad tipo)
    {
        switch (tipo)
        {
            case RecursoEntidad.VIDA:
                if (RecursoLleno(RecursoEntidad.VIDA)) { break; }
                vida += cantidad;
                int nueva_vida = vida + cantidad;
                alModificarVida.Invoke(nueva_vida);
                break;
            case RecursoEntidad.BLINDAJE:
                if (RecursoLleno(RecursoEntidad.BLINDAJE)) { break; }
                blindaje += cantidad;
                int nuevo_blindaje = blindaje + cantidad;
                alModificarBlindaje.Invoke(nuevo_blindaje);
                break;
            case RecursoEntidad.COMBUSTIBLE:
                if (RecursoLleno(RecursoEntidad.COMBUSTIBLE)) { break; }
                combustible += cantidad;
                int nuevo_combustible = combustible + cantidad;
                alModificarCombustible.Invoke(nuevo_combustible);
                break;
        }
    }

    public KeyCode ObtenerAsignacionTecla(TeclasJugador tecla)
    {
        KeyCode tecla_obtenida = KeyCode.None;

        switch (tecla)
        {
            case TeclasJugador.DISPARO:
                tecla_obtenida = teclaDisparo;
                break;
            case TeclasJugador.IMPULSO:
                tecla_obtenida = teclaImpulso;
                break;
            case TeclasJugador.HABILIDAD_MEJORA:
                tecla_obtenida = teclaMejora;
                break;
        }
        return tecla_obtenida;
    }

    private bool RecursoLleno(RecursoEntidad recurso)
    {
        switch (recurso)
        {
            case RecursoEntidad.VIDA:
                if (vida == vida_maxima)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case RecursoEntidad.BLINDAJE:
                if (blindaje == blindaje_maximo)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case RecursoEntidad.COMBUSTIBLE:
                if (combustible == combustible_maximo)
                {
                    return true;
                }
                else
                {
                    return false;
                }
        }
        return false;
    }

    private void AplicarInvulnerabilidad(float frames)
    {
        float frames_calculados = Mathf.Round(frames * factorInvulnerabilidad);
        frames_invulnerabilidad = (int)frames_calculados;
        frames_ultimo_ataque = (int)frames_calculados;

    }

    public void ReceptorEnemigoEliminado(GameObject atacante, TipoDaño[] fuente_daño, int ultimo_daño, int poder)
    {
        print("Enemigo fue eliminado");
    }

    void OnDrawGizmos()
    {
        Vector3 origen = transform.position;
        Vector3 destino = transform.position + (longitud_raycast_salto * Vector3.down);
        Gizmos.color = new Color(0.0f, 0.8f, 0.8f, 1.0f);
        Gizmos.DrawLine(origen, destino);
    }

    IEnumerator ProcesarImpulso(float tiempo)
    {
        puedeMoverse = false;
        cuerpo_rigido.mass = 0.8f;
        colisionador_cuerpo.material.staticFriction = 0.0f;
        colisionador_cuerpo.material.dynamicFriction = 0.0f;
        colisionador_cuerpo.material.frictionCombine = PhysicMaterialCombine.Minimum;
        rastro_dash.emitting = true;
        yield return new WaitForSeconds(tiempo);
        puedeMoverse = true;
        cuerpo_rigido.mass = 2.1f;
        colisionador_cuerpo.material.staticFriction = 1.0f;
        colisionador_cuerpo.material.dynamicFriction = 1.0f;
        colisionador_cuerpo.material.frictionCombine = PhysicMaterialCombine.Maximum;
        yield return new WaitForSeconds(tiempo);
        rastro_dash.emitting = false;
    }
}