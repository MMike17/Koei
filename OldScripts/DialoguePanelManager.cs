using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialoguePanelManager : MonoBehaviour, IDebugable
{
    [Header("Settings")]
    public float timePerChar;

    [Header("Assign in Inspector")]
    public TextMeshProUGUI enemyLine;
    public TextMeshProUGUI enemyName;
    public List<DialogueChoicePanel> panels;

    IDebugable debuguableInterface => (IDebugable) this;
    string IDebugable.debugLabel => "<b>[DialoguePanelManager] : </b>";

    void Awake()
    {

    }

    public void InitAll()
    {
        panels.ForEach((item) =>
        {
            item.Init(timePerChar);
        });
    }

    public void AssignLines(Phase actualPhase)
    {
        bool notInitialized = false;

        panels.ForEach((item) =>
        {
            if(!item.initialized)
            {
                Debug.LogError(item.debugLabel + "not initialized", item.gameObject);
                notInitialized = true;
            }
        });

        if(notInitialized)
            return;

        if(actualPhase.playerChoices.Length > panels.Count)
        {
            Debug.LogError(debuguableInterface.debugLabel + "there are more dialogue lines than panels to display them (" + panels.Count + " / " + actualPhase.playerChoices.Length + ")");

            return;
        }

        enemyLine.text = actualPhase.triggerLine;

        for (int i = 0; i < panels.Count; i++)
        {
            if(i >= actualPhase.playerChoices.Length)
            {
                panels[i].Hide();
            }
            else
            {
                int j = i;

                panels[i].Show();
                panels[i].StartWriting(actualPhase.playerChoices[i].line, () => { AddValue(actualPhase.playerChoices[j].choiceValue); });
            }
        }
    }

    void AddValue(int value)
    {
        FightManager.Get.playerValue += value;
        FightManager.Get.NextFight();
    }
}