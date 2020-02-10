using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

// manager for game popups
public class PopupManager : MonoBehaviour, IDebugable, IInitializable
{
	[Header("Settings")]
	public float fadeDuration;
	public float alphaComparisonThreshold;

	[Header("Assign in Inspector")]
	public List<Popup> popups;

	[Header("Debug")]
	public GamePopup actualPopup;

	IDebugable debugableInterface => (IDebugable) this;
	IInitializable initializableInterface => (IInitializable) this;

	public bool initialized => initializableInterface.initializedInternal;

	string IDebugable.debugLabel => "<b>[PopupManager] : </b>";
	bool IInitializable.initializedInternal { get; set; }

	public void Init()
	{
		actualPopup = GamePopup.EMPTY;

		// initializes popups
		popups.ForEach(popup => { popup.Init(fadeDuration, alphaComparisonThreshold); popup.ForceState(false); });

		initializableInterface.InitInternal();

		Debug.Log(debugableInterface.debugLabel + "Initializing done");
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;
	}

	// pops corresponding popup and calls transition event
	public void Pop(GamePopup popup, Action onPopupDone)
	{
		if(!initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "Not initialized");
			return;
		}

		Popup newPopup = popups.Find(item => { return item.popup == popup; });

		if(newPopup == null)
		{
			Debug.LogError(debugableInterface.debugLabel + "Can't find the requested popup : " + popup.ToString());
			return;
		}

		newPopup.Activate(this, onPopupDone);

		actualPopup = popup;
	}

	// cancels all popups
	public void CancelPop()
	{
		if(!initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "Not initialized");
			return;
		}

		popups.ForEach(popup => popup.Deactivate(this));
	}

	// calls event when transition is done
	void CallEndTransitionEvent()
	{
		Popup selected = popups.Find(item => { return item.popup == actualPopup; });

		if(selected != null && selected.onPopup != null)
		{
			selected.onPopup.Invoke();
		}
	}

	[Serializable]
	public class Popup : IInitializable, IDebugable
	{
		public GamePopup popup;
		public CanvasGroup panel;
		public Action onPopup;

		IInitializable initializableInterface => (IInitializable) this;
		IDebugable debugableInterface => (IDebugable) this;

		public bool initialized => initializableInterface.initializedInternal;

		string IDebugable.debugLabel => "<b>[Popup] : </b>";
		bool IInitializable.initializedInternal { get; set; }

		float fadeDuration, alphaComparisonThreshold;

		public void Init(float duration, float threshold)
		{
			fadeDuration = duration;
			alphaComparisonThreshold = threshold;

			initializableInterface.InitInternal();

			Debug.Log(debugableInterface.debugLabel + "Initialized");
		}

		void IInitializable.InitInternal()
		{
			initializableInterface.initializedInternal = true;
		}

		// fades popup in and calls event at the end of the transition
		public void Activate(MonoBehaviour runner, Action onPopupDone)
		{
			if(!initialized)
			{
				Debug.LogError(debugableInterface.debugLabel + "Not initialized");
				return;
			}

			runner.StartCoroutine(Fade(false));
			runner.Invoke("CallEndTransitionEvent", fadeDuration);

			onPopup = onPopupDone;
		}

		// fades popup out
		public void Deactivate(MonoBehaviour runner)
		{
			if(!initialized)
			{
				Debug.LogError(debugableInterface.debugLabel + "Not initialized");
				return;
			}

			runner.StartCoroutine(Fade(true));
		}

		// forces state of the popup (use this for backend)
		public void ForceState(bool state)
		{
			if(!initialized)
			{
				Debug.LogError(debugableInterface.debugLabel + "Not initialized");
				return;
			}

			panel.alpha = 0;
			panel.blocksRaycasts = false;
			panel.interactable = false;
		}

		// main fade coroutine (fade in and out)
		IEnumerator Fade(bool fadeGameIn)
		{
			bool done = fadeGameIn ? panel.alpha <= alphaComparisonThreshold : panel.alpha >= 1 - alphaComparisonThreshold;

			while (!done)
			{
				float step = (1 / fadeDuration) * Time.deltaTime;
				step = fadeGameIn ? -step : step;

				done = fadeGameIn ? panel.alpha <= alphaComparisonThreshold : panel.alpha >= 1 - alphaComparisonThreshold;

				panel.blocksRaycasts = panel.alpha > 0;
				panel.interactable = panel.alpha >= 1;

				panel.alpha += step;

				if(done)
				{
					if(fadeGameIn)
					{
						panel.alpha = 0;
						panel.blocksRaycasts = false;
						panel.interactable = false;
					}
					else
					{
						panel.alpha = 1;
						panel.blocksRaycasts = true;
						panel.interactable = true;
					}

					yield break;
				}

				yield return null;
			}
		}
	}
}