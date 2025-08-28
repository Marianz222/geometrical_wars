using UnityEngine;

using GeometricalWars.Enumeraciones;

[RequireComponent(typeof(VinculoParental))]

public class Arma : MonoBehaviour
{
    [Header("Plantilla")]
    [SerializeField] protected PlantillaArma estadisticasArma;

    [Header("Configuracion General")]
    [SerializeField] protected float factorVelocidadProyectil = 0.2f;

    [Header("Referencias a Componentes")]
    [SerializeField] protected Transform posicionDisparo;

    protected int daño;
    protected bool disparando;
    protected VinculoParental vinculo;
    protected GameObject proyectil;
    protected FisicasDisparo tipo_fisicas; //Implementar más adelante

    void Start()
    {
        proyectil = estadisticasArma.Proyectil;
        daño = estadisticasArma.Daño;
        tipo_fisicas = estadisticasArma.FisicasDisparo;
        vinculo = GetComponent<VinculoParental>();
    }

    public void ReceptorDefinirObjetivo(Vector3 posicion_objetivo)
    {
        //Placeholder
    }

    public float ObtenerFactorVelocidad()
    {
        return factorVelocidadProyectil;
    }

    public void DispararContinuamente(bool estado)
    {
        disparando = estado;
    }

    public void AsignarFactorVelocidad(float nuevo_factor)
    {
        factorVelocidadProyectil = nuevo_factor;
    }

    public Transform ObtenerPosicionFuente()
    {
        return posicionDisparo;
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

    public FisicasDisparo ObtenerFuncionamiento()
    {
        return tipo_fisicas;
    }

    public void AsignarDaño(int nuevo_daño)
    {
        daño = nuevo_daño;
    }

    public virtual void disparar(Vector3 origen, Vector3 destino)
    {
        Vector3 direccion = destino - origen;
        direccion.Normalize();
        GameObject nuevo_proyectil = Instantiate(proyectil, posicionDisparo.position, Quaternion.identity);
        Proyectil bala = nuevo_proyectil.GetComponent<Proyectil>();
        bala.AsignarArmaUsada(this);
        bala.DefinirMultiplicadorVelocidad(factorVelocidadProyectil);
        bala.FijarDireccion(direccion);

        VinculoParental vinculo = nuevo_proyectil.GetComponent<VinculoParental>();
        vinculo.AsignarPadre(this.vinculo.ObtenerPadre());

    }

    public void SobreescribirProyectil(GameObject nuevo_proyectil)
    {
        proyectil = nuevo_proyectil;
    }
}