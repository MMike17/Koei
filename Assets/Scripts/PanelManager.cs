using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

// manager for game panels
public class PanelManager : MonoBehaviour, IDebugable, IInitializable
{
	[Header("Settings")]
	public float fadeDuration;
	public float spinnerSpeed, alphaComparisonThreshold;

	[Header("Assign in Inspector")]
	public List<GamePanel> panels;
	[Space]
	public Transform fadeSpinner;
	public CanvasGroup fadePanel;

	[Header("Debug")]
	public GamePhase actualPanel;
	public GamePhase nextPanel;

	public bool initialized => initializableInterface.initializedInternal;

	IDebugable debugableInterface => (IDebugable) this;
	IInitializable initializableInterface => (IInitializable) this;

	string IDebugable.debugLabel => "<b>[PanelsManager] : </b>";
	bool IInitializable.initializedInternal { get; set; }

	Action onEndTransition;
	bool inTransition;

	public void Init()
	{
		actualPanel = GamePhase.TITLE;

		// deactivates all panels except TITLE panel that is activated
		panels.ForEach(panel =>
		{
			if(panel.phase == GamePhase.TITLE)
			{
				panel.Activate();
			}
			else
			{
				panel.Deactivate();
			}
		});

		initializableInterface.InitInternal();

		Debug.Log(debugableInterface.debugLabel + "Initializing done");
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;
	}

	// jumps to corresponding panel and calls transition event
	public void JumpTo(GamePhase newPanel, Action callback)
	{
		if(!initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "Not initialized");
			return;
		}

		if(inTransition)
		{
			Debug.LogError(debugableInterface.debugLabel + "Can't transition to " + newPanel.ToString() + " because panels are already transitionning");
			return;
		}

		inTransition = true;

		// starts fade coroutine
		StartCoroutine(Fade(false));
		// calls sencond fade coroutine at the end of the first
		Invoke("FinishFade", fadeDuration);

		nextPanel = newPanel;
		onEndTransition = callback;
	}

	void Update()
	{
		// spins spinner if vivisble
		if(fadePanel.alpha > 0)
			fadeSpinner.Rotate(0, 0, -spinnerSpeed * Time.deltaTime);
	}

	// changes panels and starts second fade
	void FinishFade()
	{
		GamePanel oldPanel = panels.Find((item) => { return item.phase == actualPanel; });
		GamePanel newPanel = panels.Find((item) => { return item.phase == nextPanel; });

		if(oldPanel == null)
		{
			Debug.LogError(debugableInterface.debugLabel + "There are no currently activated panel");
			return;
		}

		oldPanel.Deactivate();

		if(newPanel == null)
		{
			Debug.LogError(debugableInterface.debugLabel + "Can't find the requested panel : " + nextPanel.ToString());
			return;
		}

		newPanel.Activate();

		Debug.Log(debugableInterface.debugLabel + "Switched from " + actualPanel.ToString() + " to " + nextPanel.ToString());

		actualPanel = nextPanel;

		// starts fade coroutine
		StartCoroutine(Fade(true));
		// calls transition event at end of fade
		Invoke("CallEndTransitionEvent", fadeDuration);
	}

	// main fade coroutine (fade in and out)
	IEnumerator Fade(bool fadeGameIn)
	{
		bool done = fadeGameIn ? fadePanel.alpha <= alphaComparisonThreshold : fadePanel.alpha >= 1 - alphaComparisonThreshold;

		// fading loop
		while (!done)
		{
			float step = (1 / fadeDuration) * Time.deltaTime;
			step = fadeGameIn ? -step : step;

			done = fadeGameIn ? fadePanel.alpha <= 0 + alphaComparisonThreshold : fadePanel.alpha >= 1 - alphaComparisonThreshold;

			fadePanel.blocksRaycasts = fadePanel.alpha > 0;

			fadePanel.alpha += step;

			if(done)
			{
				if(fadeGameIn)
				{
					fadePanel.alpha = 0;
					fadePanel.blocksRaycasts = false;
				}
				else
				{

					fadePanel.alpha = 1;
					fadePanel.blocksRaycasts = true;
				}

				yield break;
			}

			yield return null;
		}
	}

	// calls event at end of transition
	void CallEndTransitionEvent()
	{
		inTransition = false;

		if(onEndTransition != null)
		{
			onEndTransition.Invoke();
		}
	}

	[Serializable]
	public class GamePanel
	{
		public GamePhase phase;
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