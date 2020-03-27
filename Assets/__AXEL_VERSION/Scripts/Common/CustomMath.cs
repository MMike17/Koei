using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomMath
{
    public class MathC
    {
        public static float DefineBiggestOfSerie(List<float> serie)
        {
            return serie.Max();
        }
        public static float DefineSmallestOfSerie(List<float> serie)
        {
            return serie.Min();
        }
    }
}
