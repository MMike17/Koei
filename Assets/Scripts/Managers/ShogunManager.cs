using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GeneralDialogue;

// class managing gameplay of Shogun panel
public class ShogunManager : MonoBehaviour, IDebugable, IInitializable
{
	[Header("Settings")]
	public float dialogueSpeed;
	public int highlightLength;
	public Color textColor, additionalDialogueColor, highlightColor;
	[Space]
	public List<ShogunCharacter> characters;

	[Header("Assing in Inspector")]
	public RectTransform dialogueScrollList;
	public GameObject characterTextPrefab, playerChoicePrefab, playerTextPrefab;
	public Button changeCharacterButton, quitButton;
	public Image characterPortrait;

	IDebugable debugableInterface => (IDebugable) this;
	IInitializable initializableInterface => (IInitializable) this;

	public bool initialized => initializableInterface.initializedInternal;

	bool IInitializable.initializedInternal { get; set; }
	string IDebugable.debugLabel => "<b>[ShogunManager] : </b>";

	GeneralDialogue actualDialogue;
	CharacterDialogue actualCharacterDialogue;
	CharacterDialogue.AdditionnalDialogue additionnalDialogue;

	Character actualCharacter;
	Action newCardEvent;
	DialogueWriter lastWriter;
	List<GameObject> lastSpawnedObjects;
	bool needsPlayerSpawn, forceStartDialogue, waitForPlayerChoice;

	// receives actions from GameManager
	public void Init(Action quitButtonCallback, Action changeCharacterButtonCallback, Action newCardCallback)
	{
		lastSpawnedObjects = new List<GameObject>();

		// plugs in actions for the buttons
		quitButton.onClick.RemoveAllListeners();
		quitButton.onClick.AddListener(() =>
		{
			if(quitButtonCallback != null)
			{
				quitButtonCallback.Invoke();
			}
		});

		changeCharacterButton.onClick.RemoveAllListeners();
		changeCharacterButton.onClick.AddListener(() =>
		{
			if(changeCharacterButtonCallback != null)
			{
				changeCharacterButtonCallback.Invoke();
			}
		});

		newCardEvent = newCardCallback;

		initializableInterface.InitInternal();

		Debug.Log(debugableInterface.debugLabel + "Initializing done");
	}

	public void StartDialogue(GeneralDialogue dialogue)
	{
		if(!initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "Not initialized");
			return;
		}

		actualDialogue = dialogue;
		actualDialogue.Init();

		ChangeCharacter(Character.SHOGUN);
		forceStartDialogue = false;

		characters.ForEach(item => item.Init());
	}

	public void ResetDialogue()
	{
		foreach (Transform child in dialogueScrollList)
		{
			Destroy(child.gameObject);
		}

		lastWriter = null;
		forceStartDialogue = false;
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;
		Debug.Log(debugableInterface.debugLabel + "Initializing done");
	}

	void Update()
	{
		// TODO : Add system to respawn text from character we already talked to so we can have a conversation hystory when we go back to character we already talked to

		if(actualCharacterDialogue != null && actualCharacterDialogue.IsMainDone())
		{
			ShowButtons();
		}
		else
		{
			HideButtons();
		}

		if((lastWriter != null && lastWriter.isDone) || forceStartDialogue)
		{
			// prevents update from spawning lines if we are waiting for the player to choose a dialogue line
			if(waitForPlayerChoice)
			{
				return;
			}

			forceStartDialogue = false;

			// previous line was a character line
			if(needsPlayerSpawn)
			{
				// if we need to show additionnal lines
				if(actualCharacterDialogue.IsMainDone())
				{
					lastSpawnedObjects = new List<GameObject>();

					// check all additional dialogues
					for (int i = 0; i < actualCharacterDialogue.additionalDialogue.Count; i++)
					{
						bool shouldShowDialogue = false;

						// checks if any player Mark unlocks this additional dialogue
						foreach (Mark mark in GameManager.Get.GetAllPlayerMark())
						{
							if(actualCharacterDialogue.additionalDialogue[i].trigger.IsUnlocked(mark))
								shouldShowDialogue = true;
						}

						// marks dialogue as not unlocked if allready saw it
						if(GetCharacter(actualCharacter).additionalDialogueIndexes.Contains(i))
						{
							shouldShowDialogue = false;
						}

						if(shouldShowDialogue)
						{
							SpawnPlayerChoice(actualCharacterDialogue.additionalDialogue[i].playerQuestion, i);
						}
					}
				}
				else // if we show next main dialogue line
				{
					SpawnPlayerChoice(actualCharacterDialogue.mainDialogue[GetCharacter(actualCharacter).lastIndex].playerQuestion);
				}
			}
			else // previous line was player line
			{
				if(GetCharacter(actualCharacter).lastIndex >= actualCharacterDialogue.mainDialogue.Count)
				{
					actualCharacterDialogue.MarkMainAsDone();
				}

				if(actualCharacterDialogue.IsMainDone())
				{
					if(additionnalDialogue != null) // show additionnal dialogue
					{
						SpawnCharacterLine(additionnalDialogue.characterAnswer, additionalDialogueColor);
						additionnalDialogue = null;
					}
				}
				else // show next dialogue line
				{
					SpawnCharacterLine(actualCharacterDialogue.mainDialogue[GetCharacter(actualCharacter).lastIndex].characterAnswer, textColor);
				}
			}
		}
	}

	void ChangeCharacter(Character character)
	{
		ResetDialogue();

		actualCharacter = character;
		actualCharacterDialogue = actualDialogue.GetCharacterDialogue(actualCharacter);
		additionnalDialogue = null;

		forceStartDialogue = true;
		waitForPlayerChoice = false;

		SpawnCharacterLine(actualCharacterDialogue.firstLine, textColor);
	}

	void SpawnPlayerChoice(string line, int index = int.MaxValue)
	{
		Button spawned = Instantiate(playerChoicePrefab, dialogueScrollList).GetComponent<Button>();

		spawned.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = line;
		spawned.onClick.AddListener(() => SelectChoice(line, index));

		lastSpawnedObjects.Add(spawned.gameObject);

		waitForPlayerChoice = true;
	}

	void SelectChoice(string line, int index)
	{
		bool mainDone = true;
		Color actualTextColor = textColor;

		// if main is not done yet
		if(index == int.MaxValue)
		{
			index = 0;
			mainDone = false;
		}
		else // if dialogue to show is additionnal
		{
			GetCharacter(actualCharacter).additionalDialogueIndexes.Add(index);
			additionnalDialogue = actualCharacterDialogue.additionalDialogue[index];
			actualTextColor = additionalDialogueColor;
		}

		// deletes all previously spawned texts
		if(lastSpawnedObjects.Count > 0)
		{
			lastSpawnedObjects.ForEach(item => Destroy(item));
			lastSpawnedObjects.Clear();
		}

		// give player mark of the actual dialogue
		GameManager.Get.GiveMark(actualCharacter, mainDone, index);

		SpawnPlayerLine(line, actualTextColor);

		needsPlayerSpawn = false;
		waitForPlayerChoice = false;
		GetCharacter(actualCharacter).lastIndex++;
	}

	void SpawnPlayerLine(string line, Color text)
	{
		DialogueWriter spawned = Instantiate(playerTextPrefab, dialogueScrollList).transform.GetChild(0).GetComponent<DialogueWriter>();

		spawned.Reset();
		spawned.Play(line, dialogueSpeed * 2, Mathf.RoundToInt(highlightLength * 1.5f), highlightColor, text);

		lastWriter = spawned;
	}

	void SpawnCharacterLine(string line, Color text)
	{
		DialogueWriter spawned = Instantiate(characterTextPrefab, dialogueScrollList).GetComponent<DialogueWriter>();

		spawned.Play(line, dialogueSpeed, highlightLength, highlightColor, text);

		lastWriter = spawned;
		needsPlayerSpawn = true;
	}

	void HideButtons()
	{
		changeCharacterButton.gameObject.SetActive(false);
		quitButton.gameObject.SetActive(false);
	}

	void ShowButtons()
	{
		changeCharacterButton.gameObject.SetActive(true);
		quitButton.gameObject.SetActive(true);
	}

	ShogunCharacter GetCharacter(Character character)
	{
		ShogunCharacter selected = characters.Find(item => { return item.character == character; });

		if(selected == null)
		{
			Debug.LogError(debugableInterface.debugLabel + "Couldn't find ShogunCharacter with character " + character.ToString());
			return null;
		}

		if(!selected.initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "ShogunCharacter is not initialized (this will cause null refs later)");
			return null;
		}

		return selected;
	}

	[Serializable]
	public class ShogunCharacter : IInitializable
	{
		public Character character;
		public Sprite characterPortrait;
		public Button popupSelectionButton;

		// hidden from inspector
		public int lastIndex { get; set; }
		public List<int> additionalDialogueIndexes { get; set; }

		public bool initialized => initializableInterface.initializedInternal;

		IInitializable initializableInterface => (IInitializable) this;

		bool IInitializable.initializedInternal { get; set; }

		void IInitializable.InitInternal()
		{
			initializableInterface.initializedInternal = true;
		}

		public void Init()
		{
			additionalDialogueIndexes = new List<int>();
			lastIndex = 0;

			initializableInterface.InitInternal();
		}
	}
}