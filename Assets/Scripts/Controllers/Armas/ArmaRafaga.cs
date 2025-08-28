using UnityEngine;
using System.Collections;

public class ArmaRafaga : Arma
{

   int cantidad_disparos;
   float intervalo_entre_disparos;

   void Awake()
   {
      cantidad_disparos = estadisticasArma.ConteoRafaga;
      intervalo_entre_disparos = estadisticasArma.IntervaloRafaga;
   }

   public override void disparar(Vector3 origen, Vector3 destino)
   {
      if (!estadisticasArma.EsRafaga) { return; }
      StartCoroutine("dispararRafaga", destino);
   } 

   IEnumerator dispararRafaga(Vector3 destino)
   {
      for (int i = 0; i < cantidad_disparos; i++)
      {
         base.disparar(posicionDisparo.position, destino);
         yield return new WaitForSeconds(intervalo_entre_disparos);
      }
   }
}
