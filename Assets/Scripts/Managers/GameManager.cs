using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static GameData;

// main manager script of the game
// everything should flow from here but not flow back to here
public class GameManager : MonoBehaviour, IDebugable
{
	// static reference to the class, used for testing or gross stitching
	public static GameManager Get;

	[Header("Settings")]
	public List<CategoryColor> colors = new List<CategoryColor>(4);
	[Space]
	public SkinData selectedSkin;
	public TMP_FontAsset projectFont;
	public List<TMP_FontAsset> fontToIgnore;
	[Space]
	public bool useCheats;

	[Header("Assign in Inspector")]
	public GameObject persistantContainer;
	public PanelManager panelManager;
	public PopupManager popupManager;
	public EventSystem eventSystem;
	public GameData gameData;
	public TitleManager titleManager;
	public AudioProjectManager audioProjectManager;
	public AudioManager audioManager;

	[Header("Debug")]
	public IntroManager introManager;
	public ShogunManager shogunManager;
	public FightManager fightManager;
	public ConsequencesManager consequencesManager;

	IDebugable debuguableInterface => (IDebugable) this;

	string IDebugable.debugLabel => "<b>[" + GetType() + "] : </b>";

	Enemy actualEnemy;

	// enum for game phases
	public enum GamePhase
	{
		TITLE,
		INTRO,
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

	void Start()
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
			eventSystem.SetSelectedGameObject(null);
	}

	void Init()
	{
		Get = this;

		actualEnemy = (Enemy) 0;

		// makes object persistant so that they don't get destroyed on scene unloading
		DontDestroyOnLoad(persistantContainer);

		// initializes all text content
		gameData.Init();

		// initializes all managers
		panelManager.Init(() =>
		{
			eventSystem = FindObjectOfType<EventSystem>();

			// gets refs to popups and closes them without cool effect (as if it was never here)
			popupManager.GetAllScenePopups();
			popupManager.ForceCancelPop();

			audioProjectManager.FadeMusicIn();

			foreach (TextMeshProUGUI text in Resources.FindObjectsOfTypeAll<TextMeshProUGUI>())
			{
				if(!fontToIgnore.Contains(text.font))
					text.font = projectFont;
			}
		});

		audioManager.Init();
		audioProjectManager.Init(panelManager.fadeDuration, popupManager.fadeDuration, () => { return panelManager.nextPanel; }, () => { return panelManager.actualPanel; }, () => { return popupManager.actualPopup; });
		popupManager.Init(audioProjectManager.FadePopupMusicIn, audioProjectManager.FadePopupMusicOut);

		PlugPanelEvents();
		PlugPopupEvents();

		// real start of game (these actions are normally called during transition events)
		popupManager.GetAllScenePopups();
		popupManager.ForceCancelPop();
		InitTitlePanel();

		audioProjectManager.FadeMusicIn();

		Skinning.Init(selectedSkin);

		foreach (TextMeshProUGUI text in Resources.FindObjectsOfTypeAll<TextMeshProUGUI>())
		{
			if(!fontToIgnore.Contains(text.font))
				text.font = projectFont;
		}
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
			() => panelManager.JumpTo(GamePhase.INTRO, () =>
			{
				introManager = FindObjectOfType<IntroManager>();
				audioProjectManager.FadeMusicOut();
			}),
			() => Application.Quit()
		);
	}

	// called when we get to the intro panel
	void InitIntroPanel()
	{
		Skinning.ResetSkin(selectedSkin);

		if(introManager == null)
		{
			Debug.LogError(debuguableInterface.debugLabel + "IntroManager component shouldn't be null. If we can't get scene references we can't do anything.");
			return;
		}

		introManager.Init(
			useCheats,
			() => panelManager.JumpTo(GamePhase.SHOGUN, () =>
			{
				shogunManager = FindObjectOfType<ShogunManager>();
				shogunManager.PreInit(
					GameState.NORMAL,
					gameData.enemyContent.Find(item => { return item.enemy == actualEnemy; }).shogunDialogue,
					AddClueToPlayer
				);

				audioProjectManager.FadeMusicOut();
			}));
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

		shogunManager.Init(useCheats, () => popupManager.Pop(GamePopup.SHOGUN_DEDUCTION));

		EnemyBundle bundle = gameData.enemyContent.Find(item => { return item.enemy == actualEnemy; });

		GeneralDialogue selectedGeneral = bundle.shogunDialogue;
		selectedGeneral.Init();

		CombatDialogue selectedCombat = bundle.combatDialogue;

		popupManager.GetPopupFromType<ShogunPopup>().SpecificInit(
			useCheats,
			selectedGeneral.GetAllClues(),
			selectedGeneral.unlockableConclusions,
			shogunManager.characters,
			() => popupManager.CancelPop(),
			() =>
			{
				audioProjectManager.FadeMusicOut();

				panelManager.JumpTo(GamePhase.FIGHT, () =>
				{
					fightManager = FindObjectOfType<FightManager>();
					fightManager.PreInit(selectedCombat);
				});
			},
			selectedCombat.actualState
		);

		if(selectedCombat.actualState == GameState.NORMAL)
			gameData.ResetPlayerClues();

		shogunManager.StartDialogue(selectedGeneral);
	}

	void InitFightPanel()
	{
		Skinning.ResetSkin(selectedSkin);

		if(fightManager == null)
		{
			Debug.LogError(debuguableInterface.debugLabel + "FightManager component shouldn't be null. If we can't get scene references we can't do anything.");
		}

		EnemyBundle bundle = gameData.enemyContent.Find(item => { return item.enemy == actualEnemy; });
		CombatDialogue selectedCombat = bundle.combatDialogue;

		fightManager.Init(
			useCheats,
			bundle.punchlines,
			bundle.shogunDialogue,
			() =>
			{
				panelManager.JumpTo(GamePhase.CONSEQUENCES, () => consequencesManager = FindObjectOfType<ConsequencesManager>());

				audioProjectManager.FadeMusicOut();
			},
			() =>
			{
				panelManager.JumpTo(GamePhase.CONSEQUENCES, () => consequencesManager = FindObjectOfType<ConsequencesManager>());

				audioProjectManager.FadeMusicOut();
			}
		);
	}

	void InitConsequencesPanel()
	{
		Skinning.ResetSkin(selectedSkin);

		if(consequencesManager == null)
		{
			Debug.LogError(debuguableInterface.debugLabel + "ConsequencesManager component shouldn't be null. If we can't get scene references we can't do anything.");
		}

		EnemyBundle bundle = gameData.enemyContent.Find(item => { return item.enemy == actualEnemy; });
		CombatDialogue selectedCombat = bundle.combatDialogue;

		consequencesManager.Init(
			selectedCombat.actualState,
			(int) actualEnemy,
			() => panelManager.JumpTo(GamePhase.SHOGUN, () =>
			{
				shogunManager = FindObjectOfType<ShogunManager>();
				shogunManager.PreInit(
					selectedCombat.actualState,
					bundle.shogunDialogue,
					AddClueToPlayer
				);

				audioProjectManager.FadeMusicOut();
			}),
			() => actualEnemy++,
			selectedCombat.actualState != GameState.NORMAL ? null : bundle.combatDialogue.playerWinConsequence
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
		panelManager.eventsManager.AddPhaseAction(GamePhase.INTRO, InitIntroPanel);
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