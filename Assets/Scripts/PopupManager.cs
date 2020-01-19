using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// manager for game popups
public class PopupManager : MonoBehaviour, IDebugable
{
    [Header("Settings")]
    public float fadeDuration;

    [Header("Assign in Inspector")]
    public List<Popup> popups;

    [Header("Debug")]
    public GameManager.GamePopup actualPopup;

    IDebugable debugableInterface => (IDebugable) this;

    string IDebugable.debugLabel => "<b>[PopupManager] : </b>";

    public void Init()
    {
        actualPopup = GameManager.GamePopup.EMPTY;

        // initializes popups
        popups.ForEach(popup => { popup.Init(fadeDuration); popup.ForceState(false); });

        Debug.Log(debugableInterface.debugLabel + "Initializing done");
    }

    // pops corresponding popup and calls transition event
    public void Pop(GameManager.GamePopup popup, Action callback)
    {
        Popup newPopup = popups.Find(item => { return item.popup == popup; });

        if(newPopup == null)
        {
            Debug.LogError(debugableInterface.debugLabel + "Can't find the requested panel : " + popup.ToString());
            return;
        }

        newPopup.Activate(this, callback);

        actualPopup = popup;
    }

    // cancels all popups
    public void CancelPop()
    {
        popups.ForEach(popup => popup.Deactivate(this));
    }

    [Serializable]
    public class Popup
    {
        public GameManager.GamePopup popup;
        public CanvasGroup panel;

        float fadeDuration;
        Action onPopup;

        public void Init(float duration)
        {
            fadeDuration = duration;
        }

        // fades popup in and calls event at the end of the transition
        public void Activate(MonoBehaviour runner, Action callback)
        {
            runner.StartCoroutine("Fade", false);
            runner.Invoke("CallEndTransitionEvent", fadeDuration);

            onPopup = callback;
        }

        // fades popup out
        public void Deactivate(MonoBehaviour runner)
        {
            runner.StartCoroutine("Fade", true);
        }

        // forces state of the popup (use this for backend)
        public void ForceState(bool state)
        {
            panel.gameObject.SetActive(state);
        }

        // main fade coroutine (fade in and out)
        IEnumerator Fade(bool fadeGameIn)
        {
            float step = 1 / fadeDuration * Time.deltaTime;
            step = fadeGameIn ? -step : step;

            panel.alpha += step;

            // TODO : transform this in loop probably

            if(fadeGameIn)
            {
                panel.gameObject.SetActive(panel.alpha > 0);

                if(panel.alpha <= 0)
                {
                    yield break;
                }
            }
            else
            {
                panel.gameObject.SetActive(true);

                if(panel.alpha >= 1)
                {
                    yield break;
                }
            }
        }

        // calls event when transition is done
        void CallEndTransitionEvent()
        {
            if(onPopup != null)
                onPopup.Invoke();
        }
    }
}