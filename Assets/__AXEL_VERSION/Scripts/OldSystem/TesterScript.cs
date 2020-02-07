using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using CSVReader;
using CardGameLib;

public class TesterScript : MonoBehaviour
{
    public string namePath;
    public string weaknessPath;
    public string cardsPath;

    public TextAsset cardsAsset;
    List<GameCard> newDeck = new List<GameCard>();

    // Start is called before the first frame update
    void Start()
    {
        //DeckTestAndDrawingSys();
        //GenerateNewDeck();
    }
    /*
    private void GenerateNewDeck()
    {
        List<Card> newDeck = GameSys.GenerateDeck(5, cardsPath);
        for (int i = 0; i < newDeck.Count; i++)
        {
            Debug.Log(newDeck[i].Name);
        }
    }

    private void DeckTestAndDrawingSys()
    {
        newDeck = GameSys.GenerateDeckFullRandom(5, cardsPath);

        // Draw a card and say his name
        Debug.Log(GameSys.DrawCard(newDeck).Name);
        Debug.Log(GameSys.DrawCard(newDeck).Name);
        Debug.Log(GameSys.DrawCard(newDeck).Name);
    }

    private void NameAndDefineWeakness()
    {
        // DEFINE NAME
        PidieAi newAi = new PidieAi(namePath, 0, 1);
        Debug.Log(newAi.Name);

        // DEFINE WEAKNESS - 2 WEAKNESS
        newAi.weakness.Add(CsvFiles.GetEntryFromPath(weaknessPath, 0, 0));
        newAi.weakness.Add(CsvFiles.GetEntryFromPath(weaknessPath, 0, 1));

        // DISPLAY NAME + WEAKNESS
        Debug.Log(newAi.Name + " | WEAKNESS >> " + newAi.weakness[0] + " ; " + newAi.weakness[1]);
    }

    public void Stupid(int nbPlants)
    {
        int counter;
        counter = 0;

        for (int i = 0; i < nbPlants; i++)
        {
            counter++;
        }
        if (counter >= 8)
        {
            // Blabla
        }
    }*/
}
