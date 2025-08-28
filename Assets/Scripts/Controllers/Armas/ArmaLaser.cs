using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class ArmaLaser : Arma
{
    private int rango;
    private float tiempo_daño_final;
    private LineRenderer renderizador_rayo;

    void Start()
    {
        renderizador_rayo = GetComponent<LineRenderer>();
        renderizador_rayo.enabled = false;
        rango = estadisticasArma.Rango;
    }

    void Update()
    {
        if (disparando)
        {
            //P
        }
    } 

    public override void disparar(Vector3 posicion_origen, Vector3 posicion_destino)
    {
        //StartCoroutine("DispararLaser", posicion_origen, posicion_destino, 0.3f);
    }

    IEnumerator DispararLaser(Vector3 origen, Vector3 destino, float tiempo_desvanecimiento)
    {
        renderizador_rayo.SetPosition(0, origen);
        renderizador_rayo.SetPosition(1, destino);
        renderizador_rayo.enabled = true;
        yield return new WaitForSeconds(tiempo_desvanecimiento);
        renderizador_rayo.enabled = false;
        foreach (GradientAlphaKey key in renderizador_rayo.colorGradient.alphaKeys)
        {
            //Placeholder
        }
    }

}
