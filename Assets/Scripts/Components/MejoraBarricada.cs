using System.Collections;
using UnityEngine;

public class MejoraBarricada : MonoBehaviour
{
    [Header("Configuracion General")]
    [SerializeField] private string identificador = "mejoras:barricada_portatil";
    [SerializeField][Tooltip("El tiempo de enfriamiento en segundos")] private float enfriamiento = 8.0f;
    [SerializeField][Tooltip("El tiempo que le toma a la barricada en autodestruirse")] private float vidaUtil = 14.0f;
    [SerializeField] private float distanciaColocacion = 4.0f;
    [SerializeField] private GameObject barricada;
    [SerializeField] private bool noDesaparece = false;

    private bool enfriandose;

    void Start()
    {
        if (!noDesaparece)
        {
            Destroy(this.gameObject, vidaUtil);
        }
    }

    public string ObtenerIdentificador()
    {
        return identificador;
    }

    private void Activar()
    {
        Vector3 posicion_activacion = this.gameObject.transform.position + gameObject.transform.forward * distanciaColocacion;
        Quaternion rotacion_activacion = this.gameObject.transform.rotation;
        ColocarBarricada(posicion_activacion, rotacion_activacion);
    }

    private void ColocarBarricada(Vector3 posicion, Quaternion rotacion)
    {
        Instantiate(barricada);
        StartCoroutine("EjecutarEnfriamiento");
    }

    IEnumerator EjecutarEnfriamiento()
    {
        enfriandose = true;
        yield return new WaitForSeconds(enfriamiento);
        enfriandose = false;
    }
}
