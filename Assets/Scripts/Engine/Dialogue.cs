using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Koei/Dialogue")]
public class Dialogue : ScriptableObject
{
    [Header("Assign in Inspector")]
    public string introLine;
    [Space]
    public List<PlayerChoice> playerChoices;
    [Space]
    public PlayerChoice endLine;

    public int choicesCountToShow { get { return playerChoices.Count; } }

    private void OnDrawGizmos()
    {
        Init();
    }

    public void Init()
    {
        DialogueIDFactory.Init();

        endLine.GenerateIndex();
        endLine.SetNextIndex(-1);

        playerChoices.ForEach(choice =>
        {
            choice.GenerateIndex();
            choice.SetNextIndex(choice.index + 1);
        });

        playerChoices[playerChoices.Count - 1].SetNextIndex(1);
    }
}

[Serializable]
public class PlayerChoice
{
    public int index { get; private set; }
    public int nextIndex { get; private set; }

    [TextArea]
    public string playerQuestion;
    [TextArea]
    public string shogunResponse;

    public void GenerateIndex()
    {
        index = DialogueIDFactory.GetIndex();
    }

    public void SetNextIndex(int index)
    {
        nextIndex = index;
    }
}

public static class DialogueIDFactory
{
    static int lastIndex;

    public static void Init()
    {
        lastIndex = 0;
    }

    public static int GetIndex()
    {
        int actualIndex = lastIndex;
        lastIndex++;

        return actualIndex;
    }
}