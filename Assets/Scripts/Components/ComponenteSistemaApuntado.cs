using UnityEngine;
using UnityEngine.UI;

using GeometricalWars.Utilidad;

public class ComponenteSistemaApuntado : MonoBehaviour
{
    [Header("Configuracion General")]
    [SerializeField] private float distanciaMaxima = 100.0f;
    [SerializeField] private LayerMask mascaraFiltro;

    [Header("Referencias a Componentes")]
    [SerializeField] private ControladorCamaraDinamica controladorCamara;
    [SerializeField] private Image miraSimple;

    private ControladorJugador controlador_jugador;
    private Camera camara;
    private ControladorArma arma_actual;
    private Vector2 dispersion_arma;
    private float bonus_dispersion_camara;

    void Start()
    {
        controlador_jugador = gameObject.GetComponentInParent<ControladorJugador>();
        camara = FindObjectOfType<Camera>();
        if (!camara.CompareTag("MainCamera") || camara == null)
        {
            camara = null;
            Debug.Log("[DEBUG/ERROR] Camara obtenida no es la cámara principal");
        }
        arma_actual = FindObjectOfType<ControladorArma>();
        dispersion_arma = arma_actual.ObtenerDispersion();
    }

    public Vector3 DefinirCoordenadasDisparo(Vector3 objetivo)
    {
        Vector2 dispersion = new Vector2(
            Random.Range(-dispersion_arma.x, dispersion_arma.x),
            Random.Range(-dispersion_arma.y, dispersion_arma.y)
        );

        Vector3 origen = arma_actual.ObtenerPosicionFuente().position;
        Vector3 direccion = (objetivo - origen).normalized;

        Ray rayo = new Ray(origen, direccion);

        if (Physics.Raycast(rayo, out RaycastHit golpe_rayo, distanciaMaxima, mascaraFiltro))
        {
            return golpe_rayo.point;
        }
        return rayo.origin + rayo.direction * distanciaMaxima;
    }

    public Vector3 DefinirCoordenadasDisparoJugador()
    {

        Vector3 centro_pantalla = new Vector3(Screen.width / 2, Screen.height / 2, 0.0f);
        Vector2 dispersion_inicial = new Vector2(Random.Range(-dispersion_arma.x, dispersion_arma.x), Random.Range(-dispersion_arma.y, dispersion_arma.y));

        bonus_dispersion_camara = controladorCamara.ObtenerBonusPrecision();
        Vector2 dispersion_final = Utilidades.LimitarVector(dispersion_inicial * bonus_dispersion_camara, 0, 200);

        Ray rayo = camara.ScreenPointToRay(centro_pantalla + new Vector3(dispersion_final.x, dispersion_final.y, 0.0f));

        if (Physics.Raycast(rayo, out RaycastHit golpe_rayo, distanciaMaxima, mascaraFiltro))
        {
            return golpe_rayo.point;
        }
        return rayo.origin + rayo.direction * distanciaMaxima;
    }
}
