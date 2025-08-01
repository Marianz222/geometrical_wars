using UnityEngine;

public class ControladorArma : MonoBehaviour
{
    [Header("Plantilla")]
    [SerializeField] private PlantillaArma estadisticasArma;

    [Header("Configuracion General")]
    [SerializeField] private float factorVelocidadProyectil = 0.2f;

    [Header("Referencias a Componentes")]
    [SerializeField] private Transform posicionFuenteDisparo;

    private int daño;
    private GameObject proyectil;

    void Awake()
    {
        proyectil = estadisticasArma.Proyectil;
        daño = estadisticasArma.Daño;
    }

    public float ObtenerFactorVelocidad()
    {
        return factorVelocidadProyectil;
    }

    public void AsignarFactorVelocidad(float nuevo_factor)
    {
        factorVelocidadProyectil = nuevo_factor;
    }

    public Transform ObtenerPosicionFuente()
    {
        return posicionFuenteDisparo;
    }

    public float ObtenerCadenciaTiro()
    {
        return estadisticasArma.Cadencia;
    }

    public Vector2 ObtenerDispersion()
    {
        return estadisticasArma.Dispersion;
    }

    public int ObtenerDaño()
    {
        return daño;
    }

    public void AsignarDaño(int nuevo_daño)
    {
        daño = nuevo_daño;
    }

    public void disparar(Vector3 direccion_disparo)
    {
        GameObject nuevo_proyectil = Instantiate(proyectil);
        nuevo_proyectil.transform.position = posicionFuenteDisparo.position;
        Proyectil controlador_bala = nuevo_proyectil.GetComponent<Proyectil>();
        controlador_bala.AsignarArmaUsada(this);
        controlador_bala.DefinirMultiplicadorVelocidad(factorVelocidadProyectil);
        controlador_bala.FijarDireccion(direccion_disparo);
        controlador_bala.AsignarDueño(this.gameObject);
    }

    public void SobreescribirProyectil(GameObject nuevo_proyectil)
    {
        proyectil = nuevo_proyectil;
    }
}