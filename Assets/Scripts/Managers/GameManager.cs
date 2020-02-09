using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static GeneralDialogue;

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
		DECKBUILDING,
		FIGHT_INTRO,
		FIGHT,
		CONSEQUENCES
	}

	// enum for game popup
	public enum GamePopup
	{
		EMPTY,
		SETTINGS,
		SHOGUN_CHARACTER,
		SHOGUN_NEW_CARD
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
		if(eventSystem.currentSelectedGameObject != null)
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
		panelManager.Init();
		popupManager.Init();

		// shogunManager.Init(
		// 	() => JumpTo(GamePhase.DECKBUILDING),
		// 	() => Pop(GamePopup.SHOGUN_CHARACTER),
		// 	() => Pop(GamePopup.SHOGUN_NEW_CARD)
		// );

		PlugEvents();

		InitTitle();
	}

	void InitTitle()
	{
		titleManager.Init(
			() => panelManager.JumpTo(GamePhase.SHOGUN, () => shogunManager = FindObjectOfType<ShogunManager>()),
			() => Application.Quit()
		);

		// shogunManager.Init()
	}

	// subscribe events to EventManager here
	void PlugEvents()
	{
		panelManager.eventsManager.AddPhaseAction(GamePhase.TITLE, InitTitle);

		// eventsManager.AddPhaseAction(GamePhase.SHOGUN, () => { shogunManager.StartDialogue(testDialogue); });
	}

	public void GiveMark(Character character, bool mainDialogueDone, int additionnalDialogueIndex = 0)
	{
		gameData.playerData.GiveMark(character, mainDialogueDone, additionnalDialogueIndex);
	}

	public List<Mark> GetAllPlayerMark()
	{
		return gameData.playerData.dialogueMarks;
	}
}