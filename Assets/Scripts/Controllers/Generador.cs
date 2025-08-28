using UnityEngine;

public class Generador : MonoBehaviour
{
    [Header("Configuracion General")]
    [SerializeField] private Transform posicionGeneracion;

    public Transform ObtenerTransformacion()
    {
        return posicionGeneracion;
    }
}
