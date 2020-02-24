using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AiCore;
using AiLib;
using AiElementsDefinitions;

public class CardAi : MonoBehaviour
{
    public TurnSys turnSys;

    [HideInInspector]
    public bool aiHasPlayed;

    private Ai newAi;

    private void Start()
    {
        // Generate new AI
         newAi = Ai.CreateNewAi();
         Debug.Log("<b>[AI]</b> >> AI was initialized and defined new strategy.");
    }

    private void Update()
    {
        if (DataGame.entityToPlay == Entity.EntityGenre.Ai)
        {
            if (!aiHasPlayed)
            {
                // AI make a choice
                Debug.Log("<b>[AI]</b> >> AI is about to make a choice.");
                Ai.MakeChoice(newAi);
                aiHasPlayed = true;
            }
        }
    }
}
