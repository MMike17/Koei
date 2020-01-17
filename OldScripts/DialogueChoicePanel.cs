using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueChoicePanel : MonoBehaviour, IInitializable, IDebugable
{
    [Header("Assign in Inspector")]
    public TextMeshProUGUI dialogueText;
    public Button button;

    IInitializable initializableInterface => (IInitializable) this;

    public bool initialized { get { return initializableInterface.initializedInternal; } }
    public string debugLabel => "<b>[DialogueChoicePanel] : </b>";

    bool IInitializable.initializedInternal { get; set; }

    string text;
    int writingIndex;
    float charTimer, writingTimer;
    bool startWriting;

    public void Init(float charTimer)
    {
        initializableInterface.InitInternal();

        this.charTimer = charTimer;
    }

    void IInitializable.InitInternal()
    {
        initializableInterface.initializedInternal = true;

        startWriting = false;
        writingTimer = 0;
        writingIndex = 0;
    }

    void Update()
    {
        if(!initialized)
        {
            return;
        }

        if(startWriting)
        {
            writingTimer += Time.deltaTime;

            if(writingTimer >= charTimer)
            {
                writingTimer = 0;

                writingIndex++;
            }

            if(writingIndex >= text.Length)
            {
                startWriting = false;
            }

            dialogueText.text = text.Substring(0, writingIndex) + "<color=#00000000>" + text.Substring(writingIndex) + "</color>";
        }
    }

    public void StartWriting(string dialogue, Action callback)
    {
        text = dialogue;

        startWriting = true;

        writingTimer = 0;
        writingIndex = 0;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(callback.Invoke);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}