using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static ShogunManager;

public class ShogunPopup : Popup
{
	[Header("Settings")]
	public float clueKnobSpawnPadding;
	public float clueKnobMinDistance, clueDisplayDelay;
	[Range(0, 1)]
	public float unselectedAlpha;
	public int positionComputingLimit, goodGameThreshold;
	public SkinTag normalPath, validatedPath, wrongPath, oldPath;
	public TMP_FontAsset streetFont;
	public KeyCode skip;

	[Header("Assing in Inspector")]
	public ClueKnob clueKnobPrefab;
	public ConclusionCard cardPrefab;
	public Path pathPrefab;
	public RectTransform clueKnobSpawnZone, cardList, endPath, cursor, infoBubble;
	public TextMeshProUGUI lineCounter, clueDescription, clueLineAddUp;
	public UICharacter popupCharacterPortrait;
	public Button returnButton, combatButton;
	public GraphicRaycaster raycaster;
	public Animator anim;
	public DialogueWriter deityWritter;

	List<Conclusion> conclusionsToUnlock;
	List<Path> selectionPath, persistentPath;
	List<ClueKnob> spawnedKnobs;
	List<Clue> selectedClues;
	ClueKnob previouslyHoveredKnob;
	Action toCombat;
	string goodDeityFeedbcak, badDeityFeedback;
	float clueDisplayTimer;
	int positionComputingStep, lineTries;
	bool isSettingPath, inFeedback, useCheats, lastLineGood;

	public void SpecificInit(bool useCheats, List<Clue> clueList, List<Conclusion> unlockableConclusions, List<ShogunCharacter> characters, string goodFeedback, string badFeedback, Action returnCallback, Action combatCallback, GameData.GameState actualState)
	{
		this.useCheats = useCheats;
		toCombat = combatCallback;
		goodDeityFeedbcak = goodFeedback;
		badDeityFeedback = badFeedback;

		SpawnKnobs(clueList, characters);
		SpawnConclusions(unlockableConclusions, actualState);

		deityWritter.SetAudio(() => AudioManager.PlaySound("Writter"), () => AudioManager.StopSound("Writter"));

		returnButton.onClick.AddListener(() => { returnCallback.Invoke(); SetStateCursor(false); AudioManager.PlaySound("Button"); });
		combatButton.onClick.AddListener(() =>
		{
			bool can = true;

			foreach (Conclusion conclusion in conclusionsToUnlock)
			{
				if(!conclusion.IsUnlocked(true))
					can = false;
			}

			if(can)
			{
				SetStateCursor(false);

				if(actualState == GameData.GameState.NORMAL)
				{
					Invoke(lineTries <= goodGameThreshold ? "StartGoodFeedback" : "StartBadFeedback", 2.55f);
					anim.Play("GoodFeedback");
				}
				else
				{
					toCombat.Invoke();
					AudioManager.PlaySound("Button");
				}
			}
			else
			{
				// should play bad sound
				// AudioManager.PlaySound("Button");
			}
		});

		selectionPath = new List<Path>();
		persistentPath = new List<Path>();
		selectedClues = new List<Clue>();

		isSettingPath = false;
		inFeedback = false;
		clueDisplayTimer = 0;
		lineTries = 0;

		foreach (TextMeshProUGUI child in transform.GetChildrenOfType<TextMeshProUGUI>())
		{
			if(child != deityWritter.GetComponent<TextMeshProUGUI>())
			{
				child.font = streetFont;
				child.wordSpacing = 5;
			}
		}

		ResetPopup();
	}

	void SpawnConclusions(List<Conclusion> unlockableConclusions, GameData.GameState state)
	{
		conclusionsToUnlock = new List<Conclusion>();

		foreach (Conclusion conclusion in unlockableConclusions)
		{
			ConclusionCard spawned = Instantiate(cardPrefab, cardList);

			spawned.Init(conclusion);

			if(state == GameData.GameState.GAME_OVER_FINISHER)
				spawned.ShowCard();
			else
				spawned.HideCard();

			conclusionsToUnlock.Add(new Conclusion(conclusion.category, conclusion.correctedSubCategory, spawned));
		}
	}

	void SpawnKnobs(List<Clue> clueList, List<ShogunCharacter> characters)
	{
		// compute real spawn zone (apply padding)
		Rect rectangle = clueKnobSpawnZone.rect;

		rectangle.xMin += clueKnobSpawnPadding / 2;
		rectangle.yMin += clueKnobSpawnPadding / 2;
		rectangle.xMax -= clueKnobSpawnPadding / 2;
		rectangle.yMax -= clueKnobSpawnPadding / 2;

		// debug
		// Image temp = Instantiate(popupCharacterPortrait, clueKnobSpawnZone);
		// temp.transform.localPosition = clueKnobSpawnZone.localPosition;
		// temp.enabled = true;
		// temp.color = Color.red;

		// RectTransform tempRectTransform = temp.GetComponent<RectTransform>();
		// tempRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, clueKnobSpawnZone.rect.width);
		// tempRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, clueKnobSpawnZone.rect.height);
		// tempRectTransform.localPosition = new Vector2(clueKnobSpawnZone.rect.xMin + clueKnobSpawnZone.rect.width / 2, clueKnobSpawnZone.rect.yMin + clueKnobSpawnZone.rect.height / 2);

		// temp = Instantiate(popupCharacterPortrait, clueKnobSpawnZone);
		// temp.transform.localPosition = rectangle.position;
		// temp.enabled = true;
		// temp.color = Color.green;

		// tempRectTransform = temp.GetComponent<RectTransform>();
		// tempRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectangle.width);
		// tempRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectangle.height);
		// tempRectTransform.localPosition = new Vector2(rectangle.xMin + rectangle.width / 2, rectangle.yMin + rectangle.height / 2);
		// debug

		// list of spawned clue knobs to check distance with existing knobs
		spawnedKnobs = new List<ClueKnob>();

		foreach (Clue clue in clueList)
		{
			positionComputingStep = 0;
			Vector3 targetPosition = ComputeRandomPosition(rectangle);

			ClueKnob spawned = Instantiate(clueKnobPrefab, clueKnobSpawnZone);
			spawned.transform.localPosition = targetPosition;
			spawnedKnobs.Add(spawned);

			ShogunCharacter character = characters.Find(item => { return item.character == clue.giverCharacter; });

			if(character == null)
			{
				Debug.LogError(debugableInterface.debugLabel + "Couldn't find ShogunCharacter for character " + clue.giverCharacter + "(this will generate errors later)");
				continue;
			}

			spawned.Init(false, clue, character, (string clueDetail, ShogunCharacter portrait) =>
			{
				// shows clue only if unlocked
				if(!spawned.isLocked)
					ShowClue(clueDetail, character);
			});
		}
	}

	Vector3 ComputeRandomPosition(Rect spawnZone)
	{
		bool shouldRestart = false;

		// computes random position
		Vector3 newPosition = new Vector2(UnityEngine.Random.Range(spawnZone.xMin, spawnZone.xMax), UnityEngine.Random.Range(spawnZone.yMin, spawnZone.yMax));

		if(positionComputingStep > positionComputingLimit)
		{
			Debug.LogWarning(debugableInterface.debugLabel + "Reached computing threshold. Aborting recursive computing to safeguard program integrity (minimum distance between knobs is no longer guaranteed)");
			return newPosition;
		}

		foreach (ClueKnob knob in spawnedKnobs)
		{
			// checks distance between vector and previous knob
			if(Vector3.Distance(knob.transform.localPosition, newPosition) < clueKnobMinDistance)
				shouldRestart = true;
		}

		// calls method again in a recursive way to get a new random vector
		if(shouldRestart)
		{
			positionComputingStep++;

			return ComputeRandomPosition(spawnZone);
		}
		else // or returns the random vector because it's correct
		{
			return newPosition;
		}
	}

	public override void ResetPopup()
	{
		SetStateInfoBubble(false);
		SetStateCursor(false);

		lineCounter.text = "0 / 3";
		clueDescription.text = string.Empty;
		popupCharacterPortrait.Hide();

		base.ResetPopup();
	}

	void Update()
	{
		// retrieves pointer event data from current input module
		PointerEventData eventData = ActuallyUsefulInputModule.GetPointerEventData(-1);

		SetStateCursor(false);

		ClueKnob selected = null;

		if(inFeedback)
		{
			if(Input.GetMouseButtonDown(0))
			{
				if(!deityWritter.isDone)
					deityWritter.Finish();
				else
				{
					toCombat.Invoke();
					AudioManager.PlaySound("Button");
				}
			}

			return;
		}

		if(Input.GetKeyDown(skip))
			conclusionsToUnlock.ForEach(item => item.cardObject.ShowCard());

		foreach (GameObject ui in eventData.hovered)
		{
			// show cursor inside area
			if(ui == clueKnobSpawnZone.gameObject)
			{
				SetStateCursor(true);
				cursor.transform.position = Input.mousePosition;
			}

			ClueKnob local = ui.GetComponent<ClueKnob>();

			// increment info bubble timer for display
			if(local != null)
			{
				clueDisplayTimer += Time.deltaTime;
				selected = local;
			}

			// blocks info bubble with goddes feedback
			if(ui.CompareTag("Block") && ui.GetComponent<CanvasGroup>().blocksRaycasts)
			{
				clueDisplayTimer = 0;
				selected = null;

				SetStateCursor(false);
			}
		}

		// shows clue info bubble
		if(selected != null)
		{
			if(clueDisplayTimer >= clueDisplayDelay)
				selected.showClue.Invoke();
		}
		else
		{
			clueDisplayTimer = 0;
			SetStateInfoBubble(false);
		}

		// if first click
		if(Input.GetMouseButtonDown(0))
		{
			// detect if pressed above clue knob
			eventData.hovered.ForEach(eventObject =>
			{
				ClueKnob knob = eventObject.GetComponent<ClueKnob>();

				if(knob != null && !knob.isLocked)
				{
					// deletes previous selection path
					selectionPath.ForEach(path =>
					{
						Path check = persistentPath.Find(item => { return item.Compare(path); });

						if(check == null && path.GetState() == Path.State.VALIDATED && lastLineGood)
						{
							persistentPath.Add(path);
							path.SetOld();
						}
						else
							Destroy(path.gameObject);
					});

					persistentPath.ForEach(item => item.transform.SetAsFirstSibling());

					selectionPath.Clear();

					// spawns first path
					SpawnNewPath(knob);

					selectedClues.Clear();

					selectedClues.Add(knob.GetClue());
					knob.SelectForPath();

					isSettingPath = true;
					Debug.Log(debugableInterface.debugLabel + "Started selection path");
				}
			});
		}

		// if we are drawing line (prevents miscalls when we're not in pannel)
		if(isSettingPath)
		{
			// change unselected clue knobs alpha
			List<ClueKnob> selectedKnobs = GetAllSelectedKnobs();

			spawnedKnobs.ForEach(item => item.SetKnobAlpha(unselectedAlpha));
			selectedKnobs.ForEach(item => item.SetKnobAlpha(1));

			// stops path if mouse click is stopped
			if(!Input.GetMouseButton(0))
			{
				isSettingPath = false;

				bool isPathFinished = false;

				// detects if ended line
				if(selectedClues.Count == 3)
				{
					// there is no clue on last knob
					selectionPath[selectionPath.Count - 1].UpdatePath(true, endPath.localPosition);
					persistentPath.ForEach(item => item.transform.SetAsFirstSibling());

					isPathFinished = true;
					Debug.Log(debugableInterface.debugLabel + "Finished selection path");
				}

				List<SubCategory> pathCheck = CheckFullPath(isPathFinished);

				if(isPathFinished)
					CheckConclusionUnlock(pathCheck);
				else
					Debug.Log(debugableInterface.debugLabel + "Interrupted selection path");

				ResetPopup();
			}
			else // updates path if mouse is pressed and path started
			{
				ClueKnob knob = null;

				eventData.hovered.ForEach(item =>
				{
					if(item.GetComponent<ClueKnob>() != null)
						knob = item.GetComponent<ClueKnob>();
				});

				// if we are above knob
				if(knob != null)
				{
					// if it's new knob (if we are not above empty anymore) & can select knob
					if(knob != previouslyHoveredKnob && !knob.isLocked)
					{
						// if knob is not in chain
						if(selectionPath.Find(path => { return path.ContainsKnob(knob.transform); }) == null)
						{
							// if we didn't select too many clues
							if(selectionPath.Count < 3)
							{
								SpawnNewPath(knob, knob.transform);

								selectedClues.Add(knob.GetClue());
								knob.SelectForPath();
							}
						}
						// if knob is in chain and is last in chain
						// (we don't take last path because last path is the path currently forming by player)
						else if(selectionPath.Count > 1 && knob.transform == selectionPath[selectionPath.Count - 2].GetEnd())
						{
							// deselects knob
							selectedClues.Remove(knob.GetClue());

							Destroy(selectionPath[selectionPath.Count - 1].gameObject);
							selectionPath.RemoveAt(selectionPath.Count - 1);

							selectionPath[selectionPath.Count - 1].SetEnd(null, SubCategory.EMPTY);

							knob.SelectForPath();
						}
					}
					else
					{
						selectionPath[selectionPath.Count - 1].UpdatePath(true, RealMousePosition());
						persistentPath.ForEach(item => item.transform.SetAsFirstSibling());
					}
				}
				else // if we are not above knob
				{
					// sticks end of path on mouse
					selectionPath[selectionPath.Count - 1].UpdatePath(true, RealMousePosition());
					persistentPath.ForEach(item => item.transform.SetAsFirstSibling());
				}

				previouslyHoveredKnob = knob;
			}

			if(selectedClues.Count > 0)
			{
				clueLineAddUp.text = selectedClues[0].deductionLine;

				if(selectedClues.Count > 1)
				{
					clueLineAddUp.text += ", " + selectedClues[1].deductionLine;

					if(selectedClues.Count > 2)
						clueLineAddUp.text += " et " + selectedClues[2].deductionLine + " donc...";
				}
			}
			else
				clueLineAddUp.text = string.Empty;
		}

		if(selectionPath.Count > 0 && isSettingPath)
		{
			lineCounter.gameObject.SetActive(true);
			lineCounter.text = selectionPath.Count + " / 3";
		}
		else
			lineCounter.gameObject.SetActive(false);
	}

	// ends previous path if there is one and spawns a new one
	void SpawnNewPath(ClueKnob start, Transform endPrevious = null)
	{
		// ends previous path
		if(selectionPath.Count > 0 && endPrevious != null)
			selectionPath[selectionPath.Count - 1].SetEnd(endPrevious, start.GetSubCategory());

		// spawn new path
		Path spawned = Instantiate(pathPrefab, clueKnobSpawnZone);
		spawned.Init(start.transform, normalPath, validatedPath, wrongPath, oldPath, start.GetSubCategory());
		selectionPath.Add(spawned);
	}

	// clue knob button
	void ShowClue(string clueLine, ShogunCharacter characterPortrait)
	{
		SetStateInfoBubble(true);

		clueDescription.text = clueLine;
		popupCharacterPortrait.SetCharacterPortrait(characterPortrait);
	}

	public void ChecKnobsState(List<Clue> playerClues)
	{
		foreach (Clue clue in playerClues)
		{
			ClueKnob knob = spawnedKnobs.Find(item => { return item.CompareClue(clue); });

			if(knob == null)
				Debug.LogError(debugableInterface.debugLabel + "Couldn't find a ClueKnob with clue " + clue.ToString() + " (ClueKnob probably haven't been initialized properly)");
			else
				knob.Unlock();
		}
	}

	List<SubCategory> CheckFullPath(bool isFinished)
	{
		// deselect all knobs
		spawnedKnobs.ForEach(item => { item.DeselectKnob(false); item.SetKnobAlpha(1); });

		// destroys last path (if finished it's linked to end knob, if not it's interrupted)
		if(selectionPath[selectionPath.Count - 1].end == null || selectionPath[selectionPath.Count - 1].end == endPath)
		{
			Destroy(selectionPath[selectionPath.Count - 1].gameObject);
			selectionPath.RemoveAt(selectionPath.Count - 1);
		}

		// path checks it's own state
		List<SubCategory> pathSubCategories = new List<SubCategory>();
		selectionPath.ForEach(item => pathSubCategories.Add(item.CheckState()));

		lineTries++;

		return pathSubCategories;
	}

	List<ClueKnob> GetAllSelectedKnobs()
	{
		List<ClueKnob> result = new List<ClueKnob>();

		spawnedKnobs.ForEach(knob =>
		{
			selectedClues.ForEach(clue =>
			{
				if(knob.GetClue() == clue)
					result.Add(knob);
			});
		});

		return result;
	}

	void CheckConclusionUnlock(List<SubCategory> pathSubCategories)
	{
		// checks if we unlocked a card
		bool unlockCard = true;

		foreach (SubCategory subCategory in pathSubCategories)
		{
			// compares first subCategory to all subCategories
			if(pathSubCategories[0] != subCategory)
				unlockCard = false;
		}

		// unlocks card
		if(pathSubCategories.Count == 2 && unlockCard && pathSubCategories[0] != SubCategory.EMPTY)
		{
			Conclusion unlock = conclusionsToUnlock.Find(item => { return item.correctedSubCategory == pathSubCategories[0]; });

			if(unlock == null)
				Debug.LogError(debugableInterface.debugLabel + "Could't find card to unlock with SubCategory " + pathSubCategories[0].ToString() + " (this will create errors later)");
			else // adds card to player data
				unlock.cardObject.ShowCard();
		}

		// checks for good feedback
		foreach (Conclusion conclusion in conclusionsToUnlock)
			conclusion.IsUnlocked();

		lastLineGood = unlockCard;
	}

	Vector2 RealMousePosition()
	{
		// complicated mouse position computation because fuck me I guess ?
		Vector2 totalOffset = RecursiveTotalOffset(clueKnobSpawnZone);
		Vector2 mousePosition = Input.mousePosition;

		mousePosition.x -= Screen.width / 2 + totalOffset.x;
		mousePosition.y -= Screen.height / 2 + totalOffset.y;

		return mousePosition;
	}

	// we need to add offset of all parent ui objects
	Vector2 RecursiveTotalOffset(RectTransform ui)
	{
		if(ui.parent.GetComponent<Canvas>() != null)
			return ui.localPosition;
		else
			return (Vector2) ui.localPosition + RecursiveTotalOffset(ui.parent.GetComponent<RectTransform>());
	}

	void StartGoodFeedback()
	{
		inFeedback = true;
		deityWritter.Play(goodDeityFeedbcak);
	}

	void StartBadFeedback()
	{
		inFeedback = true;
		deityWritter.Play(badDeityFeedback);
	}

	void SetStateCursor(bool state)
	{
		cursor.gameObject.SetActive(state);
	}

	void SetStateInfoBubble(bool state)
	{
		infoBubble.gameObject.SetActive(state);
	}
}