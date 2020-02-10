using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace CardSystem
{
    public class CardSys
    {
        /// <summary>
        /// Allows to shuffle one, or several decks
        /// </summary>
        public static List<Card> Shuffle(List<Card> deckToShuffle)
        {
            int deckLength = deckToShuffle.Count;                         // Init the deck's length
            List<Card> listCardsOfDeck = deckToShuffle;                   // Init a list of cards in deck

            for (int i = 0; i < deckLength; i++)
            {
                // Shuffle Deck
                int randCard = Random.Range(i, listCardsOfDeck.Count);

                Card temporary = listCardsOfDeck[i];
                listCardsOfDeck[i] = listCardsOfDeck[randCard];
                listCardsOfDeck[randCard] = temporary;
            }

            return listCardsOfDeck;
        }

        /// <summary>
        /// Allows to draw one card, or several cards. Available for Player and AI.
        /// </summary>
        /// <returns></returns>
        public static Card DrawCard(List<Card> deck)
        {
            Card drawedCard = deck[0];
            deck.Remove(drawedCard);

            return drawedCard;
        }

        public static List<Card> GenerateDeckFullRandom(int maxCardsInDeck, List<Card> customCards)
        {

            List<Card> newDeck = customCards;

            for (int i = 0; i < maxCardsInDeck; i++)
            {
                newDeck.Add(new Card());
            }
            Shuffle(newDeck);

            /*
             * CREATE A DEBUG.LOG TO KNOW EVERY DECKS
            for(int x = 0; x < newDeck.Count; x++)
            {
                Debug.Log(newDeck[x].name);
            }
            */

            return newDeck;
        }
    }
}
