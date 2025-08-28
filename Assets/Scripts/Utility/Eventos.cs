using UnityEngine;
using UnityEngine.Events;

using GeometricalWars.Enumeraciones;

namespace GeometricalWars.Eventos
{
    //Eventos ENTIDAD:

    [System.Serializable]
    public class EventoRecibirDaño : UnityEvent<int, bool, TipoDaño[], GameObject> { }

    [System.Serializable]
    public class EventoModificarVida : UnityEvent<float> { }

    [System.Serializable]
    public class EventoModificarBlindaje : UnityEvent<float> { }

    [System.Serializable]
    public class EventoModificarCombustible : UnityEvent<float> { }

    //Eventos ENEMIGO:

    [System.Serializable]
    public class EventoEnemigoEliminado : UnityEvent<GameObject, TipoDaño[], int, int> { }
    //Evento: Enemigo Eliminado - (GameObject atacante, TipoDaño fuente_daño, int daño_recibido, int poder_oleada)

    //Eventos PICKUP:

    [System.Serializable]
    public class EventoConsumirPickup : UnityEvent<RecursoEntidad, int, bool> { }

    //Eventos SISTEMA APUNTADO:

    [System.Serializable]
    public class EventoDefinirObjetivoApuntado : UnityEvent<Vector3> { }

    //Eventos LEVEL MANAGER:

    [System.Serializable]
    public class EventoIniciarOleada : UnityEvent<int> { }

    [System.Serializable]
    public class EventoFinalizarOleada : UnityEvent<int> { }
}