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
	public Color playerTextColor, playerChoiceDone, playerChoiceUndone, characterTextColor, highlightColor;
	[Space]
	public List<ShogunCharacter> characters;

	[Header("Assing in Inspector")]
	public Button openDeductionButton;
	public RectTransform dialogueScrollList, cluesScrollList;
	public GameObject characterTextPrefab, playerChoicePrefab, playerTextPrefab, cluePrefab;
	public Image characterPortrait;

	IDebugable debugableInterface => (IDebugable) this;
	IInitializable initializableInterface => (IInitializable) this;

	public bool initialized => initializableInterface.initializedInternal;

	bool IInitializable.initializedInternal { get; set; }
	string IDebugable.debugLabel => "<b>[ShogunManager] : </b>";

	GeneralDialogue actualDialogue;
	CharacterDialogue actualCharacterDialogue;

	Func<Clue, bool> findClueEvent;
	Character actualCharacter;
	DialogueWriter lastWriter;
	List<GameObject> lastSpawnedDialogueObjects;
	bool needsPlayerSpawn, waitForPlayerChoice;

	// receives actions from GameManager
	public void Init(Action openDeductionPopup, Func<Clue, bool> findClue)
	{
		lastSpawnedDialogueObjects = new List<GameObject>();

		findClueEvent = findClue;

		// plug in buttons
		openDeductionButton.onClick.RemoveAllListeners();
		openDeductionButton.onClick.AddListener(() => openDeductionPopup.Invoke());

		initializableInterface.InitInternal();
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

		characters.ForEach(item => item.Init(() => ChangeCharacter(item.character)));

		ChangeCharacter(Character.SHOGUN);
	}

	public void ResetDialogue()
	{
		// deletes all previously spawned texts
		if(lastSpawnedDialogueObjects.Count > 0)
		{
			GameObject[] toDestroy = lastSpawnedDialogueObjects.ToArray();

			for (int i = 0; i < toDestroy.Length; i++)
			{
				Destroy(toDestroy[i]);
			}

			lastSpawnedDialogueObjects.Clear();
		}

		foreach (Transform child in dialogueScrollList)
		{
			Destroy(child.gameObject);
		}

		lastWriter = null;
		waitForPlayerChoice = false;
		needsPlayerSpawn = false;
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;

		Debug.Log(debugableInterface.debugLabel + "Initializing done");
	}

	void Update()
	{
		// TODO : Add system to respawn text from character we already talked to so we can have a conversation hystory when we go back to character we already talked to

		if(lastWriter != null && lastWriter.isDone)
		{
			// prevents update from spawning lines if we are waiting for the player to choose a dialogue line
			if(waitForPlayerChoice)
			{
				return;
			}

			// previous line was a character line
			if(needsPlayerSpawn)
			{
				Dialogue[] availableDialogues;

				if(actualCharacterDialogue.indexesPath.Count > 0)
				{
					// adds clue to player clue list if there was one
					if(actualCharacterDialogue.GetActualDialogue().hasClue)
					{
						if(findClueEvent.Invoke(actualCharacterDialogue.GetActualDialogue().clue))
							AddClueToList(actualCharacterDialogue.GetActualDialogue().clue);
					}

					availableDialogues = actualCharacterDialogue.GetActualDialogue().nextDialogues;

					// resets indexes path if branch can't go any further
					if(availableDialogues == null || availableDialogues.Length == 0)
					{
						actualCharacterDialogue.indexesPath = new List<int>();
						return;
					}
				}
				else
				{
					availableDialogues = actualCharacterDialogue.initialDialogues.ToArray();
				}

				for (int i = 0; i < availableDialogues.Length; i++)
				{
					int j = i;
					Color actual = availableDialogues[i].IsDone() ? playerChoiceDone : playerChoiceUndone;

					SpawnPlayerChoice(availableDialogues[i].playerQuestion, actual, j);
				}
			}
			else // previous line was player line
			{
				SpawnCharacterLine(actualCharacterDialogue.GetActualDialogue().characterAnswer, characterTextColor);
			}
		}
	}

	void ChangeCharacter(Character character)
	{
		ResetDialogue();

		actualCharacter = character;
		actualCharacterDialogue = actualDialogue.GetCharacterDialogue(actualCharacter);

		characterPortrait.sprite = GetCharacter(actualCharacter).characterPortrait;

		actualCharacterDialogue.Init();

		waitForPlayerChoice = false;

		SpawnCharacterLine(actualCharacterDialogue.firstLine, characterTextColor);
	}

	void SpawnPlayerChoice(string line, Color textColor, int index)
	{
		Button spawned = Instantiate(playerChoicePrefab, dialogueScrollList).GetComponent<Button>();

		TextMeshProUGUI spawnedText = spawned.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
		spawnedText.text = line;
		spawnedText.color = textColor;

		spawned.onClick.AddListener(() => SelectChoice(line, index));

		lastSpawnedDialogueObjects.Add(spawned.gameObject);

		waitForPlayerChoice = true;
	}

	void SelectChoice(string line, int index)
	{
		actualCharacterDialogue.MoveToDialogue(index);

		// deletes all previously spawned texts
		if(lastSpawnedDialogueObjects.Count > 0)
		{
			lastSpawnedDialogueObjects.ForEach(item => Destroy(item));
			lastSpawnedDialogueObjects.Clear();
		}

		SpawnPlayerLine(line, playerTextColor);

		needsPlayerSpawn = false;
		waitForPlayerChoice = false;
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

	void AddClueToList(Clue clue)
	{

		GameObject spawnedClue = Instantiate(cluePrefab, cluesScrollList);
		spawnedClue.transform.SetAsLastSibling();

		spawnedClue.GetComponent<TextMeshProUGUI>().text = "- " + clue.summary;
	}

	[Serializable]
	public class ShogunCharacter : IInitializable
	{
		public Character character;
		public Sprite characterPortrait;
		public Button selectionButton;

		public bool initialized => initializableInterface.initializedInternal;

		IInitializable initializableInterface => (IInitializable) this;

		bool IInitializable.initializedInternal { get; set; }

		void IInitializable.InitInternal()
		{
			initializableInterface.initializedInternal = true;
		}

		public void Init(Action changeCharacter)
		{
			selectionButton.onClick.RemoveAllListeners();
			selectionButton.onClick.AddListener(() => changeCharacter.Invoke());

			selectionButton.GetComponent<Image>().sprite = characterPortrait;

			initializableInterface.InitInternal();
		}
	}
}