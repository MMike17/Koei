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
	public int preCombatWriterTrailLength;
	public Color preCombatWriterColor, preCombatWriterHighlight;

	[Header("Assign in Inspector")]
	public Animator canvasAnimator;
	public Animator effectAnimator, KanjiGeneralAnimator, KanjiFinisherAnimator, playerAnimator, enemyAnimator, bloodShed;
	public Transform playerDialoguePosition, enemyDialoguePosition;
	public DialogueWriter writerPrefab;
	[Space]
	public Image[] backgrounds;
	[Space]
	public Image enemyGraph;
	public Slider katanaSlider;
	[Space]
	public List<CategoryButton> categoryButtons;
	[Space]
	public Button punchlinePrefab;
	[Space]
	public Button[] finisherPunchlineButtons;
	[Space]
	public Transform punchlineScroll;
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
	Action toConsequences, restart;
	string selectedFinisher;
	float preCombatTimer, suicideTimer;
	int dialogueIndex, triesCount, suicideIndex;
	bool isPlayer, writerIsCommanded, isFinisher;

	public void PreInit(CombatDialogue actualCombat)
	{
		for (int i = 0; i < backgrounds.Length; i++)
		{
			backgrounds[i].sprite = actualCombat.sceneBackgrounds[i];
		}

		this.actualCombat = actualCombat;

		enemyHealth = new List<SubCategory>(actualCombat.weaknesses);
		enemyGraph.sprite = actualCombat.enemySprites[4 + enemyHealth.Count];

		actualPhase = Phase.INTRO;

		canvasAnimator.Play("Intro");
		Invoke("StartDialogue", 3);
	}

	public void Init(GeneralPunchlines punchlines, Action toConsequencesCallback, Action restartCallback)
	{
		initializableInterface.InitInternal();

		this.gamePunchlines = punchlines;

		toConsequences = toConsequencesCallback;
		restart = restartCallback;

		katanaSlider.gameObject.SetActive(false);

		categoryButtons.ForEach(item =>
		{
			item.Init(ShowPunchlines, gamePunchlines);
		});

		for (int i = 0; i < finisherPunchlineButtons.Length; i++)
		{
			finisherPunchlineButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = actualCombat.finisherPunchlines.finishers[i];

			int j = i;

			finisherPunchlineButtons[i].onClick.AddListener(() =>
			{
				selectedFinisher = actualCombat.finisherPunchlines.finishers[j];
				isFinisher = j == actualCombat.finisherPunchlines.correctOne;

				canvasAnimator.Play("PanDown");
				Invoke("ShowAttackLines", 1);

				SetCanvasInterractable(false);
				actualPhase = Phase.EFFECT_FINAL;
			});
		}

		triesCount = 0;
		isFinisher = false;

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
				if(lastWriter.isDone && !writerIsCommanded)
				{
					Invoke("SpawnNextPreDialogue", preCombatReplicaDelay * 2);
					writerIsCommanded = true;
					katanaSlider.value = 0;
				}
				break;

			case Phase.KATANA:
				katanaSlider.gameObject.SetActive(true);

				if(katanaSlider.value == 1)
				{
					if(enemyHealth.Count == 0)
						actualPhase = Phase.CHOICE_FINAL;
					else
						actualPhase = Phase.CHOICE_GENERAL;

					canvasAnimator.Play("PanUp");
				}
				break;

			case Phase.CHOICE_GENERAL:
				generalPunchlinePanel.SetActive(true);
				finisherPunchlinePanel.SetActive(false);
				break;
			case Phase.CHOICE_FINAL:
				generalPunchlinePanel.SetActive(false);
				finisherPunchlinePanel.SetActive(true);
				break;

			case Phase.EFFECT_GENERAL:
				katanaSlider.gameObject.SetActive(false);
				break;
			case Phase.EFFECT_FINAL:
				katanaSlider.gameObject.SetActive(false);
				break;

			case Phase.PLAYER_SUICIDE:
				if(lastWriter.isDone)
					Invoke("PlayerSuicideAnimation", preCombatReplicaDelay);
				break;
		}
	}

	void StartDialogue()
	{
		actualPhase = Phase.DIALOGUE;
		dialogueIndex = 0;

		isPlayer = !string.IsNullOrWhiteSpace(actualCombat.preCombatReplicas[0].playerLine);

		SpawnNextPreDialogue();
	}

	void SpawnNextPreDialogue()
	{
		if(isPlayer)
		{
			if(lastWriterPlayer != null)
				Destroy(lastWriterPlayer.gameObject);

			if(dialogueIndex > actualCombat.preCombatReplicas.Count - 1 || string.IsNullOrEmpty(actualCombat.preCombatReplicas[dialogueIndex].playerLine))
			{
				actualPhase = Phase.KATANA;
				Destroy(lastWriter.gameObject);
				return;
			}

			lastWriter = Instantiate(writerPrefab, playerDialoguePosition);
			lastWriter.Play(actualCombat.preCombatReplicas[dialogueIndex].playerLine, preCombatWriterSpeed, preCombatWriterTrailLength, preCombatWriterHighlight, preCombatWriterColor);

			lastWriterPlayer = lastWriter;
		}
		else
		{
			if(lastWriterEnemy != null)
				Destroy(lastWriterEnemy.gameObject);

			if(dialogueIndex > actualCombat.preCombatReplicas.Count - 1 || string.IsNullOrEmpty(actualCombat.preCombatReplicas[dialogueIndex].enemyLine))
			{
				actualPhase = Phase.KATANA;
				Destroy(lastWriter.gameObject);
				return;
			}

			lastWriter = Instantiate(writerPrefab, enemyDialoguePosition);
			lastWriter.Play(actualCombat.preCombatReplicas[dialogueIndex].enemyLine, preCombatWriterSpeed, preCombatWriterTrailLength, preCombatWriterHighlight, preCombatWriterColor);

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
				temp.GetComponentInChildren<TextMeshProUGUI>().text = punchline.subCategory.ToString();
			else
				temp.GetComponentInChildren<TextMeshProUGUI>().text = punchline.line;

			temp.GetComponent<Image>().color = GameData.GetColorFromCategory(GameData.GetCategoryFromSubCategory(punchline.subCategory));
		}
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

		if(actualPhase == Phase.EFFECT_FINAL)
		{
			if(isFinisher)
			{
				suicideIndex = 1;
				suicideTimer = 0;

				Invoke("EnemySuicideAnimation", 1);
				return;
			}
			else
			{
				Invoke("ShowGameOver", 0.15f);
			}
		}
		else // if(actualPhase == Phase.EFFECT_GENERAL)
		{
			if(triesCount >= actualCombat.tries && enemyHealth.Count != 0)
				Invoke("ShowGameOver", 0.15f);
			else
				Invoke("StartKatanaAgain", 0.15f);
		}
	}

	void StartKatanaAgain()
	{
		effectAnimator.Play("Empty");

		katanaSlider.value = 0;
		actualPhase = Phase.KATANA;

		SetCanvasInterractable(true);
	}

	void ShowGameOver()
	{
		effectAnimator.Play("Empty");

		actualPhase = Phase.PLAYER_SUICIDE;

		lastWriter = Instantiate(writerPrefab, enemyDialoguePosition);
		lastWriter.Play(actualCombat.playerLoseResponse, preCombatWriterSpeed, preCombatWriterTrailLength, preCombatWriterHighlight, preCombatWriterColor);
	}

	void AttackEffect()
	{
		switch(actualPhase)
		{
			case Phase.EFFECT_GENERAL:
				KanjiFinisherAnimator.Play("Hide");
				KanjiGeneralAnimator.Play("Show");

				generalAttack.text = selectedPunchline.line;
				break;

			case Phase.EFFECT_FINAL:
				KanjiFinisherAnimator.Play("Show");
				KanjiGeneralAnimator.Play("Hide");

				finisherAttack.text = selectedFinisher;
				break;
		}

		Invoke("HideAttackLines", 4.45f + 1);

		bool takesDamage = false;

		foreach (SubCategory subCategory in enemyHealth)
		{
			if(selectedPunchline.subCategory == subCategory)
				takesDamage = true;
		}

		if(takesDamage)
		{
			Invoke("TakeDamageAnimation", 4.45f);
			enemyHealth.Remove(selectedPunchline.subCategory);
		}

		if(isFinisher)
			Invoke("TakeDamageAnimation", 4.45f);
	}

	void TakeDamageAnimation()
	{
		enemyAnimator.Play("TakeDamage");
		enemyGraph.sprite = actualCombat.enemySprites[0];
	}

	void EnemySuicideAnimation()
	{
		effectAnimator.Play("Empty");
		enemyAnimator.enabled = false;

		actualPhase = Phase.SUICIDE;

		enemyGraph.sprite = actualCombat.enemySprites[suicideIndex];

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

		Invoke("FadeToBlack", 2);
	}

	void FadeToBlack()
	{
		// bloodShed.gameObject.SetActive(false);

		effectAnimator.Play("Fade");

		switch(actualPhase)
		{
			case Phase.PLAYER_SUICIDE:
				Invoke("GameOver", 2);
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

	void GameOver()
	{
		restart.Invoke();
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

			button.GetComponent<Image>().color = GameData.GetColorFromCategory(category);
		}
	}
}