using UnityEngine;

using GeometricalWars.Interfaces;
using GeometricalWars.Enumeraciones;

public class TrampaSierra : MonoBehaviour
{
    [Header("Referencias a Partes")]
    [SerializeField] private GameObject sierraCircular;

    [Header("Funcionamiento")]
    [SerializeField] private float velocidadRotacion = 5.0f;
    [SerializeField] private bool fijarVelocidad = false;

    [Header("Daño")]
    [SerializeField] private int daño = 10;
    [SerializeField] private float framesInmunidad = 25f;
    [SerializeField] private TipoDaño[] tipoDaño = { TipoDaño.CONTACTO };

    private bool encendida = true;

    void Start()
    {
        DefinirVelocidadRotacion();
    }

    void Update()
    {
        GirarSierra();
    }

    void OnCollisionStay(Collision contacto)
    {
        IDañable objetivo_valido = contacto.gameObject.GetComponent<IDañable>();
        if (objetivo_valido == null) { objetivo_valido = contacto.gameObject.GetComponentInParent<IDañable>(); }
        if (objetivo_valido == null) { return; }
        if (objetivo_valido != null)
        {
            objetivo_valido.RecibirDaño(daño, true, tipoDaño, this.gameObject, framesInmunidad);
            if (contacto.gameObject.TryGetComponent<Rigidbody>(out Rigidbody cuerpo_rigido))
            {
                float fuerza_empuje = 1.1f;
                cuerpo_rigido.AddForce(cuerpo_rigido.transform.forward * -1 * fuerza_empuje, ForceMode.Impulse);
            }
        }
    }

    private void DefinirVelocidadRotacion()
    {

        if (fijarVelocidad)
        {
            return;
        }

        float valor_bajo = velocidadRotacion * 0.8f;
        float valor_alto = velocidadRotacion * 1.2f;

        velocidadRotacion = Random.Range(valor_bajo, valor_alto);

        if (Random.value <= 0.5)
        {
            velocidadRotacion = velocidadRotacion * -1;
        }

    }

    private void Encender()
    {
        encendida = true;
    }

    private void Apagar()
    {
        encendida = false;
    }

    private void GirarSierra()
    {
        if (!encendida)
        {
            return;
        }

        sierraCircular.transform.Rotate(Vector3.forward * velocidadRotacion);

    }

}
