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
	public float cluesAddDelay;
	public int highlightLength;
	public Color playerTextColor, playerChoiceDone, playerChoiceUndone, characterTextColor, highlightColor;
	[Space]
	public List<ShogunCharacter> characters;

	[Header("Assing in Inspector")]
	public Button openDeductionButton;
	public Button combatButton, cluesPanelButton;
	public RectTransform dialogueScrollList, cluesScrollList;
	public GameObject characterTextPrefab, playerChoicePrefab, playerTextPrefab, cluePrefab;
	public Image characterPortrait;
	public TextMeshProUGUI characterName;
	public Animator cluesPanelAnim;

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
	float cluesAddTimer;
	bool needsPlayerSpawn, waitForPlayerChoice, cluesOpen;

	public void PreInit()
	{
		characters.ForEach(item => item.Init(() => ChangeCharacter(item.character)));
	}

	// receives actions from GameManager
	public void Init(Action openDeductionPopup, Func<Clue, bool> findClue)
	{
		lastSpawnedDialogueObjects = new List<GameObject>();

		findClueEvent = findClue;
		cluesOpen = false;
		cluesAddTimer = 0;

		// plug in buttons
		openDeductionButton.onClick.AddListener(() => openDeductionPopup.Invoke());

		cluesPanelButton.onClick.AddListener(OpenCloseClues);

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
		if(cluesAddTimer > 0)
		{
			cluesAddTimer -= Time.deltaTime;

			if(cluesAddTimer <= 0)
			{
				cluesAddTimer = 0;
				cluesPanelAnim.Play("Close");
			}
		}

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
		else if(Input.GetMouseButtonDown(0))
		{
			lastWriter.Finish();
		}
	}

	void ChangeCharacter(Character character)
	{
		if(character != Character.SHOGUN && !actualDialogue.GetCharacterDialogue(Character.SHOGUN).IsDone())
			return;

		ResetDialogue();

		actualCharacter = character;
		actualCharacterDialogue = actualDialogue.GetCharacterDialogue(actualCharacter);

		characterPortrait.sprite = GetCharacter(actualCharacter).characterFull;
		characterName.text = GetCharacter(actualCharacter).name;

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
		DialogueWriter spawned = Instantiate(playerTextPrefab, dialogueScrollList).GetComponent<DialogueWriter>();

		spawned.Reset();
		spawned.Play(line, dialogueSpeed * 2, Mathf.RoundToInt(highlightLength * 1.5f), highlightColor, text);

		lastWriter = spawned;
	}

	void SpawnCharacterLine(string line, Color text)
	{
		if(string.IsNullOrEmpty(line))
		{
			needsPlayerSpawn = true;
			return;
		}

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

		spawnedClue.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = clue.summary;

		LayoutRebuilder.ForceRebuildLayoutImmediate(cluesScrollList);

		cluesAddTimer = cluesAddDelay;

		cluesPanelAnim.Play("Open");
	}

	void OpenCloseClues()
	{
		cluesOpen = !cluesOpen;

		if(cluesOpen)
			cluesPanelAnim.Play("Open");
		else
		{
			cluesAddTimer = 0;
			cluesPanelAnim.Play("Close");
		}
	}

	[Serializable]
	public class ShogunCharacter : IInitializable
	{
		public Character character;
		public Sprite characterPortrait, characterDetail, characterFull;
		public Button selectionButton;
		public string name;

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
			selectionButton.GetComponentInChildren<Image>().sprite = characterDetail;

			initializableInterface.InitInternal();
		}
	}
}