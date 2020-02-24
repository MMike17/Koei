using AiElementsDefinitions;
using AiLib;
using System.Collections.Generic;
using UnityEngine;

namespace AiCore
{
    public class Ai
    {
        public Strategy strategy;
        public List<Memory> memories = new List<Memory>();

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
        public static void MakeChoice(Ai ai, CardAi cardAiScript, EnemyKnowledge knowledge)
        {
            if(DataGame.entityToPlay == Entity.EntityGenre.Ai)
            {
                Debug.Log("<b>[AI]</b> >> AI had make a choice.");
                // List cards in hand
                Debug.Log("<b>[AI]</b> >> CARD => " + cardAiScript.deckObj[0].id + " + " + cardAiScript.deckObj[1].id);
            }
        }

        public static void PlayACard(List<Card> hand)
        {
            Debug.Log("<b>[AI - CARD].Placement</b> >> Placed a card");
        }

        public static void WatchPlayer(CardManager manager, PlayerTroubles playerWeakness)
        {
            Debug.Log("<b>[AI]</b> >> Is watching at player & preparing for next moment");

            if (manager.turn < manager.playedCards.Count)
            {
                // Check the card system
                Debug.Log("<b>[AI - SYSTEM]</b> >> CARD: " + manager.playedCards[manager.turn].strength.ToString() + " | PLAYER WEAKNESS: " + playerWeakness.categoriesWeakness[manager.turn].ToString());

                // Played card is equal to playerTrouble
                if (manager.playedCards[manager.turn].strength == playerWeakness.categoriesWeakness[manager.turn])
                {
                    Debug.Log("DISPLAY CATEGORY >> " + playerWeakness.categoriesWeakness[manager.turn]);
                }
            }
        }
    }
}
