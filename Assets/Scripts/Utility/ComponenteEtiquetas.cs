using System;
using UnityEngine;

public class ComponenteEtiquetas : MonoBehaviour
{
    [SerializeField] private String[] etiquetas;

    public bool ContieneEtiqueta(String tag_id)
    {
        for (int i = 0; i < etiquetas.Length; i++)
        {
            if (etiquetas[i] == tag_id)
            {
                return true;
            }
        }
        return false;
    }
}
