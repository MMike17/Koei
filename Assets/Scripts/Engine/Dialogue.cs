using System;
using System.Collections.Generic;
using UnityEngine;

// class representing Dialogues
[CreateAssetMenu(fileName = "Dialogue", menuName = "Koei/Dialogue")]
public class Dialogue : ScriptableObject
{
    [Header("Assign in Inspector")]
    // first line to appear on the panel
    public string introLine;
    [Space]
    public List<PlayerChoice> playerChoices;
    [Space]
    // choice that leads to quitting discussion
    public PlayerChoice endLine;

    // how many choices can we show (possible choices -1 + the end choice)
    public int choicesCountToShow { get { return playerChoices.Count; } }

    private void OnDrawGizmos()
    {
        // generate ID's in the inspector for checking
        Init();
    }

    public void Init()
    {
        // resets ID factory (and base index)
        DialogueIDFactory.Init();

        // sets next index as -1 to finish dialogue (detected later)
        endLine.GenerateIndex();
        endLine.SetNextIndex(-1);

        // links each choice to next choice
        playerChoices.ForEach(choice =>
        {
            choice.GenerateIndex();
            choice.SetNextIndex(choice.index + 1);
        });

        // sets last choice's next index to the first choice's index (makes a loop)
        playerChoices[playerChoices.Count - 1].SetNextIndex(playerChoices[0].index);
    }
}

[Serializable]
public class PlayerChoice
{
    // protects index and nextIndex so that they are not overwritten
    public int index { get; private set; }
    public int nextIndex { get; private set; }

    [TextArea]
    // line displayed in the selection bubble
    public string playerQuestion;
    [TextArea]
    // line that the shogun will respond with
    public string shogunResponse;

    // generates unique ID for player choice
    public void GenerateIndex()
    {
        index = DialogueIDFactory.GetIndex();
    }

    // sets next index
    public void SetNextIndex(int index)
    {
        nextIndex = index;
    }
}

// class used to generate unique ID's for PlayerChoice's of Dialogue objects
public static class DialogueIDFactory
{
    static int lastIndex;

    // resets indexes
    public static void Init()
    {
        lastIndex = 0;
    }

    // generates a new index
    public static int GetIndex()
    {
        int actualIndex = lastIndex;
        lastIndex++;

        return actualIndex;
    }
}