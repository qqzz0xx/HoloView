using System.Collections.Generic;
using UnityEngine;

namespace nn
{
    static public class Helper
    {
        public static Matrix4x4 ArrayToMatrix(List<float> array)
        {
            Matrix4x4 matrix = Matrix4x4.zero;
            for (int i = 0; i < 16; i++)
            {
                matrix[i] = array[i];
            }

            return matrix;
        }
    }
}