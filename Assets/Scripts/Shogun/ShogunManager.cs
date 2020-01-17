using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShogunManager : MonoBehaviour, IDebugable
{
    [Header("Settings")]
    public float shogunDialogueSpeed;
    public float choiceDialogueSpeed;
    public int highlightLength;
    public Color textColor, highlightColor, hideColor;

    [Header("Assing in Inspector")]
    public List<ShogunChoice> choices;
    public EventSystem eventSystem;
    public TextMeshProUGUI shogunDialogue;
    public Button quitPopupQuitButton, quitPopupResumeButton;

    public string debugLabel => "<b>[ShogunManager] : </b>";

    Dialogue selectedDialogue;
    Action endDialogue;
    string shogunLine;
    float shogunDialogueTimer;
    int shogunIndex, actualChoice;

    void Awake()
    {
        HideChoices();
    }

    public void Init(Action quitButtonCallback, Action resumeButtonCallback, Action endDialogueCallback)
    {
        endDialogue = endDialogueCallback;

        quitPopupQuitButton.onClick.RemoveAllListeners();
        quitPopupQuitButton.onClick.AddListener(() =>
        {
            if(quitButtonCallback != null)
                quitButtonCallback.Invoke();
        });

        quitPopupResumeButton.onClick.RemoveAllListeners();
        quitPopupResumeButton.onClick.AddListener(() =>
        {
            if(resumeButtonCallback != null)
                resumeButtonCallback.Invoke();
        });

        Debug.Log(debugLabel + "Initializing done");
    }

    void Update()
    {
        if(selectedDialogue == null)
            return;

        if(eventSystem.currentSelectedGameObject != null)
            eventSystem.SetSelectedGameObject(null);

        bool shogunIsDone = ShogunLine();

        if(shogunIsDone)
            ShowChoices(selectedDialogue.choicesCountToShow);
        else
            HideChoices();

        if(eventSystem.IsPointerOverGameObject() && ActuallyUsefulInputModule.Get != null)
        {
            choices.ForEach(choice =>
            {
                bool needsHiding = true;

                ActuallyUsefulInputModule.GetPointerEventData().hovered.ForEach(item =>
                {
                    ShogunChoice other = item.GetComponent<ShogunChoice>();

                    if(other != null && other == choice && other.initialized)
                    {
                        item.GetComponent<ShogunChoice>().WriteDialogue();
                        needsHiding = false;
                    }
                });

                if(choice.initialized && needsHiding)
                    choice.HideDialogue();
            });
        }
    }

    bool ShogunLine()
    {
        if(shogunIndex >= shogunLine.Length + highlightLength)
            return true;

        shogunDialogueTimer += Time.deltaTime;

        if(shogunDialogueTimer >= 1 / shogunDialogueSpeed)
        {
            shogunDialogueTimer = 0;
            shogunIndex++;
        }

        shogunDialogue.text = DialogueTools.HighlightString(shogunLine, textColor, highlightColor, shogunIndex, highlightLength);

        return false;
    }

    void HideChoices()
    {
        choices.ForEach(item => item.Reset());
    }

    void ShowChoices(int howMany)
    {
        int howManyActive = 0;

        choices.ForEach(item =>
        {
            if(item.initialized)
                howManyActive++;
        });

        if(howManyActive >= howMany)
            return;

        for (int i = 0; i < choices.Count; i++)
        {
            choices[i].gameObject.SetActive(i < howMany);

            if(i < howMany)
                choices[i].Init(selectedDialogue.playerChoices[actualChoice].playerQuestion, choiceDialogueSpeed, textColor, highlightColor, hideColor, highlightLength, () => SelectChoice(selectedDialogue.playerChoices[actualChoice].nextIndex));
        }
    }

    void SelectChoice(int selected)
    {
        if(selected <= -1)
        {
            if(endDialogue == null)
            {
                Debug.LogError(debugLabel + "end dialogue event should not be null");
                return;
            }
            else
                endDialogue.Invoke();
        }
        else
        {
            ResetShogun(selectedDialogue.playerChoices[actualChoice].shogunResponse);

            actualChoice = selected;
        }
    }

    void ResetShogun(string line)
    {
        shogunDialogueTimer = 0;
        shogunIndex = 0;

        shogunLine = line;
    }

    public void StartDialogue(Dialogue dialogue)
    {
        shogunDialogue.color = textColor;
        selectedDialogue = dialogue;
        ResetShogun(dialogue.introLine);
    }

    void QuitDialogue()
    {
        selectedDialogue = null;
    }
}