using System.Collections;
using UnityEngine;

public class ManejadorNivel : MonoBehaviour
{
    [Header("Configuracion General")]
    [SerializeField] private int poderOleadaBase = 4;
    [SerializeField] private float tiempoPreOleada = 3.0f;
    [SerializeField] private float tiempoIntervaloSpawn = 1.0f;
    [SerializeField] private GameObject[] generadoresDisponibles;

    //[Header("Configuracion Escalar")]
    //[SerializeField] private int falopa;

    [Header("Enemigos Disponibles")]
    [SerializeField] private GameObject[] enemigosTier1;
    [SerializeField] private GameObject[] enemigosTier2;
    [SerializeField] private GameObject[] enemigosTier3;
    [SerializeField] private GameObject[] enemigosPrototipo;

    private int poder_oleada = 0;
    private int maximo_poder_oleada;
    private int oleada = 0;
    private int enemigos_oleada;

    void Start()
    {
        maximo_poder_oleada = poderOleadaBase;
        StartCoroutine("IniciarGeneracion");
    }

    IEnumerator IniciarGeneracion()
    {
        while (poder_oleada < maximo_poder_oleada)
        {
            GameObject enemigo_elegido = enemigosTier1[Random.Range(0, enemigosTier1.Length)];
            GameObject generador_elegido = generadoresDisponibles[Random.Range(0, generadoresDisponibles.Length)];
            GenerarEnemigo(enemigo_elegido, generador_elegido);
            yield return new WaitForSeconds(tiempoIntervaloSpawn);
        }
    }

    private void ReceptorEnemigoEliminado()
    {
        enemigos_oleada--;
        Debug.Log("[DEBUG/INFO]: Un enemigo fue eliminado");
    }

    public GameObject GenerarEnemigo(GameObject enemigo, GameObject generador)
    {
        if (enemigo.GetComponent<ControladorEnemigo>().ObtenerPoderOleada() + poder_oleada > maximo_poder_oleada)
        {
            return null;
        }
        if (enemigo == null || generador == null)
        {
            return null;
        }
        GameObject enemigo_generado = Instantiate(enemigo, generador.transform);
        enemigo_generado.transform.position = generador.GetComponent<ControladorGeneradorEnemigo>().ObtenerTransformacion().position;
        poder_oleada += enemigo_generado.GetComponent<ControladorEnemigo>().ObtenerPoderOleada();
        enemigos_oleada++;
        enemigo_generado.GetComponent<ControladorEnemigo>().alMorir.AddListener(ReceptorEnemigoEliminado);
        return enemigo_generado;
    }
}
