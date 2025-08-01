using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Blueprint", menuName = "Custom/Enemy Blueprint")]

public class PlantillaEnemigo : ScriptableObject
{
    [Header("Estadísticas")]
    [SerializeField] private int vida;
    [SerializeField] private float velocidadMovimiento;
    [SerializeField] private float velocidadRotacion;
    [SerializeField][Range(1, 10)] private int poderOleada;

    [Header("Multiplicadores")]
    [SerializeField][Range(0.0f, 1.0f)] private float factorDaño = 1.0f;
    [SerializeField][Range(0.0f, 1.0f)] private float factorCadenciaDisparo = 1.0f;

    [Header("Equipamiento")]
    [SerializeField] private GameObject[] armasDisponibles;

    public int Vida { get => vida; set => vida = value; }
    public float VelocidadMovimiento { get => velocidadMovimiento; set => velocidadMovimiento = value; }
    public float VelocidadRotacion { get => velocidadRotacion; set => velocidadRotacion = value; }
    public float MultiplicadorDaño { get => factorDaño; set => MultiplicadorDaño = value; }
    public float MultiplicadorCadencia { get => factorCadenciaDisparo; set => factorCadenciaDisparo = value; }
    public GameObject[] Armas { get => armasDisponibles; set => armasDisponibles = value; }
    public int PoderOleada { get => poderOleada; set => poderOleada = value; }
}