namespace GeometricalWars.Enumeraciones
{

    public enum ArmaID
    {
        RIFLE_TRIPLE_BARRIL,
        RIFLE_PULSO,
        PROYECTOR_IGNEO,
        RIFLE_REGLAMENTARIO
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
        GENERICO,
        CINETICO,
        CONTACTO,
        IGNEO,
        ENERGETICO,
        EXPLOSIVO
    }

    public enum FisicasDisparo
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
