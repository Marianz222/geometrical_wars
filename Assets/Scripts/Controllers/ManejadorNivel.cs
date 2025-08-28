using System.Collections;
using UnityEngine;

using GeometricalWars.Eventos;
using GeometricalWars.Enumeraciones;

public class ManejadorNivel : MonoBehaviour
{

    [Header("Configuracion General")]
    [SerializeField] private int poderOleadaBase = 4;
    [SerializeField] private float tiempoPreOleada = 3.0f;
    [SerializeField] private float tiempoIntervaloSpawn = 1.0f;
    [SerializeField] private int conteoPickups = 4;
    [SerializeField] private AnimationCurve incrementoPickups = new AnimationCurve();
    [SerializeField] private AnimationCurve incrementoPoderMaximo = new AnimationCurve();
    [SerializeField] private GameObject[] generadoresEnemigosDisponibles;
    [SerializeField] private GameObject[] generadoresPickupDisponibles;

    [Header("Configuracion Especial")]
    [Tooltip("Cada cuantas oleadas aparecerá un enemigo mejorado, tras eliminar a la oleada completa")]
    [SerializeField] private int intervaloMejorado = 3;

    [Tooltip("Cada cuantas oleadas aparecerá un enemigo prototipo, tras eliminar a la oleada completa")]
    [SerializeField] private int intervaloPrototipo = 10;

    //[Header("Configuracion Escalar")]
    //[SerializeField] private int falopa;

    [Header("Enemigos Disponibles")]
    [SerializeField] private GameObject[] enemigosTier1;
    [SerializeField] private GameObject[] enemigosTier2;
    [SerializeField] private GameObject[] enemigosTier3;
    [SerializeField] private GameObject[] enemigosMejorados;
    [SerializeField] private GameObject[] enemigosPrototipo;

    [Header("Pickups")]
    [SerializeField] private GameObject pickupPrefab;
    [SerializeField] private RecursoEntidad[] recursosDisponibles;

    [Header("Eventos Disponibles")]
    [SerializeField] private EventoIniciarOleada alIniciarOleada;
    [SerializeField] private EventoFinalizarOleada alFinalizarOleada;

    [Header("Depuracion")]
    [SerializeField] private bool depurar = false;

    private int poder_oleada = 0;
    private int maximo_poder_oleada;
    private int oleada = 0;
    private bool es_oleada_especial = false;
    private int enemigos_oleada;
    private bool generacion_completada = false;
    private bool oleada_en_curso = false;
    private GameObject[] pickups;

    void Start()
    {
        maximo_poder_oleada = poderOleadaBase;
        StartCoroutine("IniciarOleada");
    }

    void Update()
    {
        RevisionOleadaSuperada();
    }

    private void RevisionOleadaSuperada()
    {
        if (generacion_completada && enemigos_oleada == 0)
        {
            alFinalizarOleada.Invoke(oleada);
            if (depurar && oleada_en_curso) { Debug.Log("Oleada " + oleada + " superada. Iniciando oleada: " + (oleada + 1)); }
            oleada_en_curso = false;
            maximo_poder_oleada += 3;

            StartCoroutine("IniciarOleada");
        }
    }

    private void GenerarPickups()
    {
        pickups = new GameObject[conteoPickups];

        for (int i = 0; i < conteoPickups; i++)
        {
            if (generadoresPickupDisponibles.Length == 0) { return; }
            GameObject generador_elegido = generadoresPickupDisponibles[Random.Range(0, generadoresPickupDisponibles.Length)];
            
            if (recursosDisponibles.Length == 0) { return; }
            RecursoEntidad recurso_elegido = recursosDisponibles[Random.Range(0, recursosDisponibles.Length)];
            pickups[i] = Instantiate(pickupPrefab, generador_elegido.GetComponent<Generador>().ObtenerTransformacion());
            pickups[i].GetComponent<PickupRecurso>().ModificarRecurso(recurso_elegido);
            if (depurar) { Debug.Log("Instanciado pickup: " + pickups[i].name + "| Tipo: " + pickups[i].GetComponent<PickupRecurso>().ObtenerRecurso()); }
        }
    }

    IEnumerator IniciarOleada()
    {
        oleada_en_curso = true;
        ReiniciarSistema();
        GenerarPickups();
        oleada++;
        alIniciarOleada.Invoke(oleada);

        if (depurar) { Debug.Log("Oleada actual: " + oleada); }

        yield return new WaitForSeconds(tiempoPreOleada);
        
        if (depurar) { Debug.Log("Completado intervalo inicial"); }

        while (poder_oleada < maximo_poder_oleada)
        {
            GameObject enemigo_elegido = enemigosTier1[Random.Range(0, enemigosTier1.Length)];
            GameObject generador_elegido = generadoresEnemigosDisponibles[Random.Range(0, generadoresEnemigosDisponibles.Length)];
            GenerarEnemigo(enemigo_elegido, generador_elegido);
            yield return new WaitForSeconds(tiempoIntervaloSpawn);
        }
        generacion_completada = true;
        if (depurar) { Debug.Log("Se completó la generación de enemigos para la oleada: " + oleada); }
    }

    public GameObject GenerarEnemigo(GameObject enemigo, GameObject generador)
    {
        if (enemigo.GetComponent<Enemigo>().ObtenerPoderOleada() + poder_oleada > maximo_poder_oleada)
        {
            return null;
        }
        if (enemigo == null || generador == null)
        {
            return null;
        }
        GameObject enemigo_generado = Instantiate(enemigo, generador.transform);
        enemigo_generado.transform.position = generador.GetComponent<Generador>().ObtenerTransformacion().position;
        poder_oleada += enemigo_generado.GetComponent<Enemigo>().ObtenerPoderOleada();
        enemigos_oleada++;
        enemigo_generado.GetComponent<Enemigo>().alSerEliminado.AddListener(ReceptorEnemigoEliminado);
        if (depurar) { Debug.Log("Enemigos oleada: " + enemigos_oleada + "| Poder oleada: " + poder_oleada + "| Enemigo generado: " + enemigo_generado.gameObject.name); }
        return enemigo_generado;
    }

    private void ReiniciarSistema()
    {
        if (depurar) { Debug.Log("Reiniciando el Sistema"); }
        enemigos_oleada = 0;
        poder_oleada = 0;
        generacion_completada = false;
        if (depurar) { Debug.Log("Enemigos oleada: " + enemigos_oleada + "| Poder oleada: " + poder_oleada + "| Generacion completada: " + generacion_completada); }
    }

    private void ReceptorEnemigoEliminado(GameObject atacante, TipoDaño[] fuente_daño, int daño_ultimo_golpe, int poder_oleada)
    {
        //Quitar la lógica a continuación y llamarla antes de pasar el parametro de atacante a este metodo
        VinculoParental vinculo = atacante.GetComponent<VinculoParental>();
        if (vinculo == null) { return; }
        Jugador jugador = vinculo.ObtenerPadre().GetComponent<Jugador>();

        if (jugador != null)
        {
            jugador.RecuperarRecurso((10 * poder_oleada),RecursoEntidad.COMBUSTIBLE);
        }
        
        enemigos_oleada--;
        this.poder_oleada -= poder_oleada;
        if (depurar) { Debug.Log("Enemigos oleada: " + enemigos_oleada + "| Poder oleada: " + poder_oleada); }
        if (depurar) { Debug.Log("[DEBUG/INFO]: Un enemigo fue eliminado. Causante: " + atacante.name); }
    }
}
