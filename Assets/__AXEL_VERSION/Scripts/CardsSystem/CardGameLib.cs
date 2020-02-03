using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Using simple namespace
using CSVReader;

namespace CardGameLib
{
    public class Card
    {
        public string Effect { get; set; }
        public string Name { get; set; }
        public Sprite Image { get; set; }
        public static List<Card> cardsInDeck;

        // Define the card system
        public Card(int csvColumn, string pathCardSystem)
        {
            // Define the effect
            Name = CsvFiles.GetEntryFromPath(pathCardSystem, 0, csvColumn);
            Effect = CsvFiles.GetEntryFromPath(pathCardSystem, 1, csvColumn);
            Image = CsvFiles.GetSpriteFromPath(pathCardSystem, 2, csvColumn);
        }
    }

    public class GameSys
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

        public static List<Card> GenerateDeckFullRandom(int maxCardsInDeck, string cardsPath)
        {
            List<Card> newDeck = new List<Card>();

            for (int i = 0; i < maxCardsInDeck; i++)
            {
                newDeck.Add(new Card(i, cardsPath));
            }
            Shuffle(newDeck);

            return newDeck;
        }

        public static List<Card> GenerateDeck(int maxCardsInDeck, string cardsPath)
        {
            List<Card> newDeckReturn = new List<Card>();
            // Define total cards of game
            List<Card> totalCards = new List<Card>();

            for (int cards = 0; cards < CsvFiles.GetNumberOfLinesByPath(cardsPath); cards++)
            {
                totalCards.Add(new Card(cards, cardsPath));
            }
            // Shuffle deck
            List<Card> newDeck = Shuffle(totalCards);

            // Generate deck
            for (int i = 0; i < maxCardsInDeck; i++)
            {
                newDeckReturn.Add(newDeck[i]);
            }

            return newDeckReturn;
        }
    }
}
