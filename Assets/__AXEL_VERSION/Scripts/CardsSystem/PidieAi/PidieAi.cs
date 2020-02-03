using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Using simple namespaces
using CSVReader;

public class PidieAi
{
    public string Name { get; set; }
    public List<string> weakness = new List<string>();

    public PidieAi(string namePath, int nameRow, int nameColumn)
    {
        // Define name of character
        Name = CsvFiles.GetEntryFromPath(namePath, nameRow, nameColumn);
    }

    /// <summary>
    /// Allows AI to choose. Think = Make A Choice
    /// </summary>
    public static void Think()
    {

    }

    /// <summary>
    /// Allows AI to calculate. Imagine = Calculate
    /// </summary>
    public static void Imagine()
    {

    }

    /// <summary>
    /// Allows AI to draw a card, or several.
    /// </summary>
    /// <param name="cardNumberToDraw">Allows to select a number of cards to draw.</param>
    public static void DrawCard(int cardNumberToDraw)
    {

    }
}
