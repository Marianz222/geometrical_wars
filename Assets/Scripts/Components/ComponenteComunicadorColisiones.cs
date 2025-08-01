using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]

public class ComponenteComunicadorColisiones : MonoBehaviour
{
    //[SerializeField] private UnityEvent<Collider> alIngresarEnTrigger();
    //[SerializeField] private UnityEvent<Collider> alSalirDeTrigger();

    void OnTriggerEnter(Collider contacto)
    {
        //alIngresarEnTrigger.Invoke(contacto);
    }

    void OnTriggerExit(Collider contacto)
    {
        
    }
}
