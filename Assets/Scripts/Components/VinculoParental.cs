using UnityEngine;

public class VinculoParental : MonoBehaviour
{
    [SerializeField] private GameObject objetoPadre;
    [SerializeField] private string informacion;

    GameObject padre;

    void Awake()
    {
        padre = objetoPadre;
    }

    public void AsignarPadre(GameObject nuevo_padre)
    {
        padre = nuevo_padre;
        objetoPadre = nuevo_padre;
    }

    public GameObject ObtenerPadre()
    {
        return objetoPadre;
    }

}
