using UnityEngine;

using GeometricalWars.Enumeraciones;
using GeometricalWars.Eventos;

public class PickupRecurso : MonoBehaviour
{
    [SerializeField] private RecursoEntidad tipo = RecursoEntidad.VIDA;
    [SerializeField] private bool esPorcentual = false;
    [SerializeField] private int cantidadRecuperada = 25;
    [SerializeField] private Material[] materialesReferencia = { null, null, null };

    [Header("Eventos Disponibles")]
    [SerializeField] private EventoConsumirPickup alSerConsumido;

    private RecursoEntidad tipo_recurso;
    private int valor;
    private MeshRenderer malla;

    void Awake()
    {
        tipo_recurso = tipo;
        valor = cantidadRecuperada;
        malla = GetComponentInChildren<MeshRenderer>();
        ModificarRecurso(valor, tipo_recurso);
    }

    void OnTriggerEnter(Collider contacto)
    {
        ComponenteEtiquetas etiquetas_contacto = contacto.gameObject.GetComponent<ComponenteEtiquetas>();
        if (etiquetas_contacto == null) { etiquetas_contacto = contacto.gameObject.GetComponentInParent<ComponenteEtiquetas>(); }
        if (etiquetas_contacto == null) { return; }
        if (etiquetas_contacto.ContieneEtiqueta("Player"))
        {
            //Añadir listener de evento aca en lugar de arriba
            Consumir(contacto);
        }

    }

    private void Consumir(Collider contacto)
    {
        alSerConsumido.Invoke(tipo_recurso, valor, esPorcentual);
        Jugador jugador = contacto.gameObject.GetComponent<Jugador>(); //Esto se va cuando logre conectar dinamicamente el evento (arriba)
        if (jugador != null) { jugador.RecuperarRecurso(valor, tipo); } //Esto se va cuando logre conectar dinamicamente el evento (arriba)
        Destroy(this.gameObject);
    }

    public void ModificarRecurso(RecursoEntidad nuevo_tipo)
    {
        tipo_recurso = nuevo_tipo;

        switch (tipo_recurso)
        {
            case RecursoEntidad.VIDA:
                malla.material = materialesReferencia[0];
                break;
            case RecursoEntidad.BLINDAJE:
                malla.material = materialesReferencia[1];
                break;
            case RecursoEntidad.COMBUSTIBLE:
                malla.material = materialesReferencia[2];
                break;
        }
    }

    public void ModificarRecurso(int cantidad, RecursoEntidad nuevo_tipo)
    {
        tipo_recurso = nuevo_tipo;
        valor = cantidad;

        switch (tipo_recurso)
        {
            case RecursoEntidad.VIDA:
                malla.material = materialesReferencia[0];
                break;
            case RecursoEntidad.BLINDAJE:
                malla.material = materialesReferencia[1];
                break;
            case RecursoEntidad.COMBUSTIBLE:
                malla.material = materialesReferencia[2];
                break;
        }
    }

    public RecursoEntidad ObtenerRecurso()
    {
        return tipo_recurso;
    }
}
