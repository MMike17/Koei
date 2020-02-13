using System;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

// manager for game popups
public class PopupManager : MonoBehaviour, IDebugable, IInitializable
{
	[Header("Settings")]
	public float fadeDuration;
	public float alphaComparisonThreshold;

	[Header("Debug")]
	public GamePopup actualPopup;

	IDebugable debugableInterface => (IDebugable) this;
	IInitializable initializableInterface => (IInitializable) this;

	public bool initialized => initializableInterface.initializedInternal;

	string IDebugable.debugLabel => "<b>[PopupManager] : </b>";
	bool IInitializable.initializedInternal { get; set; }

	List<PopupEventSubscription> popupsSubscriptions;
	// list gets updated at each scene opening
	List<Popup> scenePopups;

	public void Init()
	{
		actualPopup = GamePopup.EMPTY;

		popupsSubscriptions = new List<PopupEventSubscription>();

		// init all event subscriptions (we can then subscribe events)
		foreach (GamePopup popup in Enum.GetValues(typeof(GamePopup)))
		{
			popupsSubscriptions.Add(new PopupEventSubscription(popup));
		}

		initializableInterface.InitInternal();
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;

		Debug.Log(debugableInterface.debugLabel + "Initializing done");
	}

	// pops corresponding popup and calls transition event
	public void Pop(GamePopup popup)
	{
		if(!initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "Not initialized");
			return;
		}

		Type type = GetTypeFromPopup(popup);

		if(type == null)
		{
			Debug.LogError(debugableInterface.debugLabel + "Couln't find type for GamePopup " + popup.ToString());
			return;
		}
		else
		{
			Popup selected = scenePopups.Find(item => { return item.GetType() == type; });

			if(selected == null)
			{
				Debug.LogError(debugableInterface.debugLabel + "Couldn't find Popup for GamePopup " + popup.ToString());
				return;
			}
			else
			{
				selected.Activate(this);
				actualPopup = popup;
			}
		}
	}

	// cancels all popups
	public void CancelPop()
	{
		if(!initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "Not initialized");
			return;
		}

		scenePopups.ForEach(popup => popup.Deactivate(this));
	}

	// force cancels all popups
	public void ForceCancelPop()
	{
		if(!initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "Not initialized");
			return;
		}

		scenePopups.ForEach(popup => popup.ForceState(false));
	}

	public void SubscribeEvent(GamePopup popup, Action callback)
	{
		if(!initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "Not initialized");
			return;
		}

		PopupEventSubscription eventSubscription = GetSubscriptionEvent(popup);

		if(callback != null)
			eventSubscription.AddEvent(callback);
	}

	// call this when you get on a new scene (like an Init but every time you change scenes)
	public void GetAllScenePopups()
	{
		scenePopups = new List<Popup>();
		scenePopups.AddRange(Resources.FindObjectsOfTypeAll<Popup>());

		scenePopups.ForEach(item =>
		{
			item.Init(
				fadeDuration, alphaComparisonThreshold,
				() => { actualPopup = item.popup; },
				() => GetSubscriptionEvent(item.popup).CallEvent()
			);
		});
	}

	PopupEventSubscription GetSubscriptionEvent(GamePopup popup)
	{
		PopupEventSubscription popupEvent = popupsSubscriptions.Find(item => { return item.popup == popup; });

		if(popupEvent == null)
		{
			Debug.LogWarning(debugableInterface.debugLabel + "Couldn't find PopupEventSubscription for popup " + popup.ToString());
			return null;
		}

		return popupEvent;
	}

	Type GetTypeFromPopup(GamePopup popup)
	{
		switch(popup)
		{
			case GamePopup.EMPTY:
				return null;
			case GamePopup.SETTINGS:
				return null;
			case GamePopup.SHOGUN_DEDUCTION:
				return typeof(ShogunPopup);
		}

		return null;
	}

	// used by game manager to Init specific popup
	public T GetPopupFromType<T>() where T : MonoBehaviour
	{
		foreach (Popup popup in scenePopups)
		{
			if(popup.GetType() == typeof(T))
			{
				return popup.GetComponent<T>();
			}
		}

		Debug.LogWarning(debugableInterface.debugLabel + "Couldn't find popup for type " + typeof(T).ToString());
		return null;
	}

	public class PopupEventSubscription
	{
		public GamePopup popup;
		public Action transitionEvent;

		public PopupEventSubscription(GamePopup popup)
		{
			this.popup = popup;
		}

		public void AddEvent(Action callback)
		{
			if(callback != null)
				transitionEvent += callback;
		}

		public void CallEvent()
		{
			transitionEvent.Invoke();
		}
	}
}