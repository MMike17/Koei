using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AiCore;
using AiLib;
using AiElementsDefinitions;

public class CardAi : MonoBehaviour
{
    public bool aiCanPlay;
    public bool aiHasPlayed;

    private void Start()
    {
        // Generate new AI
        Ai newAi = Ai.CreateNewAi();
        Debug.Log("<b>[AI]</b> >> AI was initialized and defined new strategy.");
    }

    private void Update()
    {
        if (aiCanPlay)
        {
            if (!aiHasPlayed)
            {
                // AI make a choice
                Debug.Log("<b>[AI]</b> >> AI is about to make a choice.");
                aiHasPlayed = true;
            }
        }
    }
}
