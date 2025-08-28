using UnityEngine;
using UnityEngine.AI;
using System.Collections;

using GeometricalWars.Enumeraciones;
using GeometricalWars.Interfaces;
using GeometricalWars.Eventos;

public class Enemigo : MonoBehaviour, IDañable
{

    [Header("Plantilla")]
    [SerializeField] private PlantillaEnemigo plantilla;

    [Header("Configuracion General")]
    [SerializeField] private bool mejorado = false;
    [SerializeField] private GameObject proyectil;
    [SerializeField] private int disparosContinuos = 6;
    [SerializeField] private float tiempoRecarga = 1.65f;

    [Header("Referencias a Componentes")]
    [SerializeField] private GameObject cuerpo;
    [SerializeField] private Transform[] marcadoresArma;
    [SerializeField] private SistemaApuntado sistemaApuntado;

    [Header("Eventos Disponibles")]
    [SerializeField] public EventoEnemigoEliminado alSerEliminado;

    private GameObject arma_equipada;
    private GameObject objetivo_actual = null;
    private NavMeshAgent agente_navegacion;
    private MeshRenderer malla;
    private Color color_base_malla;

    private int vida;
    private float multiplicador_daño;
    private float cadencia_disparo;
    private int dispersion_ataque;
    private float velocidad_movimiento;
    private float velocidad_rotacion;
    private int frames_invulnerabilidad = 0;
    private int frames_ultimo_ataque;
    private float cadenciaAtaque;
    private int contador_disparos = 0;
    private bool esta_recargando = false;

    void Awake()
    {
        objetivo_actual = FindObjectOfType<Jugador>().gameObject;
    }

    void Start()
    {
        AsignarArma();
        agente_navegacion = GetComponent<NavMeshAgent>();
        malla = cuerpo.gameObject.GetComponent<MeshRenderer>();
        color_base_malla = malla.material.color;
        PrepararEstadisticas();
    }

    void Update()
    {
        ActualizarFramesInvulnerabilidad();
        ActualizarEnfriamientoArma();
        Disparar();
        ActualizarOverlayInmunidad();
        BuscarObjetivo();
    }

    private void PrepararEstadisticas()
    {
        vida = plantilla.Vida;
        cadencia_disparo = arma_equipada.GetComponent<Arma>().ObtenerCadenciaTiro();
        multiplicador_daño = plantilla.MultiplicadorDaño;
        velocidad_movimiento = plantilla.VelocidadMovimiento;
        velocidad_rotacion = plantilla.VelocidadRotacion;

        if (mejorado)
        {
            vida = (int)(vida * 1.3f);
            cadencia_disparo = cadencia_disparo * 1.2f;
            multiplicador_daño = multiplicador_daño * 1.2f;
            velocidad_movimiento = velocidad_movimiento * 1.2f;
        }
    }

    public void AsignarArma()
    {
        GameObject[] opciones = plantilla.Armas;
        int indice_seleccionado = Random.Range(0, opciones.Length);
        if (opciones[indice_seleccionado] == null)
        {
            return;
        }
        arma_equipada = Instantiate(opciones[indice_seleccionado], marcadoresArma[0]);
        Arma arma = arma_equipada.GetComponent<Arma>();
        VinculoParental vinculo = arma_equipada.GetComponent<VinculoParental>();
        vinculo.AsignarPadre(gameObject);
        arma.AsignarDaño((int)(arma.ObtenerDaño() * multiplicador_daño));
        if (proyectil != null)
        {
            arma.SobreescribirProyectil(proyectil);
        }
    }

    public void RecibirDaño(int cantidad, bool directo, TipoDaño[] tipo, GameObject atacante, float frames)
    {
        if (frames_invulnerabilidad > 0)
        {
            return;
        }

        vida -= cantidad;
        frames_invulnerabilidad = (int)frames;
        frames_ultimo_ataque = (int)frames;

        //GestorJuego.Instancia.GenerarDañoFlotante(cantidad.ToString(), Color.red, transform.position);

        if (vida <= 0)
        {
            vida = 0;
            //Jugador jugador = atacante.gameObject.GetComponent<Jugador>();
            //if (jugador != null) { alSerEliminado.AddListener(jugador.ReceptorEnemigoEliminado); }
            alSerEliminado.Invoke(atacante, tipo, cantidad, plantilla.PoderOleada);
            Destroy(this.gameObject);
        }
        else
        {
            //Disparar evento: Enemigo recibió daño
        }
    }

    private bool BuscarObjetivo()
    {
        if (objetivo_actual == null) { return false; }
        if (!objetivo_actual.GetComponent<Jugador>().EstaVivo()) { agente_navegacion.ResetPath(); return false; }
        return agente_navegacion.SetDestination(objetivo_actual.transform.position);
    }

    private void ActualizarFramesInvulnerabilidad()
    {
        if (frames_invulnerabilidad == 0)
        {
            return;
        }

        frames_invulnerabilidad--;
    }

    private void ActualizarEnfriamientoArma()
    {
        if (cadenciaAtaque > 0)
        {
            cadenciaAtaque--;
        }
    }

    private void ActualizarOverlayInmunidad()
    {
        if (frames_ultimo_ataque == 0) { return; }

        if (frames_invulnerabilidad > 0)
        {
            float tiempo_transicion = 1.0f - ((float)frames_invulnerabilidad / (float)frames_ultimo_ataque);

            Color color_capa = Color.white;
            Color nuevo_color = Color.Lerp(color_base_malla, color_capa, tiempo_transicion);

            malla.material.color = nuevo_color;
        }
        else
        {
            malla.material.color = color_base_malla;
        }
    }

    public int ObtenerPoderOleada()
    {
        return plantilla.PoderOleada;
    }

    private void Disparar()
    {
        if (objetivo_actual == null) { return; }
        if (!objetivo_actual.GetComponent<Jugador>().EstaVivo()) { return; }
        if (cadenciaAtaque > 0) { return; }
        if (contador_disparos >= disparosContinuos && !esta_recargando) { StartCoroutine("RecargarArma"); return; }
        if (esta_recargando) { return; }
        if (!TieneContactoVisual(objetivo_actual)) { return; }

        Arma arma = arma_equipada.GetComponent<Arma>();

        Vector3 origen = arma.ObtenerPosicionFuente().position;
        Vector3 objetivo = sistemaApuntado.DefinirCoordenadasDisparo(objetivo_actual.transform.position);
        arma.disparar(origen, objetivo);
        cadenciaAtaque = (int)(cadencia_disparo * 60 * Random.Range(0.9f, 1.1f));
        contador_disparos++;
    }

    public bool TieneContactoVisual(GameObject objeto)
    {
        Vector3 offset_altura = new Vector3(0.0f, 0.2f, 0.0f);

        Vector3 origen = transform.position + offset_altura + (transform.forward * 0.2f);
        Vector3 destino = objeto.transform.position + offset_altura;
        Vector3 direccion = (destino - origen).normalized;
        Ray rayo = new Ray(origen, direccion);

        if (Physics.Raycast(rayo, out RaycastHit golpe_rayo, 200))
        {
            return (golpe_rayo.collider.gameObject == objeto);
        }
        else
        {
            return false;
        }
    }

    IEnumerator RecargarArma()
    {
        esta_recargando = true;
        yield return new WaitForSeconds(tiempoRecarga);
        esta_recargando = false;
        contador_disparos = 0;
    }

    void OnDrawGizmos()
    {
        if (objetivo_actual != null)
        {
            if (TieneContactoVisual(objetivo_actual)) { Gizmos.color = new Color(0.0f, 0.6f, 1.0f, 1.0f); }
            else { Gizmos.color = new Color(1.0f, 0.6f, 0.0f, 1.0f); }
            Vector3 offset = new Vector3(0.0f, 0.2f, 0.0f);
            Gizmos.DrawLine(this.transform.position + offset, objetivo_actual.transform.position + offset + (transform.forward * 0.2f));
        }
    }
}
