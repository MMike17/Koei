using System;
using System.Collections.Generic;
using UnityEngine;

public class EventsManager : MonoBehaviour, IDebugable
{
    public List<PhaseEvent> onPhaseTransitionEvents;
    public List<PopupEvent> onPopupEvents;

    IDebugable debugableInterface => (IDebugable) this;

    public string debugLabel => "<b>[EventsManager] : </b>";

    public void Init()
    {
        onPhaseTransitionEvents = new List<PhaseEvent>();

        foreach (GameManager.GamePhase phase in Enum.GetValues(typeof(GameManager.GamePhase)))
        {
            onPhaseTransitionEvents.Add(new PhaseEvent(phase));
        }

        onPopupEvents = new List<PopupEvent>();

        foreach (GameManager.GamePopup popup in Enum.GetValues(typeof(GameManager.GamePopup)))
        {
            onPopupEvents.Add(new PopupEvent(popup));
        }

        Debug.Log(debugLabel + "Initializing done");
    }

    public void AddPhaseAction(GameManager.GamePhase phase, Action callback)
    {
        PhaseEvent selected = onPhaseTransitionEvents.Find(item => { return item.phase == phase; });

        if(selected != null && callback != null)
            selected.AddAction(callback);
    }

    public void AddPopupAction(GameManager.GamePopup popup, Action callback)
    {
        PopupEvent selected = onPopupEvents.Find(item => { return item.popup == popup; });

        if(selected != null && callback != null)
            selected.AddAction(callback);
    }

    public void CallPhaseActions(GameManager.GamePhase phase)
    {
        PhaseEvent selected = onPhaseTransitionEvents.Find(item => { return item.phase == phase; });

        if(selected != null)
            selected.CallActions();
    }

    public void CallPopupActions(GameManager.GamePopup popup)
    {
        PopupEvent selected = onPopupEvents.Find(item => { return item.popup == popup; });

        if(selected != null)
            selected.CallActions();
    }

    public class PhaseEvent : IDebugable
    {
        public GameManager.GamePhase phase;
        Action callback;

        IDebugable debugableInterface => (IDebugable) this;

        public string debugLabel => "<b>[PhaseEvent] : </b>";

        public PhaseEvent(GameManager.GamePhase phase)
        {
            this.phase = phase;
        }

        public void AddAction(Action callback)
        {
            if(callback == null)
            {
                Debug.LogError(debugLabel + "Callback is empty");
                return;
            }

            this.callback += callback;
        }

        public void CallActions()
        {
            if(callback != null)
                callback.Invoke();
        }
    }

    public class PopupEvent : IDebugable
    {
        public GameManager.GamePopup popup;
        Action callback;

        IDebugable debugableInterface => (IDebugable) this;

        public string debugLabel => "<b>[PhaseEvent] : </b>";

        public PopupEvent(GameManager.GamePopup popup)
        {
            this.popup = popup;
        }

        public void AddAction(Action callback)
        {
            if(callback == null)
            {
                Debug.LogError(debugLabel + "Callback is empty");
                return;
            }

            this.callback += callback;
        }

        public void CallActions()
        {
            if(callback != null)
                callback.Invoke();
        }
    }
}