using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AiElementsDefinitions;

public class TurnSys : MonoBehaviour
{
    public CardAi cardAi;
    
    public void EndTurn()
    {
        if(DataGame.entityToPlay == Entity.EntityGenre.Ai)
        {
            Debug.Log("<b>TURN MANAGER</b> >> Player Turn");
            DataGame.entityToPlay = Entity.EntityGenre.Player;
        }
        else if(DataGame.entityToPlay == Entity.EntityGenre.Player)
        {
            // Define the variable to allow AI to make a futur choice
            cardAi.aiHasPlayed = false;

            // Display that AI is making something
            Debug.Log("<b>TURN MANAGER</b> >> AI Turn");

            // Define current 'entity to play' as AI
            DataGame.entityToPlay = Entity.EntityGenre.Ai;
        }
    }
}
