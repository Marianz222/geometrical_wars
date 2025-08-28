using UnityEngine;
using System.Collections;

using GeometricalWars.Interfaces;

public class Bala : Proyectil
{

    private Rigidbody cuerpo_rigido;
    private Collider colisionador;
    private ParticleSystem particulas_impacto;
    private TrailRenderer rastro;
    private MeshRenderer malla;

    void Awake()
    {
        cuerpo_rigido = GetComponent<Rigidbody>();
        colisionador = GetComponent<Collider>();
        particulas_impacto = GetComponent<ParticleSystem>();
        malla = GetComponentInChildren<MeshRenderer>();

        rastro = GetComponent<TrailRenderer>();
        rastro.enabled = true;
    }

    void OnCollisionEnter(Collision contacto)
    {
        if (contacto.collider.gameObject == dueño) { Physics.IgnoreCollision(colisionador, contacto.collider, true); }
        StartCoroutine("PrepararDestruccion", 0.2f);
        IDañable objetivo_valido = contacto.gameObject.GetComponent<IDañable>();

        if (objetivo_valido != null)
        {
            int daño_final = this.daño + arma_usada.ObtenerDaño();
            objetivo_valido.RecibirDaño(daño_final, true, tipoDaño, this.gameObject, framesAplicados);
        }
    }

    public override void FijarDireccion(Vector3 direccion_salida)
    {
        ForceMode modo_fuerza = ForceMode.Impulse;
        cuerpo_rigido.AddForce(direccion_salida * velocidad, modo_fuerza);
    }

    IEnumerator PrepararDestruccion(float segundos)
    {
        MeshRenderer malla = gameObject.GetComponentInChildren<MeshRenderer>();
        if (particulas_impacto != null) { particulas_impacto.Play(); }
        if (rastro != null) { rastro.enabled = false; }
        if (malla != null) { malla.enabled = false; }
        yield return new WaitForSeconds(segundos);
        Destroy(gameObject);
    }
}
