using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// main manager script of the game
// everything should flow from here but not flow back to here
public class GameManager : MonoBehaviour, IDebugable
{
	// static reference to the class, used for testing or emergency
	public static GameManager Get;

	[Header("Settings")]
	public List<CategoryColor> colors = new List<CategoryColor>(4);
	[Space]
	public SkinData selectedSkin;

	[Header("Assign in Inspector")]
	public GameObject persistantContainer;
	public PanelManager panelManager;
	public PopupManager popupManager;
	public EventSystem eventSystem;
	public GameData gameData;
	public TitleManager titleManager;
	public AudioManager audioManager;

	[Header("Debug")]
	public ShogunManager shogunManager;
	public FightManager fightManager;
	public ConsequencesManager consequencesManager;

	IDebugable debuguableInterface => (IDebugable) this;

	string IDebugable.debugLabel => "<b>[GameManager] : </b>";

	Enemy actualEnemy;

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

	public enum Enemy
	{
		SHIGERU, // 1
		GORO, // 3
		HIROO // 6
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

		actualEnemy = (Enemy) 0;

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
		audioManager.Init();

		PlugPanelEvents();
		PlugPopupEvents();

		// real start of game (these actions are normally called during transition events)
		popupManager.GetAllScenePopups();
		popupManager.ForceCancelPop();
		InitTitlePanel();

		Skinning.Init(selectedSkin);
	}

	// called when we get to the title panel
	void InitTitlePanel()
	{
		Skinning.ResetSkin(selectedSkin);
		
		if(titleManager == null)
		{
			Debug.LogError(debuguableInterface.debugLabel + "TitleManager component shouldn't be null. If we can't get scene references we can't do anything.");
			return;
		}

		titleManager.Init(
			() => panelManager.JumpTo(GamePhase.SHOGUN, () =>
			{
				shogunManager = FindObjectOfType<ShogunManager>();
				shogunManager.PreInit();

				audioManager.PlayMusic(GamePhase.SHOGUN);
			}),
			() => Application.Quit()
		);

		popupManager.GetPopupFromType<SettingsPopup>().SpecificInit();
	}

	// called when we get to the shogun panel
	void InitShogunPanel()
	{
		Skinning.ResetSkin(selectedSkin);
		
		if(shogunManager == null)
		{
			Debug.LogError(debuguableInterface.debugLabel + "ShogunManager component shouldn't be null. If we can't get scene references we can't do anything.");
			return;
		}

		shogunManager.Init(
			() => popupManager.Pop(GamePopup.SHOGUN_DEDUCTION),
			AddClueToPlayer
		);

		GeneralDialogue selected = gameData.shogunDialogues.Find(item => { return item.assignedEnemy == actualEnemy; });

		selected.Init();

		popupManager.GetPopupFromType<ShogunPopup>().SpecificInit(
			selected.GetAllClues(),
			selected.unlockableConclusions,
			shogunManager.characters,
			() => popupManager.CancelPop(),
			() => panelManager.JumpTo(GamePhase.FIGHT, () =>
			{
				fightManager = FindObjectOfType<FightManager>();
				fightManager.PreInit(gameData.combatDialogues.Find(item => { return item.enemy == actualEnemy; }));

				audioManager.PlayMusic(GamePhase.FIGHT);
			})
		);

		gameData.ResetPlayerClues();
		shogunManager.StartDialogue(selected);
	}

	void InitFightPanel()
	{
		Skinning.ResetSkin(selectedSkin);
		
		if(fightManager == null)
		{
			Debug.LogError(debuguableInterface.debugLabel + "FightManager component shouldn't be null. If we can't get scene references we can't do anything.");
		}

		fightManager.Init(
			gameData.comonPunchlines,
			() => panelManager.JumpTo(GamePhase.CONSEQUENCES, () => { consequencesManager = FindObjectOfType<ConsequencesManager>(); actualEnemy++; }),
			() => { panelManager.JumpTo(GamePhase.CONSEQUENCES, () => consequencesManager = FindObjectOfType<ConsequencesManager>()); gameData.gameOver = true; }
		);
	}

	void InitConsequencesPanel()
	{
		Skinning.ResetSkin(selectedSkin);
		
		if(consequencesManager == null)
		{
			Debug.LogError(debuguableInterface.debugLabel + "ConsequencesManager component shouldn't be null. If we can't get scene references we can't do anything.");
		}

		consequencesManager.Init(
			gameData.gameOver,
			(int) actualEnemy,
			() => panelManager.JumpTo(GamePhase.SHOGUN, () =>
			{
				shogunManager = FindObjectOfType<ShogunManager>();
				shogunManager.PreInit();
			}),
			gameData.gameOver?null : gameData.combatDialogues[(int) actualEnemy - 1].playerWinConsequence
		);
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
		panelManager.eventsManager.AddPhaseAction(GamePhase.FIGHT, InitFightPanel);
		panelManager.eventsManager.AddPhaseAction(GamePhase.CONSEQUENCES, InitConsequencesPanel);
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