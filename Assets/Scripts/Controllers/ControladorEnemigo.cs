using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

using GeometricalWars.Enumeraciones;
using GeometricalWars.Interfaces;
using System.Collections;

public class ControladorEnemigo : MonoBehaviour, IDañable
{

    [Header("Plantilla")]
    [SerializeField] private PlantillaEnemigo plantilla;

    [Header("Configuracion General")]
    [SerializeField] private GameObject proyectil;
    [SerializeField] private int disparosContinuos = 6;
    [SerializeField] private float tiempoRecarga = 1.65f;

    [Header("Referencias a Componentes")]
    [SerializeField] private Transform[] marcadoresArma;
    [SerializeField] private ComponenteSistemaApuntado sistemaApuntado;

    [Header("Eventos Disponibles")]
    [SerializeField] public UnityEvent alMorir;

    private GameObject arma_equipada;
    private GameObject objetivo_actual = null;
    private NavMeshAgent agente_navegacion;

    private int vida;
    private int dispersion_ataque;
    private float velocidad_movimiento;
    private float velocidad_rotacion;
    private int frames_invulnerabilidad = 0;
    private float cadenciaAtaque;
    private int contador_disparos = 0;
    private bool esta_recargando = false;

    void Awake()
    {
        objetivo_actual = FindObjectOfType<ControladorJugador>().gameObject;
    }

    void Start()
    {
        vida = plantilla.Vida;
        velocidad_movimiento = plantilla.VelocidadMovimiento;
        velocidad_rotacion = plantilla.VelocidadRotacion;
        AsignarArma();
        agente_navegacion = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        ActualizarFramesInvulnerabilidad();
        ActualizarEnfriamientoArma();
        Disparar();
        BuscarObjetivo();
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
        ControladorArma arma = arma_equipada.GetComponent<ControladorArma>();
        arma.AsignarDaño((int)(arma.ObtenerDaño() * plantilla.MultiplicadorDaño));
        if (proyectil != null)
        {
            arma.SobreescribirProyectil(proyectil);
        }
    }

    public void RecibirDaño(int cantidad, bool directo, TipoDaño tipo, GameObject atacante, float frames)
    {
        if (frames_invulnerabilidad > 0)
        {
            return;
        }

        vida -= cantidad;
        frames_invulnerabilidad = (int)frames;

        if (vida <= 0)
        {
            vida = 0;
            alMorir.Invoke();
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
        if (!objetivo_actual.GetComponent<ControladorJugador>().EstaVivo()) { agente_navegacion.ResetPath(); return false; }
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

    public int ObtenerPoderOleada()
    {
        return plantilla.PoderOleada;
    }

    private void Disparar()
    {
        if (objetivo_actual == null) { return; }
        if (!objetivo_actual.GetComponent<ControladorJugador>().EstaVivo()) { return; }
        if (cadenciaAtaque > 0) { return; }
        if (contador_disparos >= disparosContinuos && !esta_recargando) { StartCoroutine("RecargarArma"); return; }
        if (esta_recargando) { return; }
        if (!TieneContactoVisual(objetivo_actual)) { return; }

        ControladorArma arma = arma_equipada.GetComponent<ControladorArma>();
        Vector3 origen = arma.ObtenerPosicionFuente().position;
        Vector3 objetivo = sistemaApuntado.DefinirCoordenadasDisparo(objetivo_actual.transform.position);
        Vector3 direccion = objetivo - origen;
        direccion.Normalize();
        arma.disparar(direccion);
        cadenciaAtaque = (int)(arma_equipada.GetComponent<ControladorArma>().ObtenerCadenciaTiro() * 60 * Random.Range(0.9f, 1.1f));
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
            return (golpe_rayo.collider.gameObject.transform.parent.gameObject == objeto);
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
