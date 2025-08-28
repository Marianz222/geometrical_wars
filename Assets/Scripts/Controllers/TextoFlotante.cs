using TMPro;
using UnityEngine;

public class TextoFlotante : MonoBehaviour
{
    [SerializeField] private string cadena_texto = "N/A";
    //[SerializeField] private bool desvanecimiento = true;
    //[SerializeField] private float tiempoDesvanecer = 3.0f;
    [SerializeField] private Vector3 direccionMovimiento;
    [SerializeField] private float velocidadMovimiento = 2.0f;

    private TMP_Text texto_visual;

    void Update()
    {
        this.gameObject.transform.position += (direccionMovimiento * velocidadMovimiento);
    }

    public void ModificarTexto(string nueva_cadena)
    {
        cadena_texto = nueva_cadena;
    }

    public void ModificarColor(Color nuevo_color)
    {
        texto_visual.color = nuevo_color;
    }
}
