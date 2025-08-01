using UnityEngine;
using UnityEngine.Events;
using System.Collections;

using GeometricalWars.Enumeraciones;
using GeometricalWars.Interfaces;
using GeometricalWars.Eventos;

public class ControladorJugador : MonoBehaviour, IDañable
{

    [Header("Estadísticas Base")]
    [SerializeField] private int vidaMaximaBase = 100;
    [SerializeField] private int blindajeMaximoBase = 50;
    [SerializeField] private float velocidadMovimientoBase = 6.0f;
    [SerializeField] private float fuerzaSalto = 14.0f;
    [SerializeField] private float fuerzaImpulso = 26.0f;
    [SerializeField] private float enfriamientoImpulso = 3.0f;
    [SerializeField] private float enfriamientoSalto = 1.0f;
    [SerializeField] private LayerMask mascaraSalto;

    [Header("Multiplicadores Base")]
    [SerializeField][Range(0.0f, 1.0f)] private float factorVida = 1.0f;
    [SerializeField][Range(0.0f, 1.0f)] private float factorInvulnerabilidad = 1.0f;
    [SerializeField][Range(0.0f, 1.0f)] private float factorDaño = 1.0f;
    [SerializeField][Range(0.0f, 1.0f)] private float factorVelocidadMovimiento = 1.0f;
    [SerializeField][Range(0.0f, 1.0f)] private float factorCadenciaDisparo = 1.0f;

    [Header("Armamento y Mejoras")]
    [SerializeField] private GameObject armaInicial; //Placeholder
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
    [SerializeField] private ControladorCamaraDinamica controladorCamara;
    [SerializeField] private Transform[] marcadoresArma;

    [Header("Asignacion de Teclas")]
    [SerializeField] private KeyCode teclaImpulso = KeyCode.LeftAlt;
    [SerializeField] private KeyCode teclaSalto = KeyCode.Space;
    [SerializeField] private KeyCode teclaDisparo = KeyCode.Mouse0;

    [Header("Eventos Disponibles")]
    [SerializeField] private UnityEvent alInicializar;
    [SerializeField] private EventoModificarVida alModificarVida;
    [SerializeField] private EventoModificarBlindaje alModificarBlindaje;
    [SerializeField] private EventoRecibirDaño alRecibirDaño;
    [SerializeField] private UnityEvent alMorir;

    //Variables locales
    private int vida;
    private int blindaje;
    private int combustible;
    private float velocidad_movimiento;
    private float enfriamiento_impulso;
    private float enfriamiento_salto;
    private bool esta_muerto = false;
    private bool saltando = false;
    private bool tocando_suelo = true;
    private int frames_invulnerabilidad = 0;
    private float longitud_raycast_salto = 0.48f;

    //Referencias locales a componentes
    private Rigidbody cuerpo_rigido;
    private Collider colisionador_cuerpo;
    private GameObject arma_instanciada;
    private ControladorArma arma;
    private ComponenteSistemaApuntado sistema_apuntado;

    void Start()
    {
        alInicializar.Invoke();
        vida = (int)(vidaMaximaBase * factorVida);
        blindaje = blindajeMaximoBase;
        velocidad_movimiento = velocidadMovimientoBase * factorVelocidadMovimiento;
        arma_instanciada = Instantiate(armaInicial, marcadoresArma[0]);
        arma = arma_instanciada.gameObject.GetComponent<ControladorArma>();
        cuerpo_rigido = gameObject.GetComponent<Rigidbody>();
        colisionador_cuerpo = GetComponentInChildren<Collider>();
        sistema_apuntado = GetComponentInChildren<ComponenteSistemaApuntado>();
    }
    void Update()
    {

        ActualizarContadores();
        DispararArmaActual();

        if (Input.GetKeyDown(teclaSalto))
        {

            Saltar();

        }

        if (Input.GetKeyDown(teclaImpulso))
        {

            Impulsar(fuerzaImpulso, this.gameObject.transform.forward, false);

        }

    }
    void FixedUpdate()
    {
        Mover();
        ActualizarEstadoSalto();
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
        if (!puedeSaltar || !puedeMoverse || esta_muerto || !tocando_suelo)
        {
            return;
        }

        Vector3 fuerza = Vector3.up * fuerzaSalto;
        ForceMode modo = ForceMode.Impulse;

        cuerpo_rigido.AddForce(fuerza, modo);
        enfriamiento_salto = enfriamientoSalto * Time.smoothDeltaTime;
        //Disparar evento: Jugador saltó (Con mejora de salto activa)

    }

    private void Impulsar(float fuerza, Vector3 direccion, bool causa_daño)
    {
        if (enfriamiento_impulso > 0) { return; }
        if (!puedeImpulsarse || !puedeMoverse || esta_muerto)
        {
            return;
        }

        ForceMode modo = ForceMode.Impulse;

        cuerpo_rigido.velocity = Vector3.zero;
        cuerpo_rigido.AddForce(fuerza * direccion, modo);
        enfriamiento_impulso = enfriamientoImpulso * Time.smoothDeltaTime;
        StartCoroutine("InterrumpirMovimiento", 0.3f);
        //Disparar evento: Jugador se impulsó (Usó dash)

    }
    private void DispararArmaActual()
    {
        if (esta_muerto) { return; }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 origen = arma.ObtenerPosicionFuente().position;
            Vector3 objetivo = sistema_apuntado.DefinirCoordenadasDisparoJugador();
            Vector3 direccion = objetivo - origen;
            direccion.Normalize();
            arma.disparar(direccion);
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
    }

    private void ActualizarFramesInvulnerabilidad()
    {
        if (frames_invulnerabilidad == 0 || esta_muerto)
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

    public void RecibirDaño(int cantidad, bool directo, TipoDaño tipo, GameObject atacante, float frames)
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

        float frames_calculados = Mathf.Round(frames * factorInvulnerabilidad);
        frames_invulnerabilidad = (int)frames_calculados;

        if (vida <= 0)
        {
            vida = 0;
            esta_muerto = true;
            alMorir.Invoke();
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
                vida += cantidad;
                int nueva_vida = vida + cantidad;
                alModificarVida.Invoke(nueva_vida);
                break;
            case RecursoEntidad.BLINDAJE:
                blindaje += cantidad;
                int nuevo_blindaje = blindaje + cantidad;
                alModificarBlindaje.Invoke(nuevo_blindaje);
                break;
            case RecursoEntidad.COMBUSTIBLE:
                //Placeholder
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
        }
        return tecla_obtenida;
    }

    void OnDrawGizmos()
    {
        Vector3 origen = transform.position;
        Vector3 destino = transform.position + (longitud_raycast_salto * Vector3.down);
        Gizmos.color = new Color(0.0f, 0.8f, 0.8f, 1.0f);
        Gizmos.DrawLine(origen, destino);
    }

    IEnumerator InterrumpirMovimiento(float tiempo)
    {
        puedeMoverse = false;
        colisionador_cuerpo.material.staticFriction = 0.0f;
        colisionador_cuerpo.material.dynamicFriction = 0.0f;
        colisionador_cuerpo.material.frictionCombine = PhysicMaterialCombine.Minimum;
        yield return new WaitForSeconds(tiempo);
        puedeMoverse = true;
        colisionador_cuerpo.material.staticFriction = 1.0f;
        colisionador_cuerpo.material.dynamicFriction = 1.0f;
        colisionador_cuerpo.material.frictionCombine = PhysicMaterialCombine.Maximum;
    }
}