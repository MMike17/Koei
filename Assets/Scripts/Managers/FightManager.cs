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

	[Header("Assign in Inspector")]
	public Animator canvasAnimator;
	public Animator effectAnimator, KanjiGeneralAnimator, KanjiFinisherAnimator, playerAnimator, enemyAnimator, bloodShed;
	public Transform playerDialoguePosition, enemyDialoguePosition;
	public DialogueWriter writerPrefab;
	[Space]
	public Image[] backgrounds;
	[Space]
	public Image enemyGraph;
	public Image bigEnemyGraph;
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
	public Camera camera;

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
	Action toConsequencesWin, toConsequencesLost;
	string selectedFinisher;
	float preCombatTimer, suicideTimer;
	int dialogueIndex, triesCount, suicideIndex;
	bool isPlayer, writerIsCommanded, isGoodFinisher, gotToFinisher, invokedTransition, destructionAsked;

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

		actualPhase = Phase.INTRO;
		camera.backgroundColor = Skinning.GetSkin(SkinTag.PICTO);

		canvasAnimator.Play("Intro");
		Invoke("StartDialogue", 3);
	}

	public void Init(GeneralPunchlines punchlines, GeneralDialogue dialogue, Action toConsequencesWinCallback, Action toConsequencesLostCallback)
	{
		initializableInterface.InitInternal();

		gamePunchlines = punchlines;

		toConsequencesWin = toConsequencesWinCallback;
		toConsequencesLost = toConsequencesLostCallback;

		katanaSlider.gameObject.SetActive(false);
		gongSlider.Init(StartFinalPunchlines);

		categoryButtons.ForEach(item => item.Init(ShowPunchlines, gamePunchlines));

		foreach (Conclusion conclusion in dialogue.unlockableConclusions)
		{
			ConclusionCard spawned = Instantiate(conclusionPrefab, conclusionScroll);
			spawned.Init(conclusion);
			spawned.ShowCard();
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
			colors.normalColor = Skinning.GetSkin(SkinTag.PICTO);
			colors.highlightedColor = Skinning.GetSkin(SkinTag.PRIMARY_WINDOW);
			colors.pressedColor = Skinning.GetSkin(SkinTag.PRIMARY_ELEMENT);
			finisherPunchlineButtons[i].colors = colors;
		}

		triesCount = 0;
		isGoodFinisher = false;
		gotToFinisher = false;
		invokedTransition = false;
		destructionAsked = false;

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
				if(Input.GetKeyDown(skip))
				{
					actualPhase = Phase.KATANA;
					Destroy(lastWriter.gameObject);
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
				katanaSlider.gameObject.SetActive(true);

				if(katanaSlider.slider.value == 1)
				{
					actualPhase = Phase.CHOICE_GENERAL;

					canvasAnimator.Play("PanUp");
				}

				if(Input.GetKeyDown(skip))
					enemyHealth.Clear();

				if(lastWriter != null && lastWriter.isDone && !destructionAsked)
				{
					Destroy(lastWriter.gameObject, preCombatReplicaDelay * 2);
					destructionAsked = true;
				}
				break;

			case Phase.GONG:
				gongSlider.gameObject.SetActive(true);

				if(lastWriter != null)
					Destroy(lastWriter.gameObject);
				break;

			case Phase.CHOICE_GENERAL:
				generalPunchlinePanel.SetActive(true);
				finisherPunchlinePanel.SetActive(false);

				if(Input.GetKeyDown(skip))
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
				if(lastWriter.isDone)
					EnemySuicideAnimation();
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
				Destroy(lastWriterPlayer.gameObject);

			if(actualCombat.actualState != GameData.GameState.NORMAL)
			{
				if(actualCombat.actualState == GameData.GameState.GAME_OVER_FINISHER)
					actualPhase = Phase.GONG;
				else
					actualPhase = Phase.KATANA;

				if(lastWriter != null)
					Destroy(lastWriter.gameObject);
				return;
			}
			else
			{
				if(dialogueIndex > actualCombat.preCombatReplicas.Count - 1 || string.IsNullOrEmpty(actualCombat.preCombatReplicas[dialogueIndex].playerLine))
				{
					actualPhase = Phase.KATANA;
					Destroy(lastWriter.gameObject);
					return;
				}

				lastWriter = Instantiate(writerPrefab, playerDialoguePosition);
				lastWriter.Play(actualCombat.preCombatReplicas[dialogueIndex].playerLine, preCombatWriterSpeed, preCombatWriterTrailLength, preCombatWriterHighlight, preCombatWriterColor);
			}

			lastWriterPlayer = lastWriter;
		}
		else
		{
			if(lastWriterEnemy != null)
				Destroy(lastWriterEnemy.gameObject);

			if(actualCombat.actualState != GameData.GameState.NORMAL)
			{
				lastWriter = Instantiate(writerPrefab, enemyDialoguePosition);
				lastWriter.Play(actualCombat.preCombatReturnReplica, preCombatWriterSpeed, preCombatWriterTrailLength, preCombatWriterHighlight, preCombatWriterColor);
			}
			else
			{
				if(dialogueIndex > actualCombat.preCombatReplicas.Count - 1 || string.IsNullOrEmpty(actualCombat.preCombatReplicas[dialogueIndex].enemyLine))
				{
					actualPhase = Phase.KATANA;
					Destroy(lastWriter.gameObject);
					return;
				}

				lastWriter = Instantiate(writerPrefab, enemyDialoguePosition);
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
				selectedPunchline = punchline;
				triesCount++;
				actualPhase = Phase.EFFECT_GENERAL;

				canvasAnimator.Play("PanDown");
				Invoke("ShowAttackLines", 1);

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
			colors.normalColor = Skinning.GetSkin(SkinTag.PICTO);
			colors.pressedColor = categoryColor;
			colors.highlightedColor = categoryColor / 1.3f;
			temp.colors = colors;

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

		Invoke("AttackEffect", 0.3f);
	}

	void HideAttackLines()
	{
		KanjiGeneralAnimator.Play("Hide");
		KanjiFinisherAnimator.Play("Hide");

		effectAnimator.Play("CloseLines");

		enemyGraph.sprite = actualCombat.enemySprites[4 + enemyHealth.Count];
		bigEnemyGraph.sprite = enemyGraph.sprite;

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
				Invoke("ShowGameOver", 0.15f);
		}
		else // if(actualPhase == Phase.EFFECT_GENERAL)
		{
			if(enemyHealth.Count > 0)
			{
				if(triesCount >= actualCombat.tries)
					Invoke("ShowGameOver", 0.15f);
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

		lastWriter = Instantiate(writerPrefab, enemyDialoguePosition);

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

				Invoke("HideAttackLines", 4.45f + 1.10f);

				bool takesDamage = false;

				foreach (SubCategory subCategory in enemyHealth)
				{
					if(selectedPunchline.subCategory == subCategory)
						takesDamage = true;
				}

				if(takesDamage)
				{
					Invoke("GeneralAttackGood", 2.25f);
					Invoke("TakeDamageAnimation", 4.45f);
					enemyHealth.Remove(selectedPunchline.subCategory);
				}
				else
				{
					Invoke("GeneralAttackFail", 2.25f);
					Invoke("SpawnFailReaction", 4.45f + 1.15f);
				}
				break;

			case Phase.EFFECT_FINAL:
				KanjiFinisherAnimator.Play("ShowAttack");
				KanjiGeneralAnimator.Play("Hide");

				finisherAttack.text = selectedFinisher;

				Invoke("HideAttackLines", 11.10f + 0.5f);

				if(isGoodFinisher)
					Invoke("TakeDamageAnimation", 11.10f);
				break;
		}
	}

	void TakeDamageAnimation()
	{
		enemyAnimator.Play("TakeDamage");
		enemyGraph.sprite = actualCombat.enemySprites[0];
		bigEnemyGraph.sprite = enemyGraph.sprite;

		if(actualPhase == Phase.EFFECT_GENERAL)
			Invoke("SpawnDamageReaction", 1.10f);
	}

	void SpawnAttackReaction(string reaction)
	{
		lastWriter = Instantiate(writerPrefab, enemyDialoguePosition);

		lastWriter.Play(reaction, preCombatWriterSpeed, preCombatWriterTrailLength, preCombatWriterHighlight, preCombatWriterColor);
	}

	void SpawnDamageReaction()
	{
		SpawnAttackReaction(gamePunchlines.GetRandomDamageReaction());
	}

	void SpawnFailReaction()
	{
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
			lastWriter = Instantiate(writerPrefab, enemyDialoguePosition);
			lastWriter.Play(actualCombat.playerWinResponse, preCombatWriterSpeed, preCombatWriterTrailLength, preCombatWriterHighlight, preCombatWriterColor);

			return;
		}

		effectAnimator.Play("Empty");
		enemyAnimator.enabled = false;

		enemyGraph.sprite = actualCombat.enemySprites[suicideIndex];
		bigEnemyGraph.sprite = enemyGraph.sprite;

		if(suicideIndex == 2)
			PlayBloodShed();

		if(suicideIndex != 3)
		{
			Invoke("EnemySuicideAnimation", suicideDelay);
			suicideIndex++;
		}
	}

	void PlayerSuicideAnimation()
	{
		playerAnimator.Play("Suicide");

		Invoke("PlayBloodShed", 1);

		invokedTransition = true;
	}

	void PlayBloodShed()
	{
		bloodShed.gameObject.SetActive(true);

		switch(actualPhase)
		{
			case Phase.PLAYER_SUICIDE:
				bloodShed.Play("Player");
				break;
			case Phase.SUICIDE:
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