using UnityEngine;

public class GeneradorJugador : MonoBehaviour
{

    [Header("Configuracion General")]
    [SerializeField] private Transform posicionGeneracion;
    [SerializeField] private GameObject jugador;

    void Start()
    {
        GameObject nueva_entidad = Instantiate(jugador, posicionGeneracion);
    }
    
}
