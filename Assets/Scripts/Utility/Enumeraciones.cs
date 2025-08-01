using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeometricalWars.Enumeraciones
{

    public enum Arma
    {
        RIFLE_TRIPLE_BARRIL,
        RIFLE_PULSO,
        PROYECTOR_IGNEO
    }
    
    public enum TeclasJugador
    {
        DISPARO,
        IMPULSO,
        SALTO,
        HABILIDAD_MEJORA
    }

    public enum RecursoEntidad
    {
        VIDA,
        BLINDAJE,
        COMBUSTIBLE
    }

    public enum TipoDaño
    {
        CINETICO,
        CONTACTO,
        IGNEO,
        ENERGETICO,
        EXPLOSIVO
    }

    public enum TipoProyectil
    {
        HITSCAN,
        DESPLAZAMIENTO,
        DESPLAZAMIENTO_CON_CAIDA,
    }

    public enum TipoCamara
    {
        PRIMERA_PERSONA,
        TERCERA_PERSONA,
        SUPERIOR
    }
}
