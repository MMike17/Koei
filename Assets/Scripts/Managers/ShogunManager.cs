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
	public float cluesAddDelay, characterMoveDelay;
	public int highlightLength;
	public string[] characterAnimationTag;
	[Space]
	public SkinTag playerTextColor;
	public SkinTag playerHighlightColor;
	[Space]
	public SkinTag playerChoiceDone;
	public SkinTag playerChoiceUndone, playerChoiceHighlightColor;
	[Space]
	public SkinTag characterHighlightColor;
	[Space]
	public List<ShogunCharacter> characters;
	[Space]
	public KeyCode unlockAllClues;

	[Header("Assing in Inspector")]
	public Button openDeductionButton;
	public Button cluesPanelShadowButton;
	public RectTransform dialogueScrollList, cluesScrollList;
	public Scrollbar dialogueScroll;
	public GameObject characterTextPrefab, playerChoicePrefab, cluePrefab, scrollUp, scrollDown;
	public UICharacter characterPortrait;
	public TextMeshProUGUI characterName;
	public Animator cluesPanelAnim;
	public Image dialogueScrollDetail, background;
	public ScrollButton scrollButton;

	IDebugable debugableInterface => (IDebugable) this;
	IInitializable initializableInterface => (IInitializable) this;

	public bool initialized => initializableInterface.initializedInternal;

	// for testing
	public bool forceClick { get; set; }
	public bool waitForPlayerChoice { get; private set; }
	public bool cluesOpen { get; private set; }
	public bool characterDone { get; private set; }
	public DialogueWriter lastWriter { get; private set; }
	public List<GameObject> lastSpawnedDialogueObjects { get; private set; }
	public GeneralDialogue actualDialogue { get; private set; }
	public Character actualCharacter { get; private set; }

	bool IInitializable.initializedInternal { get; set; }
	string IDebugable.debugLabel => "<b>[ShogunManager] : </b>";

	CharacterDialogue actualCharacterDialogue;

	Func<Clue, bool> findClueEvent;
	Color characterTextColor;
	GameData.GameState actualState;
	Character lastAnimated;
	AudioProjectManager audioProject;
	float cluesAddTimer, characterMoveTimer;
	bool needsPlayerSpawn, didCheat, useCheats, firstCharacterSelection;

	public void PreInit(GameData.GameState targetState, GeneralDialogue dialogue, Func<Clue, bool> findClue)
	{
		findClueEvent = findClue;

		background.color = ColorTools.LerpColorValues(Skinning.GetSkin(SkinTag.PICTO), ColorTools.Value.SV, new int[2] { 8, 10 });

		characterPortrait.Hide();
		characters.ForEach(item => item.Init(() => ChangeCharacter(item.character)));

		ColorBlock block = dialogueScroll.colors;
		block.normalColor = Skinning.GetSkin(SkinTag.SECONDARY_ELEMENT);
		block.highlightedColor = Skinning.GetSkin(SkinTag.SECONDARY_WINDOW);
		block.pressedColor = Skinning.GetSkin(SkinTag.CONTRAST);
		dialogueScroll.colors = block;

		scrollButton.Init(this);

		block = scrollButton.button.colors;
		block.normalColor = Skinning.GetSkin(SkinTag.PRIMARY_ELEMENT);
		block.highlightedColor = Skinning.GetSkin(SkinTag.PRIMARY_WINDOW);
		block.pressedColor = Skinning.GetSkin(SkinTag.CONTRAST);
		block.disabledColor = block.normalColor;
		scrollButton.button.colors = block;

		// when you come back to shogun after defeat
		if(targetState != GameData.GameState.NORMAL)
		{
			// unlocks all clues
			foreach (Clue clue in dialogue.GetAllClues())
			{
				findClue.Invoke(clue);
				AddClueToList(clue, true);
			}
		}

		actualState = targetState;
	}

	// receives actions from GameManager
	public void Init(bool useCheats, Action openDeductionPopup, Action popupMusicFadeIn)
	{
		lastSpawnedDialogueObjects = new List<GameObject>();

		this.useCheats = useCheats;
		cluesOpen = false;
		didCheat = false;
		characterDone = false;
		firstCharacterSelection = false;
		cluesAddTimer = 0;
		characterMoveTimer = 0;

		// plug in buttons
		scrollButton.button.interactable = false;
		openDeductionButton.onClick.AddListener(() =>
		{
			openDeductionPopup.Invoke();
			OpenCloseClues();
			AudioManager.PlaySound("Rip", popupMusicFadeIn);
		});

		scrollButton.button.onClick.AddListener(OpenCloseClues);
		cluesPanelShadowButton.onClick.AddListener(OpenCloseClues);

		initializableInterface.InitInternal();
	}

	public void StartDialogue(GeneralDialogue dialogue)
	{
		if(!initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "Not initialized");
			return;
		}

		scrollButton.button.interactable = true;
		actualDialogue = dialogue;

		characterPortrait.Show();
		ChangeCharacter(Character.SHOGUN);

		firstCharacterSelection = false;

		characters.ForEach(item => item.UI.Grey(true));
	}

	public void ResetDialogue()
	{
		// deletes all previously spawned texts
		if(lastSpawnedDialogueObjects.Count > 0)
		{
			GameObject[] toDestroy = lastSpawnedDialogueObjects.ToArray();

			for (int i = 0; i < toDestroy.Length; i++)
				Destroy(toDestroy[i]);

			lastSpawnedDialogueObjects.Clear();
		}

		foreach (Transform child in dialogueScrollList)
			Destroy(child.gameObject);

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
		// sets character grey state
		if(actualDialogue != null && actualDialogue.GetCharacterDialogue(Character.SHOGUN).IsDone())
		{
			if(!firstCharacterSelection && !cluesPanelAnim.GetCurrentAnimatorStateInfo(0).IsName("Tutorial"))
				cluesPanelAnim.Play("ShowTutorial");

			List<UICharacter> notGreyed = new List<UICharacter>();

			characters.ForEach(item =>
			{
				if(item.character != actualCharacter)
				{
					item.selectionButton.interactable = true;
					item.UI.Grey(false);
				}

				if(!item.UI.IsGreyed())
					notGreyed.Add(item.UI);
			});

			// if shogun dialogue is already done
			if(notGreyed.Count > 1)
				characterMoveTimer += Time.deltaTime;

			// animates characters
			if(characterMoveTimer >= characterMoveDelay)
			{
				characterMoveTimer = 0;

				// select random character except shogun
				List<ShogunCharacter> allButShogunAndSelected = new List<ShogunCharacter>();

				foreach (ShogunCharacter shogunCharacter in characters)
				{
					if(shogunCharacter.character != Character.SHOGUN && shogunCharacter.character != actualCharacter && shogunCharacter.character != lastAnimated)
						allButShogunAndSelected.Add(shogunCharacter);
				}

				int index = UnityEngine.Random.Range(0, allButShogunAndSelected.Count);

				// select random animation
				int animationIndex = UnityEngine.Random.Range(0, characterAnimationTag.Length);

				// give random animation to random character
				allButShogunAndSelected[index].UI.movementAnimator.Play(characterAnimationTag[animationIndex]);

				// stores last animated
				lastAnimated = allButShogunAndSelected[index].character;
			}
		}

		scrollDown.SetActive(cluesScrollList.anchoredPosition.y < cluesScrollList.rect.height / 2);
		scrollUp.SetActive(cluesScrollList.anchoredPosition.y > cluesScrollList.parent.GetComponent<RectTransform>().rect.height - (cluesScrollList.rect.height / 2));

		// unlock all cheat
		if(Input.GetKeyDown(unlockAllClues) && !didCheat && useCheats)
		{
			foreach (Clue clue in actualDialogue.GetAllClues())
			{
				if(findClueEvent.Invoke(clue))
					AddClueToList(clue);
			}

			Debug.Log(debugableInterface.debugLabel + "Unlocked all clues");

			didCheat = true;
		}

		// timer for clues panel
		if(cluesAddTimer > 0)
		{
			cluesAddTimer -= Time.deltaTime;

			if(cluesAddTimer <= 0)
			{
				cluesAddTimer = 0;
				cluesOpen = false;

				cluesPanelAnim.Play("Close");
				AudioManager.PlaySound("CluesPanel");
			}
		}

		// when character writer is done
		if(lastWriter != null && lastWriter.isDone)
		{
			// prevents update from spawning lines if we are waiting for the player to choose a dialogue line
			if(waitForPlayerChoice)
				return;

			characterDone = true;

			// previous line was a character line
			if(needsPlayerSpawn)
			{
				{
					// sets shogun dialogue done if return dialogue done
					CharacterDialogue selected = actualDialogue.charactersDialogues.Find(item => { return item.character == Character.SHOGUN; });

					if(actualDialogue.shogunReturnDialogues.IsDone() && !selected.IsDone())
						actualDialogue.charactersDialogues.Find(item => { return item.character == Character.SHOGUN; }).ForceSetAsDone();
				}

				// spawns next dialogues choices
				if(Input.GetMouseButtonDown(0) || forceClick)
				{
					forceClick = false;

					Dialogue[] availableDialogues;

					// if we are not at beginning of dialogue
					if(actualCharacterDialogue.indexesPath.Count > 0)
					{
						// adds clue to player clue list if there was one
						if(actualCharacterDialogue.indexesPath.Count > 1 && actualCharacterDialogue.GetActualDialogue().hasClue)
						{
							if(findClueEvent.Invoke(actualCharacterDialogue.GetActualDialogue().clue))
								AddClueToList(actualCharacterDialogue.GetActualDialogue().clue);
						}

						availableDialogues = actualCharacterDialogue.GetActualDialogue().nextDialogues;

						// resets indexes path if branch can't go any further
						if(availableDialogues == null || availableDialogues.Length == 0)
						{
							actualCharacterDialogue.ResetDialoguePath();
							return;
						}
					}
					else
						availableDialogues = actualCharacterDialogue.initialDialogues.ToArray();

					for (int i = 0; i < availableDialogues.Length; i++)
					{
						int j = i;
						Color actual = Skinning.GetSkin(availableDialogues[i].IsDone() ? playerChoiceDone : playerChoiceUndone);
						string line = availableDialogues[i].playerQuestion;

						if(availableDialogues[i].IsDone())
							line = "<s>" + line + "</s>";

						SpawnPlayerChoice(line, availableDialogues[i].playerQuestion, actual, j);
					}
				}
			}
			else // previous line was player line
				SpawnCharacterLine(actualCharacterDialogue.GetActualDialogue().characterAnswer, characterTextColor);
		}
		else if(Input.GetMouseButtonDown(0) || forceClick)
		{
			forceClick = false;

			if(lastWriter != null)
				lastWriter.Finish();
		}
	}

	void ChangeCharacter(Character character)
	{
		firstCharacterSelection = true;
		cluesPanelAnim.Play("Idle");

		ResetDialogue();

		characters.ForEach(item =>
		{
			if(item.character == character)
				item.UI.Grey(true);
			else
				item.UI.Grey(false);
		});

		if(actualCharacterDialogue != null)
			actualCharacterDialogue.ResetDialoguePath();

		actualCharacter = character;

		// select shogun return when needed
		if(actualState != GameData.GameState.NORMAL && character == Character.SHOGUN)
			actualCharacterDialogue = actualDialogue.shogunReturnDialogues;
		else // select normal in other cases
			actualCharacterDialogue = actualDialogue.GetCharacterDialogue(actualCharacter);

		characterPortrait.SetCharacterPortrait(GetCharacter(actualCharacter));
		characterPortrait.detail.color = GameData.GetColorFromCharacter(actualCharacter);

		characterName.text = GetCharacter(actualCharacter).name;
		characterName.color = GameData.GetColorFromCharacter(actualCharacter);

		dialogueScrollDetail.color = GameData.GetColorFromCharacter(actualCharacter);

		ColorBlock colors = dialogueScroll.colors;
		colors.pressedColor = GameData.GetColorFromCharacter(actualCharacter);
		dialogueScroll.colors = colors;

		waitForPlayerChoice = false;
		characterTextColor = GameData.GetColorFromCharacter(character);

		SpawnCharacterLine(actualCharacterDialogue.firstLine, characterTextColor);
	}

	void SpawnPlayerChoice(string selectionLine, string displayLine, Color textColor, int index)
	{
		Button spawned = Instantiate(playerChoicePrefab, dialogueScrollList).GetComponent<Button>();

		ColorBlock block = spawned.colors;
		block.normalColor = textColor;
		block.highlightedColor = Skinning.GetSkin(playerChoiceHighlightColor);
		block.pressedColor = Skinning.GetSkin(SkinTag.CONTRAST);
		spawned.colors = block;

		TextMeshProUGUI spawnedText = spawned.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
		spawnedText.text = selectionLine;
		spawnedText.color = Color.white;

		spawned.onClick.AddListener(() => SelectChoice(displayLine, index));

		lastSpawnedDialogueObjects.Add(spawned.gameObject);

		waitForPlayerChoice = true;
	}

	void SelectChoice(string line, int index)
	{
		actualCharacterDialogue.MoveToDialogue(index);

		AudioManager.PlaySound("Knob");

		// deletes all previously spawned texts except selected one
		for (int i = 0; i < lastSpawnedDialogueObjects.Count; i++)
		{
			if(i != index)
				Destroy(lastSpawnedDialogueObjects[i]);
			else
				lastSpawnedDialogueObjects[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Skinning.GetSkin(playerTextColor);
		}

		lastSpawnedDialogueObjects.Clear();

		needsPlayerSpawn = false;
		waitForPlayerChoice = false;
	}

	void SpawnCharacterLine(string line, Color text)
	{
		if(string.IsNullOrEmpty(line))
		{
			needsPlayerSpawn = true;
			return;
		}

		DialogueWriter spawned = Instantiate(characterTextPrefab, dialogueScrollList).GetComponent<DialogueWriter>();

		spawned.SetAudio(() => AudioManager.PlaySound("Writting"), () => AudioManager.StopSound("Writting"));
		spawned.Play(line, dialogueSpeed, highlightLength, Skinning.GetSkin(characterHighlightColor), GameData.LerpColorHSV(text, 0, 0.1f, -0.3f));

		lastWriter = spawned;
		needsPlayerSpawn = true;
		characterDone = false;
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

	void AddClueToList(Clue clue, bool cancelAnimation = false)
	{
		GameObject spawnedClue = Instantiate(cluePrefab, cluesScrollList);
		spawnedClue.transform.SetAsLastSibling();

		spawnedClue.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = clue.summary;

		LayoutRebuilder.ForceRebuildLayoutImmediate(cluesScrollList);

		cluesAddTimer = cluesAddDelay;

		if(!cancelAnimation)
		{
			cluesPanelAnim.Play("Open");
			cluesOpen = true;
			AudioManager.PlaySound("CluesPanel");
		}
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

		AudioManager.PlaySound("CluesPanel");
	}

	public Dialogue ReturnDialogueFromLine(string line)
	{
		return actualCharacterDialogue.FindDialogueWithLine(line);
	}

	[Serializable]
	public class ShogunCharacter : IInitializable
	{
		public Character character;
		public Sprite characterClothes, characterSkin, characterDetail, characterEyes, characterOver;
		public UICharacter UI;
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
			selectionButton.interactable = false;

			selectionButton.onClick.RemoveAllListeners();
			selectionButton.onClick.AddListener(() =>
			{
				changeCharacter.Invoke();
				UI.SwitchState();
				AudioManager.PlaySound("Button");
			});

			UI.SetCharacterPortrait(this, selectionButton);

			initializableInterface.InitInternal();
		}
	}
}