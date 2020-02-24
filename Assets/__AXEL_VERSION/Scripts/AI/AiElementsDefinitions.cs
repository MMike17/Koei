using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AiLib;

namespace AiElementsDefinitions
{
    // DEFINE EVERYTHING THAT IS OUT OF AI DEFINITIONS //

    public class Turn
    {
        public static int idTurn;
        public static Entity entityToPlay;

        public Turn(int nbTurns)
        {
            idTurn = nbTurns;
        }
    }

    public class Strategy
    {
        // Define strategies
        public enum Strategies { Agresive, Calm, Defensive, Bluff }
        public Strategies strategy;

        // Define the good and bad moments in fight
        private int totalGoodMoments;
        private int totalBadMoments;

        /// <summary>
        /// Define a new strategy
        /// </summary>
        /// <param name="memories"></param>
        /// <returns></returns>
        public Strategies DefineNewStrategy(List<Memory> memories)
        {
            for(int i = 0; i < memories.Count; i++)
            {
                for(int moments = 0; moments < memories[i].moments.Count; moments++)
                {
                    // Calculate Good Moments Total
                    if(memories[i].moments[moments].momentGenre == Moment.MomentGenre.Good)
                    {
                        totalGoodMoments++;
                    }
                    // Calculte Bad Moments Total
                    if(memories[i].moments[moments].momentGenre == Moment.MomentGenre.Bad)
                    {
                        totalBadMoments++;
                    }
                }
            }

            // Define the behaviour of the AI at x moment
            if(totalGoodMoments > totalBadMoments)
            {
                return Strategies.Calm;
            }
            else if(totalGoodMoments < totalBadMoments)
            {
                return Strategies.Agresive;
            }
            else
            {
                int randBehaviour = Random.Range(0, 1);
                if(randBehaviour > 0)
                {
                    return Strategies.Defensive;
                }
                else
                {
                    return Strategies.Bluff;
                }
            }
        }
    }

    public class Behaviour
    {
        public enum BehaviourChoices { Agresive, Calm, Defensive, Bluff, Escape, Charge }
        public BehaviourChoices personality;
    }

    public class Entity
    {
        public enum EntityGenre { Player, Ai }
    }
}
