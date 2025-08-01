using UnityEngine;

using GeometricalWars.Enumeraciones;

public class ControladorPickup : MonoBehaviour
{
    [SerializeField] private RecursoEntidad tipo = RecursoEntidad.VIDA;
    [SerializeField] private bool esPorcentual = false;
    [SerializeField] private float valor = 25.0f;

    void OnCollisionEnter(Collision contacto)
    {
        Debug.Log("Jugador lo agarró");
    }
}
