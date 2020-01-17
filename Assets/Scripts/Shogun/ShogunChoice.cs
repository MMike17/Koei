using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShogunChoice : MonoBehaviour, IInitializable, IDebugable
{
    [Header("Assing in Inspector")]
    public TextMeshProUGUI choiceDialogue;
    public Button choiceButton;

    IInitializable initializableInterface => (IInitializable) this;

    public bool initialized { get { return initializableInterface.initializedInternal; } }
    public string debugLabel => "<b>[ShogunChoice] : </b>";

    bool IInitializable.initializedInternal { get; set; }

    Color highlightColor, hideColor, initialColor;
    string dialogueLine;
    float dialogueSpeed, dialogueTimer;
    int dialogueIndex, highlightLength;
    bool done;

    public void Init(string choiceLine, float choiceSpeed, Color initial, Color highlight, Color hide, int highlightLength, Action selected)
    {
        initializableInterface.InitInternal();

        dialogueLine = choiceLine;
        dialogueSpeed = choiceSpeed;

        initialColor = initial;
        highlightColor = highlight;
        hideColor = hide;

        this.highlightLength = highlightLength;

        choiceButton.onClick.RemoveAllListeners();
        choiceButton.onClick.AddListener(selected.Invoke);

        HideDialogue();
    }

    void IInitializable.InitInternal()
    {
        initializableInterface.initializedInternal = true;

        dialogueIndex = 0;
        done = false;
    }

    public void WriteDialogue()
    {
        if(!initialized)
        {
            Debug.LogError(debugLabel + "Not initialized");
            return;
        }

        if(done)
        {
            choiceDialogue.color = highlightColor;
            choiceDialogue.text = dialogueLine;
            return;
        }

        choiceDialogue.color = initialColor;
        dialogueTimer += Time.deltaTime;

        if(dialogueTimer >= 1 / dialogueSpeed)
        {
            dialogueTimer = 0;
            dialogueIndex++;
        }

        choiceDialogue.text = DialogueTools.HighlightString(dialogueLine, choiceDialogue.color, highlightColor, dialogueIndex, highlightLength);

        done = dialogueIndex > dialogueLine.Length - 1 + highlightLength;
    }

    public void HideDialogue()
    {
        if(!initialized)
        {
            Debug.LogError(debugLabel + "Not initialized");
            return;
        }

        choiceDialogue.text = dialogueLine;

        if(done)
            choiceDialogue.color = initialColor;
        else
            choiceDialogue.color = hideColor;

        dialogueIndex = 0;
    }

    public void Reset()
    {
        gameObject.SetActive(false);

        initializableInterface.initializedInternal = false;
        done = false;
    }
}