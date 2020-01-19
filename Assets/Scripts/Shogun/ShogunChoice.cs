using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// represents the buttons the player is going to click on to select an answer to the shogun on the Shogun panel
public class ShogunChoice : MonoBehaviour, IInitializable, IDebugable
{
    [Header("Assing in Inspector")]
    public TextMeshProUGUI choiceDialogue;
    public Button choiceButton;

    IDebugable debugableInterface => (IDebugable) this;
    IInitializable initializableInterface => (IInitializable) this;

    public bool initialized { get { return initializableInterface.initializedInternal; } }

    string IDebugable.debugLabel => "<b>[ShogunChoice] : </b>";
    bool IInitializable.initializedInternal { get; set; }

    Color highlightColor, hideColor, initialColor;
    string dialogueLine;
    float dialogueSpeed, dialogueTimer;
    int dialogueIndex, highlightLength;
    bool transitionDone;

    // retrieves a lot of data to adjust behaviour
    public void Init(string choiceLine, float choiceSpeed, Color initial, Color highlight, Color hide, int highlightLength, Action selected)
    {
        // resets behaviour
        dialogueIndex = 0;
        transitionDone = false;

        // sets line and speed
        dialogueLine = choiceLine;
        dialogueSpeed = choiceSpeed;

        // sets colors
        initialColor = initial;
        highlightColor = highlight;
        hideColor = hide;

        // sets highlight length
        this.highlightLength = highlightLength;

        // sets action to button
        choiceButton.onClick.RemoveAllListeners();
        choiceButton.onClick.AddListener(selected.Invoke);

        initializableInterface.InitInternal();

        PointerNotOver();
    }

    void IInitializable.InitInternal()
    {
        initializableInterface.initializedInternal = true;
    }

    // writes the line on the panel
    public void WriteLine()
    {
        // returns if not initialized
        if(!initialized)
        {
            Debug.LogError(debugableInterface.debugLabel + "Not initialized");
            return;
        }

        // shows the line in highlight if transition is done
        if(transitionDone)
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

        // highlights part of line that should be highlighted
        choiceDialogue.text = DialogueTools.HighlightString(dialogueLine, choiceDialogue.color, highlightColor, dialogueIndex, highlightLength);

        transitionDone = dialogueIndex > dialogueLine.Length - 1 + highlightLength;
    }

    // called when pointer is not over the panel
    public void PointerNotOver()
    {
        // returns if not initialized
        if(!initialized)
        {
            Debug.LogError(debugableInterface.debugLabel + "Not initialized");
            return;
        }

        choiceDialogue.text = dialogueLine;

        if(transitionDone)
        {
            // shows line in the initial color from the editor
            choiceDialogue.color = initialColor;
        }
        else
        {
            // hides line
            choiceDialogue.color = hideColor;
        }

        dialogueIndex = 0;
    }

    // resets the panel and hides it
    public void HidePanel()
    {
        gameObject.SetActive(false);

        initializableInterface.initializedInternal = false;
        transitionDone = false;
    }
}