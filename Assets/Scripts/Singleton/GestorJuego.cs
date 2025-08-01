using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GestorJuego : MonoBehaviour
{
    public static GestorJuego Instancia { get; private set; }

    void Awake()
    {

        if (Instancia == null)
        {

            Instancia = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {

            Destroy(gameObject);

        }

    }

    //Reiniciar Escena Actual: Método que reinicia la escena actualmente activa, sin carga asíncrona
    public void ReiniciarEscenaActual()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CambiarEscena(int indiceObjetivo)
    {

        if (indiceObjetivo == SceneManager.GetActiveScene().buildIndex)
        {

            Debug.Log("No se pudo intercambiar de escena: Los indices son iguales");

            return;

        }

        StartCoroutine(CargarEscenaAsincrona(indiceObjetivo));

    }

    //Cerrar Juego: Método que cierra la aplicación
    public void CerrarJuego()
    {

        Debug.Log("[INFO/DEBUG]: Cerrando el juego");

        Application.Quit();

    }

    //Cargar Escena Asíncrona [Corrutina]: Descarga la escena actual, prepara la carga asíncrona de la escena objetivo
    //(definida por el indice que se suministra por parámetro), carga la escena y la activa, y descarga
    //asíncronamente la escena previa. Permite intercambiar entre escenas con carga progresiva
    IEnumerator CargarEscenaAsincrona(int indice)
    {

        AsyncOperation cargaProgresiva = SceneManager.LoadSceneAsync(indice, LoadSceneMode.Additive);
        cargaProgresiva.allowSceneActivation = false;

        string nombreEscena = SceneManager.GetSceneByBuildIndex(indice).name;

        while (cargaProgresiva.progress < 0.9f)
        {

            Debug.Log("[INFO/DEBUG]: Cargando escena con nombre: " + nombreEscena + " y progreso: " + (cargaProgresiva.progress * 100) + "%");
            yield return null;

        }

        cargaProgresiva.allowSceneActivation = true;

        yield return new WaitUntil(() => cargaProgresiva.isDone);

        Scene escenaSiguiente = SceneManager.GetSceneByBuildIndex(indice);
        Scene escenaAnterior = SceneManager.GetActiveScene();

        SceneManager.SetActiveScene(escenaSiguiente);

        if (escenaAnterior.isLoaded)
        {

            SceneManager.UnloadSceneAsync(escenaAnterior);

        }

    }

}
