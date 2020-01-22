using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// class managing gameplay of Shogun panel
public class ShogunManager : MonoBehaviour, IDebugable, IInitializable
{
	[Header("Settings")]
	public float dialogueSpeed;
	public float moveSpeed, zoomSpeed;
	public int highlightLength;
	public Color textColor, highlightColor, hideColor;
	[Space]
	public List<ShogunCharacter> characters;

	[Header("Assing in Inspector")]
	public List<ShogunChoice> choices;
	[Space]
	public EventSystem eventSystem;
	public TextMeshProUGUI dialogueText, quitPopupText;
	public Button quitPopupQuitButton, quitPopupResumeButton, quitShogunButton;
	public Animator nextDialogueIndicator;
	public RectTransform table;

	IDebugable debugableInterface => (IDebugable) this;
	IInitializable initializableInterface => (IInitializable) this;

	public bool initialized => initializableInterface.initializedInternal;

	bool IInitializable.initializedInternal { get; set; }
	string IDebugable.debugLabel => "<b>[ShogunManager] : </b>";

	Dialogue selectedDialogue;
	Dialogue.Character actualCharacter;
	Action endDialogue;
	float dialogueTimer, actualZoom, quitPopupTimer;
	int lineIndex, dialogueIndex, quitPopupIndex;
	bool showedFirst, isQuittingPopup;

	void Awake()
	{
		// prevents choices from appearing during transition
		HideChoices();

		dialogueText.text = string.Empty;
	}

	// receives actions from GameManager
	public void Init(Action quitButtonCallback, Action resumeButtonCallback, Action endDialogueCallback)
	{
		// action to call when player selects choice that ends dialogue
		endDialogue = endDialogueCallback;

		// plugs in actions for the popup buttons
		quitPopupQuitButton.onClick.RemoveAllListeners();
		quitPopupQuitButton.onClick.AddListener(() =>
		{
			selectedDialogue = null;

			if(quitButtonCallback != null)
				quitButtonCallback.Invoke();
		});

		quitPopupResumeButton.onClick.RemoveAllListeners();
		quitPopupResumeButton.onClick.AddListener(() =>
		{
			isQuittingPopup = false;
			quitPopupIndex = 0;

			if(resumeButtonCallback != null)
				resumeButtonCallback.Invoke();
		});

		quitShogunButton.onClick.RemoveAllListeners();
		quitShogunButton.onClick.AddListener(() => QuitShogunPhase());

		isQuittingPopup = false;

		initializableInterface.InitInternal();
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;
		Debug.Log(debugableInterface.debugLabel + "Initializing done");
	}

	void Update()
	{
		// doesn't do anything if Dialogue object is not set
		if(selectedDialogue == null)
		{
			return;
		}

		if(!initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "Not initialized");
			return;
		}

		if(isQuittingPopup)
		{
			quitPopupText.text = DialogueTools.HighlightString(selectedDialogue.endLine, quitPopupText.color, highlightColor, quitPopupIndex, highlightLength);

			quitPopupTimer += Time.deltaTime;

			if(quitPopupTimer >= 1 / dialogueSpeed)
			{
				quitPopupTimer = 0;
				quitPopupIndex++;
			}
		}

		// enables button to skip the "selected" stage (usefull only for gamepad use)
		if(eventSystem.currentSelectedGameObject != null)
		{
			eventSystem.SetSelectedGameObject(null);
		}

		bool characterIsDone;

		if(showedFirst)
		{
			MoveCamera();

			characterIsDone = CharacterLine(selectedDialogue.GetCharacterLines(actualCharacter) [dialogueIndex]);
		}
		else
		{
			ChangeZoom(1);

			characterIsDone = CharacterLine(selectedDialogue.introLine);
		}

		// shows choices panels if character is done talking
		if(characterIsDone && selectedDialogue.IsCharacterDone(actualCharacter, dialogueIndex))
		{
			ShowChoices();
		}
		else // hides choices panels while character is talking
		{
			HideChoices();
		}
	}

	void MoveCamera()
	{
		ShogunCharacter selectedCharacter = characters.Find(item => { return item.character == actualCharacter; });

		if(selectedCharacter == null)
		{
			Debug.LogError(debugableInterface.debugLabel + "Couldn't find settings for character " + actualCharacter.ToString());
			return;
		}

		// moves background to target
		table.anchoredPosition = Vector2.MoveTowards(table.anchoredPosition, selectedCharacter.targetPosition * selectedCharacter.zoomLevel, moveSpeed * Time.deltaTime);

		ChangeZoom(selectedCharacter.zoomLevel);
	}

	void ChangeZoom(float targetZoom)
	{
		// changes background scale to emulate zoom effects
		actualZoom = Mathf.MoveTowards(actualZoom, targetZoom, zoomSpeed * Time.deltaTime);
		table.localScale = Vector3.one * actualZoom;
	}

	// writes the character line with highlight
	bool CharacterLine(string line)
	{
		// if line is displayed fully
		if(lineIndex >= line.Length + highlightLength)
		{
			// if all lines of dialogue are displayed
			if(selectedDialogue.IsCharacterDone(actualCharacter, dialogueIndex))
			{
				// hides cursor
				if(!nextDialogueIndicator.GetCurrentAnimatorStateInfo(0).IsName("Hide"))
				{
					nextDialogueIndicator.Play("Hide");
				}

				return true;
			}
			else
			{
				// shows cursor
				if(!nextDialogueIndicator.GetCurrentAnimatorStateInfo(0).IsName("Press"))
				{
					nextDialogueIndicator.Play("Press");
				}

				// goes to next dialogue line
				if(Input.GetMouseButtonDown(0))
				{
					dialogueIndex++;
					NewLine();
				}

				return false;
			}
		}

		// hides cursor
		if(!nextDialogueIndicator.GetCurrentAnimatorStateInfo(0).IsName("Hide"))
		{
			nextDialogueIndicator.Play("Hide");
		}

		// character display index timer and incrementation
		dialogueTimer += Time.deltaTime;

		if(dialogueTimer >= 1 / dialogueSpeed)
		{
			dialogueTimer = 0;
			lineIndex++;
		}

		// displays highlighted character line
		dialogueText.text = DialogueTools.HighlightString(line, textColor, highlightColor, lineIndex, highlightLength);

		return false;
	}

	// hides choices panels
	void HideChoices()
	{
		choices.ForEach(item => item.HidePanel());
	}

	// shows choices depending on how many there are
	void ShowChoices()
	{
		int howManyPortraits = selectedDialogue.characterDialogues.Count;

		if(howManyPortraits >= choices.Count)
		{
			Debug.LogError(debugableInterface.debugLabel + "Too many character portraits to show");
			return;
		}

		int initializedCount = 0;
		choices.ForEach(item =>
		{
			if(item.initialized)
			{
				initializedCount++;
			}
		});

		if(initializedCount == howManyPortraits)
		{
			Debug.Log(debugableInterface.debugLabel + "All necessary choices have been initialized");
			return;
		}

		// initializes (or reinitializes) choices panels with necessary infos
		for (int i = 0; i < choices.Count; i++)
		{
			int j = i;

			bool active = j < howManyPortraits;

			choices[j].gameObject.SetActive(active);

			if(active)
			{
				choices[j].Init(characters[j].characterPortrait, textColor, highlightColor, hideColor, () => SelectChoice(characters[j].character));
			}
		}
	}

	// called when the player clicks on a choice
	void SelectChoice(Dialogue.Character character)
	{
		CharacterDialogue selected = selectedDialogue.characterDialogues.Find(item => { return item.character == character; });

		showedFirst = true;

		if(selected == null)
		{
			Debug.LogError(debugableInterface.debugLabel + "Can't find character dialogue with character " + character.ToString());
			return;
		}

		actualCharacter = character;

		NewDialogue();
	}

	// resets variables for new character dialogue
	void NewDialogue()
	{
		NewLine();

		dialogueIndex = 0;
	}

	// resets variables for new character line
	void NewLine()
	{
		dialogueTimer = 0;
		lineIndex = 0;
	}

	// resets component for next phase
	void QuitShogunPhase()
	{
		if(endDialogue != null)
			endDialogue.Invoke();

		isQuittingPopup = true;
	}

	// called by GameManager to start dialogue
	public void StartDialogue(Dialogue dialogue)
	{
		if(!initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "Not initialized");
			return;
		}

		actualCharacter = Dialogue.Character.SHOGUN;

		dialogueText.color = textColor;
		selectedDialogue = dialogue;

		actualZoom = 1;

		NewDialogue();
	}

	[Serializable]
	public class ShogunCharacter
	{
		public Dialogue.Character character;
		public Sprite characterPortrait;
		public Vector2 targetPosition;
		public float zoomLevel;
	}
}