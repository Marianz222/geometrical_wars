using UnityEngine;

using GeometricalWars.Enumeraciones;

[RequireComponent(typeof(VinculoParental))]

public class Proyectil : MonoBehaviour
{
    [Header("Configuracion General")]
    [SerializeField] protected int daño;
    [SerializeField][Range(1.0f, 200.0f)] protected float framesAplicados;
    [SerializeField] protected TipoDaño[] tipoDaño;
    [SerializeField] protected int duracion;
    [SerializeField] protected float velocidad;
    [SerializeField] protected bool tieneGravedad;

    protected Arma arma_usada;
    protected GameObject dueño;

    public void AsignarDueño(GameObject nuevo_dueño)
    {
        dueño = nuevo_dueño;
    }

    public void AsignarArmaUsada(Arma arma)
    {
        arma_usada = arma;
    }

    public virtual void FijarDireccion(Vector3 direccion_salida)
    {
        //Sobreescribir en herederos
    }

    public void FijarVelocidad(float nueva_velocidad)
    {
        velocidad = nueva_velocidad;
    }

    public void DefinirMultiplicadorVelocidad(float factor)
    {
        velocidad *= factor;
    }

    public float ObtenerVelocidad()
    {
        return velocidad;
    }
}
