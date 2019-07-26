using System.Collections.Generic;
using UnityEngine;

namespace nn
{
    static public class Helper
    {
        public static Matrix4x4 ArrayToMatrix(List<float> array)
        {
            Matrix4x4 matrix = Matrix4x4.zero;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    matrix[i, j] = array[i * 4 + j];
                }
            }

            return matrix;
        }

        public static Color ArrayToColor(List<float> array)
        {
            Color color = Color.white;
            for (int i = 0; i < 3; i++)
            {
                color[i] = array[i];
            }

            return color;
        }

        public static Vector3 ArrayToVector3(List<float> array)
        {
            Vector3 v = Vector3.zero;
            for (int i = 0; i < 3; i++)
            {
                v[i] = array[i];
            }

            return v;
        }
    }
}