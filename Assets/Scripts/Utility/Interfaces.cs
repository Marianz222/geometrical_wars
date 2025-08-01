using UnityEngine;

using GeometricalWars.Enumeraciones;

namespace GeometricalWars.Interfaces
{
    public interface IDañable
    {
        void RecibirDaño(int cantidad, bool directo, TipoDaño tipo, GameObject atacante, float frames_invulnerabilidad);
    }
}