// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using AiCore;
// using AiLib;
// using AiElementsDefinitions;
// using CardSystem;

// public class CardAi : MonoBehaviour
// {
//     [Header("Cards Settings")]
//     public List<Card> deck = new List<Card>();
//     [Header("Knowledges")]
//     public EnemyKnowledge knowledge;
//     public PlayerTroubles currentPlayerTroubles;
//     [Header("Objects Attribution")]
//     public TurnSys turnSys;
//     public CardManager manager;

//     [HideInInspector]
//     public List<CardObj> deckObj = new List<CardObj>();

//     [HideInInspector]
//     public bool aiHasPlayed;

//     private Ai newAi;

//     private void Start()
//     {
//         // Generate CardObj to each card ; useful to get probabilities
//         for (int cards = 0; cards < deck.Count; cards++)
//         {
//             deckObj.Add(new CardObj(deck[cards], 50f));
//         }

//         // Generate new AI
//          newAi = Ai.CreateNewAi();
//          Debug.Log("<b>[AI]</b> >> AI was initialized and defined new strategy.");
//     }

//     private void Update()
//     {
//         if (DataGame.entityToPlay == Entity.EntityGenre.Ai)
//         {
//             if (!aiHasPlayed)
//             {
//                 // AI make a choice
//                 Debug.Log("<b>[AI]</b> >> AI is about to make a choice.");
//                 Ai.MakeChoice(newAi, GetComponent<CardAi>(), knowledge);

//                 // Create the hand of enemy
//                 // HERE

//                 // Play a card
//                 Ai.PlayACard(deck);

//                 // Finish turn
//                 turnSys.EndTurn();

//                 // Finish loop
//                 aiHasPlayed = true;
//             }
//         }
//         else
//         {
//             Ai.WatchPlayer(manager, currentPlayerTroubles);
//         }
//     }
// }
