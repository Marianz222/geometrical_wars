using UnityEngine;

public class Bala : Proyectil
{

    private Rigidbody cuerpo_rigido;

    void Awake()
    {
        cuerpo_rigido = GetComponent<Rigidbody>();
    }

    public override void FijarDireccion(Vector3 direccion_salida)
    {
        ForceMode modo_fuerza = ForceMode.Impulse;
        cuerpo_rigido.AddForce(direccion_salida * velocidad, modo_fuerza);
    }
}
