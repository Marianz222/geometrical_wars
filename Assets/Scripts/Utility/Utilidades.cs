using System;
using System.Collections;
using UnityEngine;

namespace GeometricalWars.Utilidad
{
    public static class Utilidades
    {
        public static Vector2 LimitarVector(Vector2 vector, int minimo, int maximo)
        {
            Vector2 vector_limitado = new Vector2();
            vector_limitado.x = Mathf.Clamp(vector.x, minimo, maximo);
            vector_limitado.y = Mathf.Clamp(vector.y, minimo, maximo);
            return vector_limitado;
        }

        public static Vector3 LimitarVector(Vector3 vector, int minimo, int maximo)
        {
            Vector3 vector_limitado = new Vector3();
            vector_limitado.x = Mathf.Clamp(vector.x, minimo, maximo);
            vector_limitado.y = Mathf.Clamp(vector.y, minimo, maximo);
            vector_limitado.z = Mathf.Clamp(vector.z, minimo, maximo);
            return vector_limitado;
        }
    }
}


