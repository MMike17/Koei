using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Using simple namespace
using CSVReader;

namespace CardGameLib
{
    public class GameCard
    {
        public string Effect { get; set; }
        public string Name { get; set; }
        public Sprite Image { get; set; }
        public static List<GameCard> cardsInDeck;

        // Define the card system
        public GameCard(int csvColumn, string pathCardSystem)
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
        public static List<GameCard> Shuffle(List<GameCard> deckToShuffle)
        {
            int deckLength = deckToShuffle.Count;                         // Init the deck's length
            List<GameCard> listCardsOfDeck = deckToShuffle;                   // Init a list of cards in deck

            for (int i = 0; i < deckLength; i++)
            {
                // Shuffle Deck
                int randCard = Random.Range(i, listCardsOfDeck.Count);

                GameCard temporary = listCardsOfDeck[i];
                listCardsOfDeck[i] = listCardsOfDeck[randCard];
                listCardsOfDeck[randCard] = temporary;
            }

            return listCardsOfDeck;
        }

        /// <summary>
        /// Allows to draw one card, or several cards. Available for Player and AI.
        /// </summary>
        /// <returns></returns>
        public static GameCard DrawCard(List<GameCard> deck)
        {
            GameCard drawedCard = deck[0];
            deck.Remove(drawedCard);

            return drawedCard;
        }


        public static List<GameCard> GenerateDeckFullRandom(int maxCardsInDeck, string cardsPath)
        {
            List<GameCard> newDeck = new List<GameCard>();

            for (int i = 0; i < maxCardsInDeck; i++)
            {
                newDeck.Add(new GameCard(i, cardsPath));
            }
            Shuffle(newDeck);

            return newDeck;
        }

        public static List<GameCard> GenerateDeck(int maxCardsInDeck, string cardsPath)
        {
            List<GameCard> newDeckReturn = new List<GameCard>();
            // Define total cards of game
            List<GameCard> totalCards = new List<GameCard>();

            for (int cards = 0; cards < CsvFiles.GetNumberOfLinesByPath(cardsPath); cards++)
            {
                totalCards.Add(new GameCard(cards, cardsPath));
            }
            // Shuffle deck
            List<GameCard> newDeck = Shuffle(totalCards);

            // Generate deck
            for (int i = 0; i < maxCardsInDeck; i++)
            {
                newDeckReturn.Add(newDeck[i]);
            }

            return newDeckReturn;
        }
    }
}
