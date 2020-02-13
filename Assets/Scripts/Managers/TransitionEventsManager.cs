using System;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

// class managing events called when panels change (use to start up game phases)
public class TransitionEventsManager : MonoBehaviour, IDebugable, IInitializable
{
	public List<PhaseEvent> onPhaseTransitionEvents;

	public bool initialized => initializableInterface.initializedInternal;

	IDebugable debugableInterface => (IDebugable) this;
	IInitializable initializableInterface => (IInitializable) this;

	string IDebugable.debugLabel => "<b>[EventsManager] : </b>";
	bool IInitializable.initializedInternal { get; set; }

	// initializes both lists with GamePhases and GamePopups
	public void Init()
	{
		onPhaseTransitionEvents = new List<PhaseEvent>();

		foreach (GamePhase phase in Enum.GetValues(typeof(GamePhase)))
		{
			onPhaseTransitionEvents.Add(new PhaseEvent(phase));
		}

		initializableInterface.InitInternal();

		Debug.Log(debugableInterface.debugLabel + "Initializing done");
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;
	}

	// ads an action to a phase transition
	public void AddPhaseAction(GamePhase phase, Action callback)
	{
		if(!initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "Not initialized");
			return;
		}

		PhaseEvent selected = onPhaseTransitionEvents.Find(item => { return item.phase == phase; });

		if(selected != null && callback != null)
		{
			selected.AddAction(callback);
		}
	}

	// calls actions registered for a phase
	public void CallPhaseActions(GamePhase phase)
	{
		if(!initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "Not initialized");
			return;
		}

		PhaseEvent selected = onPhaseTransitionEvents.Find(item => { return item.phase == phase; });

		if(selected != null)
		{
			selected.CallActions();
			Debug.Log(debugableInterface.debugLabel + "Called transition method");
		}
		else
		{
			Debug.Log(debugableInterface.debugLabel + "There was no transition method");
		}
	}

	// class representing events for a given GamePhase
	public class PhaseEvent : IDebugable
	{
		public GamePhase phase;
		Action callback;

		IDebugable debugableInterface => (IDebugable) this;

		string IDebugable.debugLabel => "<b>[PhaseEvent] : </b>";

		public PhaseEvent(GamePhase phase)
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
}