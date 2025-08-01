using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorGeneradorEnemigo : MonoBehaviour
{
    [Header("Configuracion General")]
    [SerializeField] private Transform posicionGeneracion;

    public Transform ObtenerTransformacion()
    {
        return posicionGeneracion;
    }
}
