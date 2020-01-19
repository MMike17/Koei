using System;
using System.Collections.Generic;
using UnityEngine;

// class managing events called when panels change (use to start up game phases)
public class EventsManager : MonoBehaviour, IDebugable
{
    public List<PhaseEvent> onPhaseTransitionEvents;
    public List<PopupEvent> onPopupEvents;

    IDebugable debugableInterface => (IDebugable) this;

    string IDebugable.debugLabel => "<b>[EventsManager] : </b>";

    // initializes both lists with GamePhases and GamePopups
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

        Debug.Log(debugableInterface.debugLabel + "Initializing done");
    }

    // ads an action to a phase transition
    public void AddPhaseAction(GameManager.GamePhase phase, Action callback)
    {
        PhaseEvent selected = onPhaseTransitionEvents.Find(item => { return item.phase == phase; });

        if(selected != null && callback != null)
        {
            selected.AddAction(callback);
        }
    }

    // ads an action to a popup transition
    public void AddPopupAction(GameManager.GamePopup popup, Action callback)
    {
        PopupEvent selected = onPopupEvents.Find(item => { return item.popup == popup; });

        if(selected != null && callback != null)
        {
            selected.AddAction(callback);
        }
    }

    // calls actions registered for a phase
    public void CallPhaseActions(GameManager.GamePhase phase)
    {
        PhaseEvent selected = onPhaseTransitionEvents.Find(item => { return item.phase == phase; });

        if(selected != null)
        {
            selected.CallActions();
        }
    }

    // calls actions registered for a popup
    public void CallPopupActions(GameManager.GamePopup popup)
    {
        PopupEvent selected = onPopupEvents.Find(item => { return item.popup == popup; });

        if(selected != null)
        {
            selected.CallActions();
        }
    }

    // class representing events for a given GamePhase
    public class PhaseEvent : IDebugable
    {
        public GameManager.GamePhase phase;
        Action callback;

        IDebugable debugableInterface => (IDebugable) this;

        string IDebugable.debugLabel => "<b>[PhaseEvent] : </b>";

        public PhaseEvent(GameManager.GamePhase phase)
        {
            this.phase = phase;
        }

        // adds action to event
        public void AddAction(Action callback)
        {
            if(callback == null)
            {
                Debug.LogError(debugableInterface.debugLabel + "Callback is empty");
                return;
            }

            this.callback += callback;
        }

        // calls event action
        public void CallActions()
        {
            if(callback != null)
            {
                callback.Invoke();
            }
        }
    }

    // class representing events for a giver GamePopup
    public class PopupEvent : IDebugable
    {
        public GameManager.GamePopup popup;
        Action callback;

        IDebugable debugableInterface => (IDebugable) this;

        string IDebugable.debugLabel => "<b>[PhaseEvent] : </b>";

        public PopupEvent(GameManager.GamePopup popup)
        {
            this.popup = popup;
        }

        // adds action to event
        public void AddAction(Action callback)
        {
            if(callback == null)
            {
                Debug.LogError(debugableInterface.debugLabel + "Callback is empty");
                return;
            }

            this.callback += callback;
        }

        // calls event action
        public void CallActions()
        {
            if(callback != null)
            {
                callback.Invoke();
            }
        }
    }
}