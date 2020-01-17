using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour, IDebugable
{
    [Header("Settings")]
    public float fadeDuration;
    public float spinnerSpeed, comparisonThreshold;

    [Header("Assign in Inspector")]
    public List<GamePanel> panels;
    [Space]
    public Transform fadeSpinner;
    public CanvasGroup fadePanel;

    [Header("Debug")]
    public GameManager.GamePhase actualPanel;
    public GameManager.GamePhase nextPanel;

    IDebugable debugableInterface => (IDebugable) this;

    public string debugLabel => "<b>[PanelsManager] : </b>";

    Action onEndTransition;

    public void Init()
    {
        actualPanel = GameManager.GamePhase.TITLE;

        panels.ForEach(panel => panel.Deactivate());
        panels.Find(panel => { return panel.phase == GameManager.GamePhase.TITLE; }).Activate();

        Debug.Log(debugLabel + "Initializing done");
    }

    public void JumpTo(GameManager.GamePhase newPanel, Action callback)
    {
        StartCoroutine("Fade", false);
        Invoke("FinishFade", fadeDuration);

        nextPanel = newPanel;
        onEndTransition = callback;
    }

    void Update()
    {
        if(fadePanel.alpha > 0)
            fadeSpinner.Rotate(0, 0, -spinnerSpeed * Time.deltaTime);
    }

    void FinishFade()
    {
        GamePanel oldPanel = panels.Find((item) => { return item.phase == actualPanel; });
        GamePanel newPanel = panels.Find((item) => { return item.phase == nextPanel; });

        if(oldPanel == null)
        {
            Debug.LogError(debugLabel + "There are no currently activated panel");
            return;
        }

        oldPanel.Deactivate();

        if(newPanel == null)
        {
            Debug.LogError(debugLabel + "Can't find the requested panel : " + nextPanel.ToString());
            return;
        }

        newPanel.Activate();

        Debug.Log(debugLabel + "Switched from " + actualPanel.ToString() + " to " + nextPanel.ToString());

        actualPanel = nextPanel;

        StartCoroutine("Fade", true);
        Invoke("CallEndTransitionEvent", fadeDuration);
    }

    IEnumerator Fade(bool fadeGameIn)
    {
        bool condition = fadeGameIn ? fadePanel.alpha <= 0 + comparisonThreshold : fadePanel.alpha >= 1 - comparisonThreshold;

        while (!condition)
        {
            float step = (1 / fadeDuration) * Time.deltaTime;
            step = fadeGameIn ? -step : step;

            condition = fadeGameIn ? fadePanel.alpha <= 0 + comparisonThreshold : fadePanel.alpha >= 1 - comparisonThreshold;

            fadePanel.gameObject.SetActive(fadePanel.alpha > 0 + comparisonThreshold);

            fadePanel.alpha += step;

            if(condition)
                yield break;

            yield return null;
        }
    }

    void CallEndTransitionEvent()
    {
        if(onEndTransition != null)
            onEndTransition.Invoke();
    }

    [Serializable]
    public class GamePanel
    {
        public GameManager.GamePhase phase;
        public GameObject panel;

        public void Deactivate()
        {
            panel.SetActive(false);
        }

        public void Activate()
        {
            panel.SetActive(true);
        }
    }
}