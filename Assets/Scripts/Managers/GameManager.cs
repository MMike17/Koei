using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// main manager script of the game
// everything should flow from here but not flow back to here
public class GameManager : MonoBehaviour, IDebugable
{
	// static reference to the class, used for testing or emergency
	public static GameManager Get;

	[Header("Assign in Inspector")]
	public GameObject persistantContainer;
	public PanelManager panelManager;
	public PopupManager popupManager;
	public EventSystem eventSystem;

	[Header("Debug")]
	public GameData gameData;
	public TitleManager titleManager;
	public ShogunManager shogunManager;

	[Header("test")]
	public GeneralDialogue testDialogue;

	IDebugable debuguableInterface => (IDebugable) this;

	string IDebugable.debugLabel => "<b>[GameManager] : </b>";

	// enum for game phases
	public enum GamePhase
	{
		TITLE,
		SHOGUN,
		FIGHT,
		CONSEQUENCES
	}

	// enum for game popup
	public enum GamePopup
	{
		EMPTY,
		TITLE_SETTINGS,
		SHOGUN_DEDUCTION
	}

	void Awake()
	{
		// makes sure there is only one GameManager instance and init is not done twice
		if(Get == this)
		{
			return;
		}

		if(Get != null)
		{
			Destroy(Get.gameObject);
		}

		Init();
	}

	void Update()
	{
		// enables button to skip the "selected" stage (usefull only for gamepad use)
		if(eventSystem != null && eventSystem.currentSelectedGameObject != null)
		{
			eventSystem.SetSelectedGameObject(null);
		}
	}

	void Init()
	{
		Get = this;

		// makes object persistant so that they don't get destroyed on scene unloading
		DontDestroyOnLoad(persistantContainer);

		// initializes all managers
		panelManager.Init(() =>
		{
			eventSystem = FindObjectOfType<EventSystem>();

			// gets refs to popups and closes them without cool effect (as if it was never here)
			popupManager.GetAllScenePopups();
			popupManager.ForceCancelPop();
		});

		popupManager.Init();

		PlugPanelEvents();
		PlugPopupEvents();

		// real start of game (these actions are normally called during transition events)
		popupManager.GetAllScenePopups();
		popupManager.ForceCancelPop();
		InitTitlePanel();
	}

	// called when we get to the title panel
	void InitTitlePanel()
	{
		if(titleManager == null)
		{
			Debug.LogError(debuguableInterface.debugLabel + "TitleManager component shouldn't be null. If we can't get scene references we can't do anything.");
			return;
		}

		titleManager.Init(
			() => panelManager.JumpTo(GamePhase.SHOGUN, () => shogunManager = FindObjectOfType<ShogunManager>()),
			() => Application.Quit()
		);

		popupManager.GetPopupFromType<SettingsPopup>().SpecificInit();
	}

	// called when we get to the shogun panel
	void InitShogunPanel()
	{
		if(shogunManager == null)
		{
			Debug.LogError(debuguableInterface.debugLabel + "ShogunManager component shouldn't be null. If we can't get scene references we can't do anything.");
			return;
		}

		shogunManager.Init(
			() => popupManager.Pop(GamePopup.SHOGUN_DEDUCTION),
			AddClueToPlayer
		);

		testDialogue.Init();

		popupManager.GetPopupFromType<ShogunPopup>().SpecificInit(
			testDialogue.GetAllClues(),
			testDialogue.unlockableConclusions,
			shogunManager.characters,
			() => popupManager.CancelPop()
		);

		gameData.ResetPlayerClues();
		shogunManager.StartDialogue(testDialogue);
	}

	// gets called every time we pop deduction popup
	void StartDeduction()
	{
		popupManager.GetPopupFromType<ShogunPopup>().ChecKnobsState(gameData.playerClues);
	}

	// subscribe events to panel EventManager here
	void PlugPanelEvents()
	{
		panelManager.eventsManager.AddPhaseAction(GamePhase.TITLE, InitTitlePanel);
		panelManager.eventsManager.AddPhaseAction(GamePhase.SHOGUN, InitShogunPanel);
	}

	// subscribe events to popup EventManager here
	void PlugPopupEvents()
	{
		popupManager.SubscribeEvent(GamePopup.SHOGUN_DEDUCTION, StartDeduction);
	}

	bool AddClueToPlayer(Clue clue)
	{
		return gameData.FindClue(clue);
	}
}