using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AiLib;
using AiElementsDefinitions;

namespace AiCore
{
    public class Ai
    {
        public Strategy strategy;
        public List<Memory> memories = new List<Memory>();
        Entity.EntityGenre entityToPlay = new Entity.EntityGenre();

        /// <summary>
        /// [WIP] Allows to recalculate score according to human brain deformation.
        /// </summary>
        /// <param name="scores"></param>
        /// <param name="hasBeenTest"></param>
        /// <returns></returns>
        public List<float> NormalizedScore(List<float> scores, List<bool> hasBeenTest)
        {
            List<float> scoresNew = new List<float>();
            return scoresNew;
        }

        public static Ai CreateNewAi()
        {
            // AI was created here. 
            Ai newAi = new Ai();

            // Randomize the new strategy
            int randBehaviour = Random.Range(0, 3);
            Strategy newStrat = new Strategy();

            switch (randBehaviour)
            {
                case 0:
                    newStrat.strategy = Strategy.Strategies.Agresive;
                    break;

                case 1:
                    newStrat.strategy = Strategy.Strategies.Bluff;
                    break;

                case 2:
                    newStrat.strategy = Strategy.Strategies.Calm;
                    break;

                case 3:
                    newStrat.strategy = Strategy.Strategies.Defensive;
                    break;
            }

            // Define the start strategy of the AI
            newAi.strategy = newStrat;

            return newAi;
        }


        // HERE: Allows to AI to make choice
        public static void MakeChoice(Ai ai)
        {
            if(ai.entityToPlay == Entity.EntityGenre.Ai)
            {
                Debug.Log("<b>[AI]</b> >> AI had make a choice.");
            }
        }
    }
}
