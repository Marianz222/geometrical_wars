using UnityEngine;

using GeometricalWars.Enumeraciones;

[CreateAssetMenu(fileName = "New Weapon Blueprint", menuName = "Custom/Weapon Blueprint")]

public class PlantillaArma : ScriptableObject
{
    [SerializeField] private ArmaID identificadorArma;
    [SerializeField] private FisicasDisparo tipoFisicas = FisicasDisparo.DESPLAZAMIENTO;
    [SerializeField] private int daño;
    [SerializeField] private int rango_maximo = 100;
    [SerializeField] private TipoDaño[] fuenteDaño = new TipoDaño[] { TipoDaño.GENERICO };
    [SerializeField] private float cadenciaDisparo;
    [SerializeField] private bool consumeMunicion = true;
    [SerializeField] private Vector2 dispersionBase = new Vector2(10, 10);
    [SerializeField] private Vector3 recoil = new Vector3(0.0f, 0.0f, 1.0f);
    [SerializeField] private bool puedeSobrecalentarse;
    [SerializeField] private float calorGenerado;
    [SerializeField] private GameObject proyectil;

    [Header("Configuraciones de Rafaga")]
    [SerializeField] private bool esRafaga = false;
    [SerializeField][Range(0.05f, 3.0f)] private float intervaloRafaga = 0.2f;
    [SerializeField][Range(2, 12)] private int conteoRafaga = 2;

    public ArmaID Arma { get => identificadorArma; set => identificadorArma = value; }
    public FisicasDisparo FisicasDisparo { get => tipoFisicas; set => tipoFisicas = value; }
    public int Daño { get => daño; set => daño = value; }
    public int Rango { get => rango_maximo; set => rango_maximo = value; }
    public float Cadencia { get => cadenciaDisparo; set => cadenciaDisparo = value; }
    public bool ConsumeMunicion { get => consumeMunicion; set => consumeMunicion = value; }
    public Vector2 Dispersion { get => dispersionBase; set => dispersionBase = value; }
    public Vector3 Recoil { get => recoil; set => recoil = value; }
    public bool Sobrecalentamiento { get => puedeSobrecalentarse; set => puedeSobrecalentarse = value; }
    public float CalorGenerado { get => calorGenerado; set => calorGenerado = value; }
    public GameObject Proyectil { get => proyectil; set => proyectil = value; }

    public bool EsRafaga { get => esRafaga; set => esRafaga = value; }
    public float IntervaloRafaga { get => intervaloRafaga; set => intervaloRafaga = value; }
    public int ConteoRafaga { get => conteoRafaga; set => conteoRafaga = value; }
}
