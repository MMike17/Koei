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
	public SkinTag preCombatPlayerColor, preCombatPlayerHighlight, preCombatEnnemyColor, preCombatEnnemyHighlight;
	public TMP_FontAsset streetFont;
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
	public Transform punchlineScroll, conclusionScroll, triesTopHolder, triesBottomHolder;
	public TextMeshProUGUI generalAttack, finisherAttack;
	public GameObject generalPunchlinePanel, finisherPunchlinePanel, lifePrefab;

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
	List<Animator> lifePoints;
	TMP_FontAsset normalFont;
	Action toConsequences;
	string selectedFinisher;
	float preCombatTimer, suicideTimer;
	int dialogueIndex, triesCount, suicideIndex, playerTargetIndex;
	bool isPlayer, writerIsCommanded, isGoodFinisher, gotToFinisher, invokedTransition, destructionAsked, isStartingGameOver, suicideStarted, useCheats;

	public void PreInit(CombatDialogue actualCombat)
	{
		// assign backgrounds
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

		if(actualCombat.actualState == GameData.GameState.GAME_OVER_FINISHER)
		{
			enemyHealth.Clear();
			enemyGraph.sprite = actualCombat.enemySprites[4];
		}
	}

	public void Init(bool useCheats, GeneralPunchlines punchlines, GeneralDialogue dialogue, Action toConsequencesCallback)
	{
		normalFont = FindObjectOfType<TextMeshProUGUI>().font;
		initializableInterface.InitInternal();

		this.useCheats = useCheats;
		gamePunchlines = punchlines;
		toConsequences = toConsequencesCallback;

		katanaSlider.gameObject.SetActive(false);
		gongSlider.Init(StartFinalPunchlines);

		lifePoints = new List<Animator>();

		// spawns life points
		for (int i = 0; i < actualCombat.tries; i++)
		{
			Animator spawned = null;

			if(triesTopHolder.childCount < 3)
				spawned = Instantiate(lifePrefab, triesTopHolder).GetComponent<Animator>();
			else
				spawned = Instantiate(lifePrefab, triesBottomHolder).GetComponent<Animator>();

			lifePoints.Add(spawned);
		}

		categoryButtons.ForEach(item => item.Init(ShowPunchlines, gamePunchlines, streetFont));

		spawnedConclusions = new List<ConclusionCard>();

		// spawns conclusions
		foreach (Conclusion conclusion in dialogue.unlockableConclusions)
		{
			ConclusionCard spawned = Instantiate(conclusionPrefab, conclusionScroll);
			spawned.Init(conclusion);

			spawned.comment.font = streetFont;

			spawnedConclusions.Add(spawned);
		}

		// finisher buttons
		for (int i = 0; i < finisherPunchlineButtons.Length; i++)
		{
			finisherPunchlineButtons[i].transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = actualCombat.finisherPunchlines.finishers[i];
			finisherPunchlineButtons[i].transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().font = streetFont;

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

		canvasAnimator.Play("Intro");
		Invoke("StartDialogue", 3);

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

				if(!enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("TakeDamage"))
				{
					enemyGraph.sprite = actualCombat.enemySprites[4 + enemyHealth.Count];

					if(!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("TakeDamage"))
						playerGraph.sprite = playerSprites[0];
				}

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
				if(!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("TakeDamage"))
					playerGraph.sprite = playerSprites[playerTargetIndex];
				else
					playerGraph.sprite = playerSprites[2];

				if(lastWriter.isDone && !invokedTransition)
					Invoke("PlayerSuicideAnimation", preCombatReplicaDelay);
				break;
			case Phase.SUICIDE:
				if(!lastWriter.isDone)
				{
					if(!enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("TakeDamage"))
					{
						enemyGraph.sprite = actualCombat.enemySprites[4 + enemyHealth.Count];

						if(!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("TakeDamage"))
							playerGraph.sprite = playerSprites[0];
					}
				}
				else if(!suicideStarted)
				{
					playerGraph.sprite = playerSprites[0];

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

		if(actualCombat.actualState == GameData.GameState.GAME_OVER_FINISHER)
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

			if(actualCombat.actualState == GameData.GameState.GAME_OVER_FINISHER)
			{
				actualPhase = Phase.GONG;

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

				lastWriter.GetComponent<TextMeshProUGUI>().font = streetFont;
				lastWriter.SetAudio(() => AudioManager.PlaySound("Writting"), () => AudioManager.StopSound("Writting"));
				lastWriter.Play(actualCombat.preCombatReplicas[dialogueIndex].playerLine, preCombatWriterSpeed, preCombatWriterTrailLength, Skinning.GetSkin(preCombatPlayerHighlight), Skinning.GetSkin(preCombatPlayerColor));
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

			if(actualCombat.actualState == GameData.GameState.GAME_OVER_FINISHER)
			{
				enemyDialoguePosition.gameObject.SetActive(true);
				lastWriter = Instantiate(writerPrefab, enemyDialoguePosition.GetChild(1));

				lastWriter.GetComponent<TextMeshProUGUI>().font = normalFont;
				lastWriter.SetAudio(() => AudioManager.PlaySound("Writting"), () => AudioManager.StopSound("Writting"));
				lastWriter.Play(actualCombat.preCombatReturnReplica, preCombatWriterSpeed, preCombatWriterTrailLength, Skinning.GetSkin(preCombatEnnemyHighlight), Skinning.GetSkin(preCombatEnnemyColor));
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

				lastWriter.GetComponent<TextMeshProUGUI>().font = normalFont;
				lastWriter.SetAudio(() => AudioManager.PlaySound("Writting"), () => AudioManager.StopSound("Writting"));
				lastWriter.Play(actualCombat.preCombatReplicas[dialogueIndex].enemyLine, preCombatWriterSpeed, preCombatWriterTrailLength, Skinning.GetSkin(preCombatEnnemyHighlight), Skinning.GetSkin(preCombatEnnemyColor));
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
			temp.transform.GetChild(0).GetComponent<Image>().color = temp.interactable ? Skinning.GetSkin(SkinTag.SECONDARY_ELEMENT) : Skinning.GetSkin(SkinTag.SECONDARY_WINDOW);

			temp.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().font = streetFont;
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
		int index = actualCombat.tries - triesCount;

		if(index >= 0)
			lifePoints[actualCombat.tries - triesCount].Play("Break");

		effectAnimator.Play("Empty");

		katanaSlider.slider.value = 0;
		actualPhase = Phase.KATANA;

		SetCanvasInterractable(true);
	}

	void StartGong()
	{
		int index = actualCombat.tries - triesCount;

		if(index >= 0)
			lifePoints[actualCombat.tries - triesCount].Play("Break");

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
		lastWriter.GetComponent<TextMeshProUGUI>().font = normalFont;
		lastWriter.SetAudio(() => AudioManager.PlaySound("Writting"), () => AudioManager.StopSound("Writting"));

		if(actualPhase == Phase.EFFECT_GENERAL)
			lastWriter.Play(actualCombat.playerLoseResponse, preCombatWriterSpeed, preCombatWriterTrailLength, Skinning.GetSkin(preCombatEnnemyHighlight), Skinning.GetSkin(preCombatEnnemyColor));

		if(actualPhase == Phase.EFFECT_FINAL)
			lastWriter.Play(actualCombat.playerFinisherLoseResponse, preCombatWriterSpeed, preCombatWriterTrailLength, Skinning.GetSkin(preCombatEnnemyHighlight), Skinning.GetSkin(preCombatEnnemyColor));

		actualPhase = Phase.PLAYER_SUICIDE;
		playerTargetIndex = 0;
	}

	void AttackEffect()
	{
		switch(actualPhase)
		{
			case Phase.EFFECT_GENERAL:
				KanjiFinisherAnimator.Play("Hide");
				KanjiGeneralAnimator.Play("ShowAttack");

				generalAttack.font = streetFont;
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

				finisherAttack.font = streetFont;
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

		lastWriter.GetComponent<TextMeshProUGUI>().font = normalFont;
		lastWriter.SetAudio(() => AudioManager.PlaySound("Writting"), () => AudioManager.StopSound("Writting"));
		lastWriter.Play(reaction, preCombatWriterSpeed, preCombatWriterTrailLength, Skinning.GetSkin(preCombatEnnemyHighlight), Skinning.GetSkin(preCombatEnnemyColor));
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

			lastWriter.GetComponent<TextMeshProUGUI>().font = normalFont;
			lastWriter.SetAudio(() => AudioManager.PlaySound("Writting"), () => AudioManager.StopSound("Writting"));
			lastWriter.Play(actualCombat.playerWinResponse, preCombatWriterSpeed, preCombatWriterTrailLength, Skinning.GetSkin(preCombatEnnemyHighlight), Skinning.GetSkin(preCombatEnnemyColor));

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
				playerTargetIndex = 2;
				playerAnimator.enabled = false;

				foreach (Animator anim in lifePoints)
				{
					if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Break"))
						anim.Play("Break");
				}

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
		toConsequences.Invoke();
	}

	void GameOverGeneral()
	{
		actualCombat.actualState = GameData.GameState.GAME_OVER_GENERAL;
		MoveToConsequences();
	}

	void GameOverFinisher()
	{
		actualCombat.actualState = GameData.GameState.GAME_OVER_FINISHER;
		MoveToConsequences();
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

		public void Init(Action<List<Punchline>> showCallback, GeneralPunchlines gamePunchlines, TMP_FontAsset streetAsset)
		{
			List<Punchline> temp = gamePunchlines.allPunchlines.Find(item => { return item.category == category; }).punchlines;
			button.onClick.AddListener(() => showCallback.Invoke(temp));

			button.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().font = streetAsset;
			button.targetGraphic.color = GameData.GetColorFromCategory(category);
		}
	}
}