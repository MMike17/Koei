using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
	public TransitionEventsManager eventsManager;

	[Header("Debug")]
	public GamePhase actualPanel;
	public GamePhase nextPanel;

	public bool initialized => initializableInterface.initializedInternal;

	IDebugable debugableInterface => (IDebugable) this;
	IInitializable initializableInterface => (IInitializable) this;

	string IDebugable.debugLabel => "<b>[PanelsManager] : </b>";
	bool IInitializable.initializedInternal { get; set; }

	Action onLoadingDone;
	bool inTransition;

	public void Init()
	{
		actualPanel = GamePhase.TITLE;

		// protects fade panel from disapearing on scene loading
		DontDestroyOnLoad(fadePanel.gameObject);

		eventsManager.Init();

		initializableInterface.InitInternal();
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;

		Debug.Log(debugableInterface.debugLabel + "Initializing done");
	}

	// jumps to corresponding panel and calls transition event
	public void JumpTo(GamePhase newPanel, Action getRefsCallback)
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

		GamePanel panel = panels.Find(item => { return item.phase == newPanel; });

		if(panel == null)
		{
			Debug.LogError(debugableInterface.debugLabel + "Can't find game panel with GamePhase " + panel.phase);
			return;
		}

		if(getRefsCallback == null)
		{
			Debug.LogError(debugableInterface.debugLabel + "Can't change scene if there are no callback to get refs");
			return;
		}

		onLoadingDone = getRefsCallback;

		inTransition = true;

		StartCoroutine(ChageScene(panel.sceneBuildIndex));

		nextPanel = newPanel;
	}

	void Update()
	{
		// spins spinner if vivisble
		if(fadePanel.alpha > 0)
			fadeSpinner.Rotate(0, 0, -spinnerSpeed * Time.deltaTime);
	}

	// main coroutine for scene changing
	IEnumerator ChageScene(int sceneIndex)
	{
		// fades panel in
		yield return new WaitUntil(() => { return Fade(true); });

		// loads scene
		yield return SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);

		// gets ref of new scene
		onLoadingDone.Invoke();

		// fades panel out
		yield return new WaitUntil(() => { return Fade(false); });

		// called transition event
		eventsManager.CallPhaseActions(nextPanel);

		actualPanel = nextPanel;
		inTransition = false;
		yield break;
	}

	// main fade coroutine (fade in and out)
	bool Fade(bool fadePanelIn)
	{
		float step = (1 / fadeDuration) * Time.deltaTime;
		step = fadePanelIn ? step : -step;

		fadePanel.alpha += step;
		fadePanel.blocksRaycasts = fadePanel.alpha > 0;

		bool done = fadePanelIn ? fadePanel.alpha >= 1 - alphaComparisonThreshold : fadePanel.alpha <= alphaComparisonThreshold;

		if(done)
		{
			// snaps values so that they are always perfect at end of fade
			if(fadePanelIn)
			{
				fadePanel.alpha = 1;
				fadePanel.blocksRaycasts = true;
			}
			else
			{
				fadePanel.alpha = 0;
				fadePanel.blocksRaycasts = false;
			}
		}

		return done;
	}

	[Serializable]
	public class GamePanel
	{
		public GamePhase phase;
		public int sceneBuildIndex;
	}
}