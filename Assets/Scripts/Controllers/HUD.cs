using UnityEngine;
using UnityEngine.UI;

using GeometricalWars.Enumeraciones;

public class HUD : MonoBehaviour
{
    [Header("Referencias a Componentes")]
    [SerializeField] private GameObject jugador;
    [SerializeField] private Slider indicadorVida;
    [SerializeField] private Slider indicadorBlindaje;
    [SerializeField] private Slider indicadorCombustible;

    private Jugador controlador_jugador;

    void Start()
    {
        controlador_jugador = jugador.GetComponent<Jugador>();
        InicializarIndicadores();
    }

    private void InicializarIndicadores()
    {
        float porcentaje_vida = controlador_jugador.ObtenerRecurso(RecursoEntidad.VIDA) / 100.0f;
        float porcentaje_blindaje = controlador_jugador.ObtenerRecurso(RecursoEntidad.BLINDAJE) / 50.0f;
        float porcentaje_combustible = controlador_jugador.ObtenerRecurso(RecursoEntidad.COMBUSTIBLE) / 100.0f;
        ActualizarIndicadorVida(porcentaje_vida);
        ActualizarIndicadorBlindaje(porcentaje_blindaje);
        ActualizarIndicadorCombustible(porcentaje_combustible);
    }

    public void ActualizarIndicadorVida(float porcentaje)
    {
        indicadorVida.value = porcentaje;
    }

    public void ActualizarIndicadorBlindaje(float porcentaje)
    {
        indicadorBlindaje.value = porcentaje;
    }

    public void ActualizarIndicadorCombustible(float porcentaje)
    {
        indicadorCombustible.value = porcentaje;
    }

}
