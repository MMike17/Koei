using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiCore
{
    public class Ai
    {
        /// <summary>
        /// [WIP] Allows to recalculate score according to human brain deformation.
        /// </summary>
        /// <param name="scores"></param>
        /// <param name="hasBeenTest"></param>
        /// <returns></returns>
        public static List<float> NormalizedScore(List<float> scores, List<bool> hasBeenTest)
        {
            List<float> scoresNew = new List<float>();
            return scoresNew;
        }
    }

    public class Neuron
    {
        public int Id { get; set; }
    }

    public class BrainChecker
    {
        public bool CheckEveryNeurons(List<Neuron> neurons)
        {
            bool hasSameId = false;

            for (int i = 0; i < neurons.Count; i++)
            {
                if (neurons[i].Id == neurons[i + 1].Id)
                {
                    hasSameId = false;
                }
                else
                {
                    hasSameId = true;
                }
            }
            return hasSameId;
        }
    }
}
