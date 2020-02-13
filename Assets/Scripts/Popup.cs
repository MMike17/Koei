using System;
using System.Collections;
using UnityEngine;
using static GameManager;

public class Popup : MonoBehaviour, IInitializable, IDebugable
{
	public GamePopup popup;
	public CanvasGroup panel;

	internal IInitializable initializableInterface => (IInitializable) this;
	internal IDebugable debugableInterface => (IDebugable) this;

	public bool initialized => initializableInterface.initializedInternal;

	string IDebugable.debugLabel => "<b>[Popup] : </b>";
	bool IInitializable.initializedInternal { get; set; }

	Action popSignal, onPopup;
	float fadeDuration, alphaComparisonThreshold;

	public virtual void Init(float duration, float threshold, Action signal, Action onPopupDone)
	{
		fadeDuration = duration;
		alphaComparisonThreshold = threshold;

		popSignal = signal;
		onPopup = onPopupDone;

		initializableInterface.InitInternal();
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;

		Debug.Log(debugableInterface.debugLabel + "Initilizing done");
	}

	// fades popup in and calls event at the end of the transition
	public void Activate(MonoBehaviour runner)
	{
		if(!initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "Not initialized");
			return;
		}

		if(popSignal == null)
		{
			Debug.LogError(debugableInterface.debugLabel + "Signal callback shouldn't be empty or null. This will prevent popup system from working properly");
			return;
		}

		popSignal.Invoke();
		runner.StartCoroutine(Fade(false));
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
		enabled = false;
	}

	// forces state of the popup (use this for backend)
	public void ForceState(bool state)
	{
		if(!initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "Not initialized");
			return;
		}

		panel.alpha = state? 1 : 0;
		panel.blocksRaycasts = state;
		panel.interactable = state;
		enabled = state;

		ResetPopup();
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

					ResetPopup();
				}
				else
				{
					panel.alpha = 1;
					panel.blocksRaycasts = true;
					panel.interactable = true;

					CallEndTransitionEvent();
					enabled = true;
				}

				yield break;
			}

			yield return null;
		}
	}

	// calls event when transition is done
	void CallEndTransitionEvent()
	{
		if(onPopup == null)
		{
			Debug.Log(debugableInterface.debugLabel + "No transition event was found");
			return;
		}

		onPopup.Invoke();
	}

	// gets overwritten by child class
	public virtual void ResetPopup()
	{
		Debug.Log(debugableInterface.debugLabel + "Reseted popup content");
	}
}