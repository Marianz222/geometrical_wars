using UnityEngine;

using GeometricalWars.Enumeraciones;
using UnityEditor.UIElements;

[CreateAssetMenu(fileName = "New Weapon Blueprint", menuName = "Custom/Weapon Blueprint")]

public class PlantillaArma : ScriptableObject
{
    [SerializeField] private Arma identificador_arma;
    [SerializeField] private int daño;
    [SerializeField] private float cadenciaDisparo;
    [SerializeField] private bool consumeMunicion = true;
    [SerializeField] private Vector2 dispersionBase = new Vector2(10, 10);
    [SerializeField] private int recoil;
    [SerializeField] private bool puedeSobrecalentarse;
    [SerializeField] private float calorGenerado;
    [SerializeField] private GameObject proyectil;

    public Arma Arma { get => identificador_arma; set => identificador_arma = value; }
    public int Daño { get => daño; set => daño = value; }
    public float Cadencia { get => cadenciaDisparo; set => cadenciaDisparo = value; }
    public bool ConsumeMunicion { get => consumeMunicion; set => consumeMunicion = value; }
    public Vector2 Dispersion { get => dispersionBase; set => dispersionBase = value; }
    public int Recoil { get => recoil; set => recoil = value; }
    public bool Sobrecalentamiento { get => puedeSobrecalentarse; set => puedeSobrecalentarse = value; }
    public float CalorGenerado { get => calorGenerado; set => calorGenerado = value; }
    public GameObject Proyectil { get => proyectil; set => proyectil = value; }
}
