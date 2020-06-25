using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GeneralPunchlines;

// class managing gameplay of Fight panel
public class FightManager : MonoBehaviour, IDebugable, IInitializable
{
	[Header("Settings")]
	public float preCombatReplicaDelay;
	public float preCombatWriterSpeed, suicideDelay;
	public int preCombatWriterTrailLength, backgroundSvalue, backgroundVvalue;
	public Color preCombatWriterColor, preCombatWriterHighlight;
	public KeyCode skip;
	// 0 => Idle / 1 => attack / 2 => damage
	[Space]
	public Sprite[] playerSprites;

	[Header("Assign in Inspector")]
	public Animator canvasAnimator;
	public Animator effectAnimator, KanjiGeneralAnimator, KanjiFinisherAnimator, playerAnimator, enemyAnimator, bloodShed;
	public Transform playerDialoguePosition, enemyDialoguePosition;
	public DialogueWriter writerPrefab;
	[Space]
	public Image[] backgrounds;
	[Space]
	public Image enemyGraph;
	public Image bigEnemyGraph, playerGraph, bigPlayerGraph;
	public KatanaSlider katanaSlider;
	public GongSlider gongSlider;
	[Space]
	public List<CategoryButton> categoryButtons;
	[Space]
	public Button punchlinePrefab;
	[Space]
	public Button[] finisherPunchlineButtons;
	[Space]
	public ConclusionCard conclusionPrefab;
	public Transform punchlineScroll, conclusionScroll;
	public TextMeshProUGUI generalAttack, finisherAttack;
	public GameObject generalPunchlinePanel, finisherPunchlinePanel;

	[Header("Debug")]
	public CombatDialogue actualCombat;

	IInitializable initializableInterface => (IInitializable) this;
	IDebugable debugableInterface => (IDebugable) this;

	public bool initialized => initializableInterface.initializedInternal;

	bool IInitializable.initializedInternal { get; set; }
	string IDebugable.debugLabel => "<b>[FightManager] : </b>";

	enum Phase
	{
		INTRO,
		DIALOGUE,
		KATANA,
		GONG,
		CHOICE_GENERAL,
		EFFECT_GENERAL,
		CHOICE_FINAL,
		EFFECT_FINAL,
		SUICIDE,
		PLAYER_SUICIDE
	}

	Phase actualPhase;
	DialogueWriter lastWriter, lastWriterPlayer, lastWriterEnemy;
	Punchline selectedPunchline;
	GeneralPunchlines gamePunchlines;
	List<SubCategory> enemyHealth;
	List<Punchline> usedPunchlines;
	List<ConclusionCard> spawnedConclusions;
	Action toConsequencesWin, toConsequencesLost;
	string selectedFinisher;
	float preCombatTimer, suicideTimer;
	int dialogueIndex, triesCount, suicideIndex;
	bool isPlayer, writerIsCommanded, isGoodFinisher, gotToFinisher, invokedTransition, destructionAsked, isStartingGameOver, suicideStarted, useCheats;

	public void PreInit(CombatDialogue actualCombat)
	{
		for (int i = 0; i < backgrounds.Length; i++)
		{
			backgrounds[i].sprite = actualCombat.sceneBackgrounds[i];
			backgrounds[i].color = ColorTools.LerpColorValues(Skinning.GetSkin(backgrounds[i].GetComponent<SkinGraphic>().skin_tag), ColorTools.Value.SV, new int[2] { backgroundSvalue, backgroundVvalue });
			backgrounds[i].GetComponent<SkinGraphic>().enabled = false;
		}

		this.actualCombat = actualCombat;

		enemyHealth = new List<SubCategory>(actualCombat.weaknesses);
		enemyGraph.sprite = actualCombat.enemySprites[4 + enemyHealth.Count];
		bigEnemyGraph.sprite = enemyGraph.sprite;

		playerGraph.sprite = playerSprites[0];

		actualPhase = Phase.INTRO;

		canvasAnimator.Play("Intro");
		Invoke("StartDialogue", 3);
	}

	public void Init(bool useCheats, GeneralPunchlines punchlines, GeneralDialogue dialogue, Action toConsequencesWinCallback, Action toConsequencesLostCallback)
	{
		initializableInterface.InitInternal();

		this.useCheats = useCheats;

		gamePunchlines = punchlines;

		toConsequencesWin = toConsequencesWinCallback;
		toConsequencesLost = toConsequencesLostCallback;

		katanaSlider.gameObject.SetActive(false);
		gongSlider.Init(StartFinalPunchlines);

		categoryButtons.ForEach(item => item.Init(ShowPunchlines, gamePunchlines));

		spawnedConclusions = new List<ConclusionCard>();

		foreach (Conclusion conclusion in dialogue.unlockableConclusions)
		{
			ConclusionCard spawned = Instantiate(conclusionPrefab, conclusionScroll);
			spawned.Init(conclusion);
			spawned.ShowCard(false);

			spawnedConclusions.Add(spawned);
		}

		for (int i = 0; i < finisherPunchlineButtons.Length; i++)
		{
			finisherPunchlineButtons[i].transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = actualCombat.finisherPunchlines.finishers[i];

			int j = i;

			finisherPunchlineButtons[i].onClick.AddListener(() =>
			{
				selectedFinisher = actualCombat.finisherPunchlines.finishers[j];
				isGoodFinisher = j == actualCombat.finisherPunchlines.correctOne;

				canvasAnimator.Play("PanDown");
				Invoke("ShowAttackLines", 1);

				SetCanvasInterractable(false);
				actualPhase = Phase.EFFECT_FINAL;
			});

			ColorBlock colors = finisherPunchlineButtons[i].colors;
			colors.normalColor = Skinning.GetSkin(SkinTag.SECONDARY_WINDOW);
			colors.highlightedColor = Skinning.GetSkin(SkinTag.PRIMARY_WINDOW);
			colors.pressedColor = Skinning.GetSkin(SkinTag.CONTRAST);
			finisherPunchlineButtons[i].colors = colors;
		}

		triesCount = 0;
		isGoodFinisher = false;
		gotToFinisher = false;
		invokedTransition = false;
		destructionAsked = false;
		isStartingGameOver = false;

		usedPunchlines = new List<Punchline>();

		Debug.Log(debugableInterface.debugLabel + "Initializing done");
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;
	}

	void Update()
	{
		if(!initializableInterface.initialized)
			return;

		switch(actualPhase)
		{
			case Phase.DIALOGUE:
				if(Input.GetKeyDown(skip) && useCheats)
				{
					actualPhase = Phase.KATANA;
					Destroy(lastWriter.gameObject);

					playerDialoguePosition.gameObject.SetActive(false);
					enemyDialoguePosition.gameObject.SetActive(false);
					return;
				}

				if(lastWriter.isDone && !writerIsCommanded)
				{
					Invoke("SpawnNextPreDialogue", preCombatReplicaDelay * 2);
					writerIsCommanded = true;
					katanaSlider.slider.value = 0;
				}
				break;

			case Phase.KATANA:
				if(katanaSlider.slider.value == 1)
				{
					actualPhase = Phase.CHOICE_GENERAL;

					canvasAnimator.Play("PanUp");
				}

				if(Input.GetKeyDown(skip) && useCheats)
					enemyHealth.Clear();

				if(lastWriter != null && lastWriter.isDone && !destructionAsked)
				{
					Destroy(lastWriter.gameObject, preCombatReplicaDelay * 2);
					destructionAsked = true;
				}

				if(!enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("TakeDamage"))
				{
					enemyGraph.sprite = actualCombat.enemySprites[4 + enemyHealth.Count];

					if(!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("TakeDamage"))
					{
						playerGraph.sprite = playerSprites[0];

						katanaSlider.gameObject.SetActive(true);
					}
				}

				if(lastWriter == null && destructionAsked)
				{
					enemyDialoguePosition.gameObject.SetActive(false);
					destructionAsked = false;
				}
				break;

			case Phase.GONG:
				gongSlider.gameObject.SetActive(true);
				enemyDialoguePosition.gameObject.SetActive(false);

				if(lastWriter != null)
					Destroy(lastWriter.gameObject);
				break;

			case Phase.CHOICE_GENERAL:
				generalPunchlinePanel.SetActive(true);
				finisherPunchlinePanel.SetActive(false);

				enemyDialoguePosition.gameObject.SetActive(false);

				if(Input.GetKeyDown(skip) && useCheats)
					triesCount = actualCombat.tries;

				if(lastWriter != null)
					Destroy(lastWriter.gameObject);
				break;
			case Phase.CHOICE_FINAL:
				generalPunchlinePanel.SetActive(false);
				finisherPunchlinePanel.SetActive(true);

				if(lastWriter != null)
					Destroy(lastWriter.gameObject);
				break;

			case Phase.EFFECT_GENERAL:
				katanaSlider.gameObject.SetActive(false);
				destructionAsked = false;
				break;
			case Phase.EFFECT_FINAL:
				gongSlider.gameObject.SetActive(false);
				destructionAsked = false;
				break;

			case Phase.PLAYER_SUICIDE:
				if(lastWriter.isDone && !invokedTransition)
					Invoke("PlayerSuicideAnimation", preCombatReplicaDelay);
				break;
			case Phase.SUICIDE:
				if(lastWriter.isDone && !suicideStarted)
				{
					EnemySuicideAnimation();
					suicideStarted = true;
				}
				break;
		}
	}

	void StartDialogue()
	{
		actualPhase = Phase.DIALOGUE;
		dialogueIndex = 0;

		if(actualCombat.actualState != GameData.GameState.NORMAL)
			isPlayer = false;
		else
			isPlayer = !string.IsNullOrWhiteSpace(actualCombat.preCombatReplicas[0].playerLine);

		SpawnNextPreDialogue();
	}

	void SpawnNextPreDialogue()
	{
		if(isPlayer)
		{
			if(lastWriterPlayer != null)
			{
				playerDialoguePosition.gameObject.SetActive(false);
				Destroy(lastWriterPlayer.gameObject);
			}

			if(actualCombat.actualState != GameData.GameState.NORMAL)
			{
				if(actualCombat.actualState == GameData.GameState.GAME_OVER_FINISHER)
					actualPhase = Phase.GONG;
				else
					actualPhase = Phase.KATANA;

				if(lastWriter != null)
				{
					playerDialoguePosition.gameObject.SetActive(false);
					enemyDialoguePosition.gameObject.SetActive(false);

					Destroy(lastWriter.gameObject);
				}
				return;
			}
			else
			{
				if(dialogueIndex > actualCombat.preCombatReplicas.Count - 1 || string.IsNullOrEmpty(actualCombat.preCombatReplicas[dialogueIndex].playerLine))
				{
					actualPhase = Phase.KATANA;

					playerDialoguePosition.gameObject.SetActive(false);
					enemyDialoguePosition.gameObject.SetActive(false);

					Destroy(lastWriter.gameObject);
					return;
				}

				playerDialoguePosition.gameObject.SetActive(true);
				lastWriter = Instantiate(writerPrefab, playerDialoguePosition.GetChild(1));

				lastWriter.SetAudio(() => AudioManager.PlaySound("Writting"), () => AudioManager.StopSound("Writting"));
				lastWriter.Play(actualCombat.preCombatReplicas[dialogueIndex].playerLine, preCombatWriterSpeed, preCombatWriterTrailLength, preCombatWriterHighlight, preCombatWriterColor);
			}

			lastWriterPlayer = lastWriter;
		}
		else
		{
			if(lastWriterEnemy != null)
			{
				enemyDialoguePosition.gameObject.SetActive(false);
				Destroy(lastWriterEnemy.gameObject);
			}

			if(actualCombat.actualState != GameData.GameState.NORMAL)
			{
				enemyDialoguePosition.gameObject.SetActive(true);
				lastWriter = Instantiate(writerPrefab, enemyDialoguePosition.GetChild(1));

				lastWriter.SetAudio(() => AudioManager.PlaySound("Writting"), () => AudioManager.StopSound("Writting"));
				lastWriter.Play(actualCombat.preCombatReturnReplica, preCombatWriterSpeed, preCombatWriterTrailLength, preCombatWriterHighlight, preCombatWriterColor);
			}
			else
			{
				if(dialogueIndex > actualCombat.preCombatReplicas.Count - 1 || string.IsNullOrEmpty(actualCombat.preCombatReplicas[dialogueIndex].enemyLine))
				{
					actualPhase = Phase.KATANA;

					enemyDialoguePosition.gameObject.SetActive(false);
					playerDialoguePosition.gameObject.SetActive(false);

					Destroy(lastWriter.gameObject);
					return;
				}

				enemyDialoguePosition.gameObject.SetActive(true);
				lastWriter = Instantiate(writerPrefab, enemyDialoguePosition.GetChild(1));

				lastWriter.SetAudio(() => AudioManager.PlaySound("Writting"), () => AudioManager.StopSound("Writting"));
				lastWriter.Play(actualCombat.preCombatReplicas[dialogueIndex].enemyLine, preCombatWriterSpeed, preCombatWriterTrailLength, preCombatWriterHighlight, preCombatWriterColor);
			}

			dialogueIndex++;

			lastWriterEnemy = lastWriter;
		}

		isPlayer = !isPlayer;
		writerIsCommanded = false;
	}

	void ShowPunchlines(List<Punchline> punchlines)
	{
		foreach (Transform transform in punchlineScroll)
			Destroy(transform.gameObject);

		foreach (Punchline punchline in punchlines)
		{
			Button temp = Instantiate(punchlinePrefab, punchlineScroll);
			temp.onClick.AddListener(() =>
			{
				usedPunchlines.Add(punchline);
				selectedPunchline = punchline;

				triesCount++;
				actualPhase = Phase.EFFECT_GENERAL;

				canvasAnimator.Play("PanDown");
				Invoke("ShowAttackLines", 1);

				ConclusionCard selected = null;

				spawnedConclusions.ForEach(item =>
				{
					if(item.IsCorrelated(punchline))
						selected = item;
				});

				if(selected != null)
				{
					spawnedConclusions.Remove(selected);
					Destroy(selected.gameObject);
				}

				SetCanvasInterractable(false);

				foreach (Transform transform in punchlineScroll)
					Destroy(transform.gameObject);
			});

			if(string.IsNullOrEmpty(punchline.line))
				temp.transform.GetChild(2).GetChild(0).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = punchline.subCategory.ToString();
			else
				temp.transform.GetChild(2).GetChild(0).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = punchline.line;

			Color categoryColor = GameData.GetColorFromCategory(GameData.GetCategoryFromSubCategory(punchline.subCategory));

			ColorBlock colors = temp.colors;
			colors.normalColor = Skinning.GetSkin(SkinTag.SECONDARY_WINDOW);
			colors.pressedColor = categoryColor;
			colors.highlightedColor = ColorTools.LerpColorValues(categoryColor, ColorTools.Value.SV, new int[2] {-20, -30 });
			colors.disabledColor = Skinning.GetSkin(SkinTag.PRIMARY_WINDOW);
			temp.colors = colors;

			temp.interactable = !usedPunchlines.Contains(punchline);

			temp.GetComponent<Animator>().Play("Open");
		}
	}

	void StartFinalPunchlines()
	{
		actualPhase = Phase.CHOICE_FINAL;

		canvasAnimator.Play("PanUp");
	}

	void ShowAttackLines()
	{
		effectAnimator.Play("OpenLines");

		playerGraph.sprite = playerSprites[1];

		Invoke("AttackEffect", 0.3f);
	}

	void HideAttackLines()
	{
		KanjiGeneralAnimator.Play("Hide");
		KanjiFinisherAnimator.Play("Hide");

		effectAnimator.Play("CloseLines");

		if(actualPhase == Phase.EFFECT_FINAL)
		{
			if(isGoodFinisher)
			{
				suicideIndex = 1;
				suicideTimer = 0;

				Invoke("EnemySuicideAnimation", 0.15f);
				return;
			}
			else
			{
				isStartingGameOver = true;
				Invoke("ShowGameOver", 0.15f);
			}
		}
		else // if(actualPhase == Phase.EFFECT_GENERAL)
		{
			if(enemyHealth.Count > 0)
			{
				if(triesCount >= actualCombat.tries)
				{
					isStartingGameOver = true;
					Invoke("ShowGameOver", 0.15f);
				}
				else
					Invoke("StartKatanaAgain", 0.15f);
			}
			else
				Invoke("StartGong", 0.15f);
		}
	}

	void StartKatanaAgain()
	{
		effectAnimator.Play("Empty");

		katanaSlider.slider.value = 0;
		actualPhase = Phase.KATANA;

		SetCanvasInterractable(true);
	}

	void StartGong()
	{
		effectAnimator.Play("Empty");

		gongSlider.SetInitialValue();
		actualPhase = Phase.GONG;
		gotToFinisher = true;

		SetCanvasInterractable(true);
	}

	void ShowGameOver()
	{
		effectAnimator.Play("Empty");

		playerGraph.sprite = playerSprites[0];

		enemyDialoguePosition.gameObject.SetActive(true);
		lastWriter = Instantiate(writerPrefab, enemyDialoguePosition.GetChild(1));
		lastWriter.SetAudio(() => AudioManager.PlaySound("Writting"), () => AudioManager.StopSound("Writting"));

		if(actualPhase == Phase.EFFECT_GENERAL)
			lastWriter.Play(actualCombat.playerLoseResponse, preCombatWriterSpeed, preCombatWriterTrailLength, preCombatWriterHighlight, preCombatWriterColor);

		if(actualPhase == Phase.EFFECT_FINAL)
			lastWriter.Play(actualCombat.playerFinisherLoseResponse, preCombatWriterSpeed, preCombatWriterTrailLength, preCombatWriterHighlight, preCombatWriterColor);

		actualPhase = Phase.PLAYER_SUICIDE;
	}

	void AttackEffect()
	{
		switch(actualPhase)
		{
			case Phase.EFFECT_GENERAL:
				KanjiFinisherAnimator.Play("Hide");
				KanjiGeneralAnimator.Play("ShowAttack");

				generalAttack.text = selectedPunchline.line;

				Invoke("HideAttackLines", 4.45f);

				bool takesDamage = false;

				foreach (SubCategory subCategory in enemyHealth)
				{
					if(selectedPunchline.subCategory == subCategory)
						takesDamage = true;
				}

				if(takesDamage)
				{
					Invoke("GeneralAttackGood", 2.25f);
					Invoke("EnnemyTakeDamage", 4.45f);
					enemyHealth.Remove(selectedPunchline.subCategory);
				}
				else
				{
					Invoke("GeneralAttackFail", 2.25f);
					Invoke("PlayerTakeDamage", 4.45f);
					Invoke("SpawnFailReaction", 4.45f + 1.15f);
				}
				break;

			case Phase.EFFECT_FINAL:
				KanjiFinisherAnimator.Play("ShowAttack");
				KanjiGeneralAnimator.Play("Hide");

				finisherAttack.text = selectedFinisher;

				Invoke("HideAttackLines", 11.10f);

				if(isGoodFinisher)
					Invoke("EnnemyTakeDamage", 11.10f);
				break;
		}
	}

	void EnnemyTakeDamage()
	{
		enemyAnimator.Play("TakeDamage");
		enemyGraph.sprite = actualCombat.enemySprites[0];
		bigEnemyGraph.sprite = enemyGraph.sprite;

		if(actualPhase == Phase.EFFECT_GENERAL)
			Invoke("SpawnDamageReaction", 1.10f);
	}

	void PlayerTakeDamage()
	{
		playerAnimator.Play("TakeDamage");
		playerGraph.sprite = playerSprites[2];
		bigPlayerGraph.sprite = playerGraph.sprite;
	}

	void SpawnAttackReaction(string reaction)
	{
		enemyDialoguePosition.gameObject.SetActive(true);
		lastWriter = Instantiate(writerPrefab, enemyDialoguePosition.GetChild(1));

		lastWriter.SetAudio(() => AudioManager.PlaySound("Writting"), () => AudioManager.StopSound("Writting"));
		lastWriter.Play(reaction, preCombatWriterSpeed, preCombatWriterTrailLength, preCombatWriterHighlight, preCombatWriterColor);
	}

	void SpawnDamageReaction()
	{
		SpawnAttackReaction(gamePunchlines.GetRandomDamageReaction());
	}

	void SpawnFailReaction()
	{
		if(!isStartingGameOver)
			SpawnAttackReaction(gamePunchlines.GetRandomFailReaction());
	}

	void GeneralAttackGood()
	{
		KanjiGeneralAnimator.Play("GoodAttack");
	}

	void GeneralAttackFail()
	{
		KanjiGeneralAnimator.Play("BadAttack");
	}

	void EnemySuicideAnimation()
	{
		actualPhase = Phase.SUICIDE;

		if(lastWriter == null)
		{
			enemyDialoguePosition.gameObject.SetActive(true);
			lastWriter = Instantiate(writerPrefab, enemyDialoguePosition.GetChild(1));

			lastWriter.SetAudio(() => AudioManager.PlaySound("Writting"), () => AudioManager.StopSound("Writting"));
			lastWriter.Play(actualCombat.playerWinResponse, preCombatWriterSpeed, preCombatWriterTrailLength, preCombatWriterHighlight, preCombatWriterColor);

			return;
		}

		effectAnimator.Play("Empty");
		enemyAnimator.enabled = false;

		enemyGraph.sprite = actualCombat.enemySprites[suicideIndex];
		bigEnemyGraph.sprite = enemyGraph.sprite;

		if(suicideIndex != 3)
		{
			Invoke("EnemySuicideAnimation", suicideDelay);
			suicideIndex++;
		}
		else
			PlayBloodShed();
	}

	void PlayerSuicideAnimation()
	{
		Invoke("PlayBloodShed", 1);

		invokedTransition = true;
	}

	void PlayBloodShed()
	{
		switch(actualPhase)
		{
			case Phase.PLAYER_SUICIDE:
				playerGraph.sprite = playerSprites[2];
				playerAnimator.enabled = false;
				bloodShed.Play("Idle");
				break;
			case Phase.SUICIDE:
				bloodShed.gameObject.SetActive(true);
				bloodShed.Play("Enemy");
				break;
		}

		bloodShed.GetComponent<AnimationSystemUI>().PlayClip("Play");
		Invoke("FadeToBlack", 2);
	}

	void FadeToBlack()
	{
		// bloodShed.gameObject.SetActive(false);

		effectAnimator.Play("Fade");

		switch(actualPhase)
		{
			case Phase.PLAYER_SUICIDE:
				if(gotToFinisher)
					Invoke("GameOverFinisher", 2);
				else
					Invoke("GameOverGeneral", 2);
				break;
			case Phase.SUICIDE:
				Invoke("MoveToConsequences", 2);
				break;
		}
	}

	void MoveToConsequences()
	{
		toConsequencesWin.Invoke();
	}

	void GameOverGeneral()
	{
		actualCombat.actualState = GameData.GameState.GAME_OVER_GENERAL;
		toConsequencesLost.Invoke();
	}

	void GameOverFinisher()
	{
		actualCombat.actualState = GameData.GameState.GAME_OVER_FINISHER;
		toConsequencesLost.Invoke();
	}

	void SetCanvasInterractable(bool state)
	{
		canvasAnimator.GetComponent<CanvasGroup>().interactable = state;
	}

	[Serializable]
	public class CategoryButton
	{
		public Category category;
		public Button button;

		public void Init(Action<List<Punchline>> showCallback, GeneralPunchlines gamePunchlines)
		{
			List<Punchline> temp = gamePunchlines.allPunchlines.Find(item => { return item.category == category; }).punchlines;
			button.onClick.AddListener(() => showCallback.Invoke(temp));

			button.targetGraphic.color = GameData.GetColorFromCategory(category);
		}
	}
}