using UnityEngine;
using UnityEngine.Events;

using GeometricalWars.Enumeraciones;

namespace GeometricalWars.Eventos
{
    [System.Serializable]
    public class EventoRecibirDaño : UnityEvent<int, bool, TipoDaño, GameObject> { }

    [System.Serializable]
    public class EventoModificarVida : UnityEvent<float> { }

    [System.Serializable]
    public class EventoModificarBlindaje : UnityEvent<float> { }
}