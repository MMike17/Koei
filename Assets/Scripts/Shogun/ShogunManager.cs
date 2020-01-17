using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// class managing gameplay of Shogun panel
public class ShogunManager : MonoBehaviour, IDebugable // may add Iinitializable interface
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

    IDebugable debugableInterface => (IDebugable) this;
    string IDebugable.debugLabel => "<b>[ShogunManager] : </b>";

    Dialogue selectedDialogue;
    Action endDialogue;
    string shogunLine;
    float shogunDialogueTimer;
    int shogunIndex, actualChoice;

    void Awake()
    {
        HideChoices();
    }

    // receives actions from GameManager
    public void Init(Action quitButtonCallback, Action resumeButtonCallback, Action endDialogueCallback)
    {
        // action to call when player selects choice that ends dialogue
        endDialogue = endDialogueCallback;

        // plugs in actions for the popup buttons
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

        Debug.Log(debugableInterface.debugLabel + "Initializing done");
    }

    void Update()
    {
        // doesn't do anything if Dialogue object is not set
        if(selectedDialogue == null)
        {
            return;
        }

        // enables button to skip the "selected" stage (usefull only for consoles)
        if(eventSystem.currentSelectedGameObject != null)
        {
            eventSystem.SetSelectedGameObject(null);
        }

        bool shogunIsDone = ShogunLine();

        if(shogunIsDone) // shows choices panels if shogun is done talking
        {
            ShowChoices(selectedDialogue.choicesCountToShow);
        }
        else // hides choices panels while shogun is talking
        {
            HideChoices();
        }

        // if the mouse is over an object and input module exists
        if(eventSystem.IsPointerOverGameObject() && ActuallyUsefulInputModule.Get != null)
        {
            choices.ForEach(choice =>
            {
                bool needsHiding = true;

                // loops through all hovered objects
                ActuallyUsefulInputModule.GetPointerEventData().hovered.ForEach(item =>
                {
                    ShogunChoice other = item.GetComponent<ShogunChoice>();

                    if(other != null && other == choice)
                    {
                        // tells panel to write line if there is one and it's selected
                        item.GetComponent<ShogunChoice>().WriteLine();
                        needsHiding = false;
                    }
                });

                // hides choice if not selected
                if(needsHiding)
                {
                    choice.PointerNotOver();
                }
            });
        }
    }

    // writes the shogun line with highlight
    bool ShogunLine()
    {
        // returns true if the line is displayed fully
        if(shogunIndex >= shogunLine.Length + highlightLength)
        {
            return true;
        }

        shogunDialogueTimer += Time.deltaTime;

        if(shogunDialogueTimer >= 1 / shogunDialogueSpeed)
        {
            shogunDialogueTimer = 0;
            shogunIndex++;
        }

        // displays highlighted shogun line
        shogunDialogue.text = DialogueTools.HighlightString(shogunLine, textColor, highlightColor, shogunIndex, highlightLength);

        return false;
    }

    // hides choices panels
    void HideChoices()
    {
        choices.ForEach(item => item.HidePanel());
    }

    // shows choices depending on how many there are
    void ShowChoices(int howMany)
    {
        int howManyActive = 0;

        // activates desired choices panels
        choices.ForEach(item =>
        {
            if(item.initialized)
            {
                howManyActive++;
            }
        });

        // if choices panels are already active
        if(howManyActive >= howMany)
        {
            return;
        }

        // initializes choices panels with necessary infos
        for (int i = 0; i < choices.Count; i++)
        {
            choices[i].gameObject.SetActive(i < howMany);

            if(i < howMany)
            {
                choices[i].Init(selectedDialogue.playerChoices[actualChoice].playerQuestion, choiceDialogueSpeed, textColor, highlightColor, hideColor, highlightLength, () => SelectChoice(selectedDialogue.playerChoices[actualChoice].nextIndex));
            }
        }
    }

    // called when the player clicks on a choice
    void SelectChoice(int selected)
    {
        if(selected <= -1) // if the player clicked on the "end conversation" choice
        {
            if(endDialogue == null)
            {
                Debug.LogError(debugableInterface.debugLabel + "end dialogue event should not be null");
                return;
            }
            else
            {
                QuitDialogue();
            }
        }
        else
        {
            // shows shogun answer and goes to next choice
            ResetShogun(selectedDialogue.playerChoices[actualChoice].shogunResponse);

            actualChoice = selected;
        }
    }

    // resets shogun line and signals for new shogun line
    void ResetShogun(string line)
    {
        shogunDialogueTimer = 0;
        shogunIndex = 0;

        shogunLine = line;
    }

    // called by GameManager to start dialogue
    public void StartDialogue(Dialogue dialogue)
    {
        shogunDialogue.color = textColor;
        selectedDialogue = dialogue;
        ResetShogun(dialogue.introLine);
    }

    // resets component for next phase
    void QuitDialogue()
    {
        selectedDialogue = null;
        endDialogue.Invoke();
    }
}