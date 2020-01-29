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

	[Header("Settings")]

	[Header("Title")]
	public Button titlePlay;
	public Button titleSettings;
	public Button titleQuit;

	[Header("Assign in Inspector")]
	public PanelManager panelManager;
	public PopupManager popupManager;
	public EventsManager eventsManager;
	public ShogunManager shogunManager;
	public EventSystem eventSystem;

	[Header("Debug")]
	public GameData gameData;

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
		FIGHT
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

		// initializes all managers
		panelManager.Init();
		popupManager.Init();
		eventsManager.Init();
		shogunManager.Init(
			() => JumpTo(GamePhase.DECKBUILDING),
			() => Pop(GamePopup.SHOGUN_CHARACTER),
			() => Pop(GamePopup.SHOGUN_NEW_CARD)
		);

		PlugEvents();
		PlugButtons();
	}

	// adds actions to buttons
	void PlugButtons()
	{
		titlePlay.onClick.AddListener(() => JumpTo(GamePhase.SHOGUN));
		// titleSettings.onClick.AddListener(() => popupManager.Pop(GamePopup.SETTINGS));
		// titleQuit.onClick.AddListener(() => Application.Quit());
	}

	// subscribe events to EventManager here
	void PlugEvents()
	{
		eventsManager.AddPhaseAction(GamePhase.SHOGUN, () => { shogunManager.StartDialogue(testDialogue); });
	}

	// /!\ Use this instead of PanelManager.JumpTo() /!\
	// jumps to GamePhase and calls subscribed events
	void JumpTo(GameManager.GamePhase phase)
	{
		panelManager.JumpTo(phase, () => eventsManager.CallPhaseActions(phase));
	}

	// /!\ Use this instead of PopupManager.Pop() /!\
	// pops GamePopup and calls subscribed events
	void Pop(GameManager.GamePopup popup)
	{
		popupManager.Pop(popup, () => eventsManager.CallPopupActions(popup));
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