using UnityEngine;
using System.Collections;

using GeometricalWars.Enumeraciones;
using GeometricalWars.Interfaces;

public class Proyectil : MonoBehaviour
{
    [Header("Configuracion General")]
    [SerializeField] protected int daño;
    [SerializeField][Range(1.0f, 200.0f)] protected float framesAplicados;
    [SerializeField] protected TipoDaño tipoDaño;
    [SerializeField] protected TipoProyectil tipoProyectil;
    [SerializeField] protected int duracion;
    [SerializeField] protected float velocidad;
    [SerializeField] protected bool tieneGravedad;

    protected ControladorArma arma_usada;
    protected GameObject dueño;
    protected TrailRenderer rastro;

    void Awake()
    {
        rastro = GetComponent<TrailRenderer>();
        rastro.enabled = false;
    }

    void Start()
    {
        StartCoroutine("Apagar", 30.0f);
    }

    public void AsignarDueño(GameObject nuevo_dueño)
    {
        dueño = nuevo_dueño;
    }

    public void AsignarArmaUsada(ControladorArma arma)
    {
        arma_usada = arma;
    }

    public virtual void FijarDireccion(Vector3 direccion_salida)
    {
        rastro.enabled = true;
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

    void OnCollisionEnter(Collision contacto)
    {
        Destroy(this.gameObject);
        IDañable objetivo_valido = contacto.gameObject.GetComponent<IDañable>();
        if (objetivo_valido == null) { objetivo_valido = contacto.gameObject.GetComponentInParent<IDañable>(); }
        if (objetivo_valido == null) { return; }

        if (objetivo_valido != null)
        {
            int daño_final = this.daño + arma_usada.ObtenerDaño();
            objetivo_valido.RecibirDaño(daño_final, true, tipoDaño, this.gameObject, framesAplicados);
        }
    }

    IEnumerator Apagar(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        Destroy(this.gameObject);
    }
}
