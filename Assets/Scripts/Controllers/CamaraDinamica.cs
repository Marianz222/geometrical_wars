using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

using GeometricalWars.Enumeraciones;

public class CamaraDinamica : MonoBehaviour
{

    [Header("Configuracion General")]
    [SerializeField] private bool centradoCursor = true;
    [SerializeField] private bool ocultarCursor = false;
    [SerializeField] private float sensibilidadCursor = 300.0f;
    [SerializeField] private Vector2 anguloMaximoCamara;
    [SerializeField] private float[] valoresBonusPrecision = new float[2];
    [SerializeField] private float[] valoresBonusVelocidadMovimiento = new float[2];

    [Header("Referencias Componentes")]
    [SerializeField] private CinemachineVirtualCamera camaraPP;
    [SerializeField] private CinemachineVirtualCamera camaraTP;

    [Header("Asignacion de Teclas")]
    [SerializeField] private KeyCode teclaCambioCamara = KeyCode.E;

    [Header("Eventos Disponibles")]
    [SerializeField] private UnityEvent<TipoCamara> alCambiarCamara;

    private bool toggle = false;
    private float bonus_precision;
    private float bonus_velocidad_movimiento;
    private float rotacion_x;
    private bool movimiento_cursor = true;
    private CinemachineVirtualCamera camara_activa = null;
    private GameObject cuerpo_apuntado;
    private GameObject visor_cuerpo;

    void Start()
    {
        cuerpo_apuntado = this.gameObject.transform.parent.gameObject.transform.parent.gameObject;
        visor_cuerpo = camaraPP.LookAt.gameObject;

        if (centradoCursor) { Cursor.lockState = CursorLockMode.Locked; }
        if (ocultarCursor) { }
        Cursor.visible = !ocultarCursor;
    }

    void Update()
    {

        if (camaraPP == null || camaraTP == null) { return; }

        ManejarMovimientoCamara();

        if (Input.GetKeyDown(teclaCambioCamara))
        {
            toggle = !toggle;
        }

        if (toggle)
        {
            camaraPP.Priority = 10;
            camaraTP.Priority = 1;
            camara_activa = camaraPP;
            alCambiarCamara.Invoke(TipoCamara.PRIMERA_PERSONA);
            AsignarBonusPrecision(valoresBonusPrecision[0]);
            AsignarBonusVelocidadMovimiento(valoresBonusVelocidadMovimiento[0]);
        }
        else
        {
            camaraPP.Priority = 1;
            camaraTP.Priority = 10;
            camara_activa = camaraTP;
            alCambiarCamara.Invoke(TipoCamara.TERCERA_PERSONA);
            AsignarBonusPrecision(valoresBonusPrecision[1]);
            AsignarBonusVelocidadMovimiento(valoresBonusVelocidadMovimiento[1]);
        }
    }

    private void ManejarMovimientoCamara()
    {

        if (!movimiento_cursor) { return; }

        float movimiento_mouse_x = Input.GetAxis("Mouse X") * sensibilidadCursor * Time.deltaTime;
        float movimiento_mouse_y = Input.GetAxis("Mouse Y") * sensibilidadCursor * Time.deltaTime;

        // Control vertical
        rotacion_x -= movimiento_mouse_y;
        rotacion_x = Mathf.Clamp(rotacion_x, -anguloMaximoCamara.x, anguloMaximoCamara.x);
        visor_cuerpo.transform.localRotation = Quaternion.Euler(rotacion_x, 0.0f, 0.0f);

        // Control horizontal
        cuerpo_apuntado.transform.Rotate(Vector3.up * movimiento_mouse_x);

    }

    public void AsignarBonusPrecision(float porcentaje) //Unificar este método y el asignarbonusvelocidad en uno que use una enum "TipoAtributo"
    {
        bonus_precision = 1 - porcentaje;
    }

    public float ObtenerBonusPrecision() //Mismo que arriba, unificar con obtener bonus movimiento y añadir enum de atributo
    {
        return bonus_precision;
    }

    public void AsignarBonusVelocidadMovimiento(float porcentaje)
    {
        bonus_velocidad_movimiento = 1 - porcentaje;
    }

    public float ObtenerBonusVelocidadMovimiento()
    {
        return bonus_velocidad_movimiento;
    }

    public void BloquearMovimientoCursor(bool estado)
    {
        if (estado) { Cursor.lockState = CursorLockMode.Locked; }
        else { Cursor.lockState = CursorLockMode.None; }
    }

    public void CambiarVisibilidadCursor(bool estado)
    {
        Cursor.visible = estado;
    }

    public void PermitirMovimientoCursor(bool estado)
    {
        movimiento_cursor = estado;
    }
}
