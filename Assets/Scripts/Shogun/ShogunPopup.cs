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
	public float clueKnobMinDistance;
	public int positionComputingLimit;
	public Color normalPath, validatedPath, wrongPath;

	[Header("Assing in Inspector")]
	public ClueKnob clueKnobPrefab;
	public ConclusionCard cardPrefab;
	public Path pathPrefab;
	public RectTransform clueKnobSpawnZone, cardList, startPath, endPath;
	public TextMeshProUGUI lineCounter, clueDescription, clueLineAddUp;
	public Image popupCharacterPortrait;
	public Button returnButton, combatButton;

	List<Conclusion> conclusionsToUnlock;
	List<Path> selectionPath;
	List<Path> persistentPaths;
	List<ClueKnob> spawnedKnobs;
	List<Clue> selectedClues;
	ClueKnob previouslyHoveredKnob;
	int positionComputingStep;
	bool isSettingPath;

	public void SpecificInit(List<Clue> clueList, List<Conclusion> unlockableConclusions, List<ShogunCharacter> characters, Action returnCallback, Action combatCallback)
	{
		SpawnKnobs(clueList, characters);
		SpawnConclusions(unlockableConclusions);

		returnButton.onClick.AddListener(() => returnCallback.Invoke());
		combatButton.onClick.AddListener(() => combatCallback.Invoke());

		selectionPath = new List<Path>();
		persistentPaths = new List<Path>();
		selectedClues = new List<Clue>();

		isSettingPath = false;
	}

	void SpawnConclusions(List<Conclusion> unlockableConclusions)
	{
		conclusionsToUnlock = new List<Conclusion>();

		foreach (Conclusion conclusion in unlockableConclusions)
		{
			ConclusionCard spawned = Instantiate(cardPrefab, cardList);

			spawned.Init(conclusion);
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

			spawned.Init(false, clue, character.characterPortrait, validatedPath, (string clueDetail, Sprite portrait) =>
			{
				// shows clue only if unlocked
				if(!spawned.isLocked)
				{
					ShowClue(clueDetail, portrait);
				}
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
			{
				shouldRestart = true;
			}
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
		lineCounter.text = "0 / 3";
		clueDescription.text = string.Empty;

		popupCharacterPortrait.enabled = false;

		base.ResetPopup();
	}

	void Update()
	{
		// retrieves pointer event data from current input module
		PointerEventData eventData = ActuallyUsefulInputModule.GetPointerEventData(-1);

		// if first click
		if(Input.GetMouseButtonDown(0))
		{
			// detect if pressed above startPath object
			eventData.hovered.ForEach(item =>
			{
				if(item.CompareTag("StartPath"))
				{
					// spawns first path (there is no clue on first knob)
					SpawnNewPath(SubCategory.EMPTY);

					isSettingPath = true;
					Debug.Log(debugableInterface.debugLabel + "Started selection path");
				}
			});

			// resets clue panel if clic in empty
			if(!isSettingPath)
			{
				ResetPopup();
			}
		}

		// if we are drawing line (prevents miscalls when we're not in pannel)
		if(isSettingPath)
		{
			// stops path if mouse click is stopped
			if(!Input.GetMouseButton(0))
			{
				isSettingPath = false;
				selectedClues.Clear();

				bool isPathFinished = false;

				// detect if lifted above endPath object
				eventData.hovered.ForEach(item =>
				{
					if(item.CompareTag("EndPath") && selectionPath.Count == 4)
					{
						// there is no clue on last knob
						selectionPath[selectionPath.Count - 1].UpdatePath(endPath.position);

						isPathFinished = true;
						Debug.Log(debugableInterface.debugLabel + "Finished selection path");
					}
				});

				List<SubCategory> pathCheck = CheckFullPath(isPathFinished);

				if(isPathFinished)
				{
					CheckConclusionUnlock(pathCheck);
				}
				else
				{
					Debug.Log(debugableInterface.debugLabel + "Interrupted selection path");
				}
			}
			else // updates path if mouse is pressed and path started
			{
				// feeds in list of knobs from hovered objects
				List<ClueKnob> knobs = new List<ClueKnob>();

				eventData.hovered.ForEach(item =>
				{
					ClueKnob selected = item.GetComponent<ClueKnob>();

					if(selected != null)
					{
						knobs.Add(selected);
					}
				});

				if(knobs.Count > 1)
				{
					Debug.LogWarning(debugableInterface.debugLabel + "There shouldn't be more than one knob hovered at a time");
				}

				ClueKnob knob = null;

				// there should only be one knob at a time
				if(knobs.Count > 0)
				{
					knob = knobs[0];
				}

				// if we are above knob
				if(knob != null)
				{
					if(knob != previouslyHoveredKnob)
					{
						Debug.Log(selectionPath[0].ContainsKnob(knob.transform));
					}

					// if it's new knob (if we are not above empty anymore) & can select knob
					if(knob != previouslyHoveredKnob && !knob.isLocked)
					{
						// if knob is not in chain
						if(selectionPath.Find(path => { return path.ContainsKnob(knob.transform); }) == null)
						{
							// if we didn't select too many clues
							if(selectionPath.Count < 4)
							{
								SpawnNewPath(knob.GetSubCategory(), knob.transform);

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
						selectionPath[selectionPath.Count - 1].UpdatePath(RealMousePosition());
					}
				}
				else // if we are not above knob
				{
					// sticks end of path on mouse
					selectionPath[selectionPath.Count - 1].UpdatePath(RealMousePosition());
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
					{
						clueLineAddUp.text += " et " + selectedClues[2].deductionLine + " donc...";
					}
				}
			}
			else
			{
				clueLineAddUp.text = string.Empty;
			}
		}

		if(selectionPath.Count > 0)
		{
			lineCounter.text = selectionPath.Count - 1 + " / 3";
		}
		else
		{
			lineCounter.text = "0 / 3";
		}
	}

	// ends previous path if there is one and spawns a new one
	void SpawnNewPath(SubCategory start, Transform endPrevious = null)
	{
		Transform beginPath = startPath;

		// ends previous path
		if(selectionPath.Count > 0 && endPrevious != null)
		{
			selectionPath[selectionPath.Count - 1].SetEnd(endPrevious, start);

			// gets previous end point as starting point
			beginPath = selectionPath[selectionPath.Count - 1].GetEnd();
		}

		// spawn new path
		Path spawned = Instantiate(pathPrefab, clueKnobSpawnZone);
		spawned.Init(beginPath, normalPath, validatedPath, wrongPath, start);
		selectionPath.Add(spawned);
	}

	// clue knob button
	void ShowClue(string clueLine, Sprite characterPortrait)
	{
		clueDescription.text = clueLine;
		popupCharacterPortrait.sprite = characterPortrait;
		popupCharacterPortrait.enabled = true;
	}

	public void ChecKnobsState(List<Clue> playerClues)
	{
		foreach (Clue clue in playerClues)
		{
			ClueKnob knob = spawnedKnobs.Find(item => { return item.CompareClue(clue); });

			if(knob == null)
			{
				Debug.LogError(debugableInterface.debugLabel + "Couldn't find a ClueKnob with clue " + clue.ToString() + " (ClueKnob probably haven't been initialized properly)");
			}
			else
			{
				knob.Unlock();
			}
		}
	}

	List<SubCategory> CheckFullPath(bool isFinished)
	{
		// deselect all knobs
		spawnedKnobs.ForEach(item => item.DeselectKnob());

		// destroys first path (linked to start node)
		Destroy(selectionPath[0].gameObject);
		selectionPath.RemoveAt(0);

		if(selectionPath.Count > 0 && selectionPath[selectionPath.Count - 1].end == null || selectionPath[selectionPath.Count - 1].end == endPath)
		{
			// destroys last path (if finished it's linked to end knob, if not it's interrupted)
			Destroy(selectionPath[selectionPath.Count - 1].gameObject);
			selectionPath.RemoveAt(selectionPath.Count - 1);
		}

		List<SubCategory> pathSubCategories = new List<SubCategory>();

		foreach (Path path in selectionPath)
		{
			// path checks it's state
			pathSubCategories.Add(path.CheckState());

			// moves path to path already checked (if we don't already have a similar path)
			Path found = persistentPaths.Find(item => { return item.ContainsKnob(path.start) && item.ContainsKnob(path.end); });

			if(found == null)
			{
				// adds path to paths that will be destroyed
				persistentPaths.Add(path);
			}
		}

		// removes persistent paths from selectionPath to not destroy them
		persistentPaths.ForEach(item => selectionPath.Remove(item));

		// deletes selection path
		selectionPath.ForEach(item => Destroy(item.gameObject));
		selectionPath.Clear();

		return pathSubCategories;
	}

	void CheckConclusionUnlock(List<SubCategory> pathSubCategories)
	{
		// checks if we unlocked a card
		bool unlockCard = true;

		foreach (SubCategory subCategory in pathSubCategories)
		{
			// compares first subCategory to all subCategories
			if(pathSubCategories[0] != subCategory)
			{
				unlockCard = false;
			}
		}

		// unlocks card
		if(pathSubCategories.Count == 2 && unlockCard && pathSubCategories[0] != SubCategory.EMPTY)
		{
			Conclusion unlock = conclusionsToUnlock.Find(item => { return item.correctedSubCategory == pathSubCategories[0]; });

			if(unlock == null)
			{
				Debug.LogError(debugableInterface.debugLabel + "Could't find card to unlock with SubCategory " + pathSubCategories[0].ToString() + " (this will create errors later)");
			}
			else
			{
				// adds card to player data
				unlock.cardObject.ShowCard();
			}
		}
	}

	Vector2 RealMousePosition()
	{
		// complicated mouse position computation because fuck me I guess ?
		Vector2 mousePosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
		mousePosition.x = mousePosition.x * Screen.width - Screen.width / 2;
		mousePosition.y = mousePosition.y * Screen.height - Screen.height / 2;

		return mousePosition;
	}
}