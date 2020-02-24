using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AiCore;
using AiLib;
using AiElementsDefinitions;

public class CardAi : MonoBehaviour
{
    public TurnSys turnSys;
    public List<Card> deck = new List<Card>();

    [HideInInspector]
    public List<CardObj> deckObj = new List<CardObj>();

    [HideInInspector]
    public bool aiHasPlayed;

    private Ai newAi;

    private void Start()
    {
        // Generate CardObj to each card ; useful to get probabilities
        for (int cards = 0; cards < deck.Count; cards++)
        {
            deckObj.Add(new CardObj(deck[cards], 50f));
        }

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
                Ai.MakeChoice(newAi, GetComponent<CardAi>());
                aiHasPlayed = true;
            }
        }
    }
}
