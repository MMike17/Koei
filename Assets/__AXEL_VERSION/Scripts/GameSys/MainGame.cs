using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CardGameLib;
using CSVReader;

public class MainGame : MonoBehaviour
{
    private List<GameCard> cardsInHand = new List<GameCard>();
    public string cardsPath;

    public Image[] images;

    private void Start()
    {
        // Create the deck
        List<GameCard> deck = GameSys.GenerateDeck(5, cardsPath);

        //images[0].sprite = CsvFiles.GetSpriteFromPath(cardsPath, 2, 0);               // WORKS

        // Design the hand
        // FIRST METHOD
        cardsInHand.Add(deck[0]);
        cardsInHand.Add(deck[1]);
        cardsInHand.Add(deck[2]);
        cardsInHand.Add(deck[3]);
        cardsInHand.Add(deck[4]);


        // Set sprites on cards
        images[0].sprite = cardsInHand[0].Image;
        images[1].sprite = cardsInHand[1].Image;
        images[2].sprite = cardsInHand[2].Image;
        images[3].sprite = cardsInHand[3].Image;
        images[4].sprite = cardsInHand[4].Image;
    }
}
