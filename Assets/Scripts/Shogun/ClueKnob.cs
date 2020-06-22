using System;
using UnityEngine;
using UnityEngine.UI;
using static ShogunManager;

public class ClueKnob : MonoBehaviour, IInitializable, IDebugable
{
	[Header("Assign in Inspector")]
	public GameObject locked;
	public GameObject unlocked, selected;
	public SkinGraphic lockedKanji, unlockedBack, selectedBack;

	IDebugable debuguableInterface => (IDebugable) this;
	IInitializable initializableInterface => (IInitializable) this;

	public bool initialized => initializableInterface.initializedInternal;
	public bool isLocked => !isUnlocked;

	public Action showClue { get; private set; }

	bool IInitializable.initializedInternal { get; set; }
	string IDebugable.debugLabel => "<b>[ClueKnob] : </b>";

	Clue clue;
	bool isSelected, isUnlocked;

	public void Init(bool isUnlocked, Clue clue, ShogunCharacter characterPortrait, Action<string, ShogunCharacter> showClue)
	{
		this.isUnlocked = isUnlocked;
		this.clue = clue;
		this.showClue = () => { showClue.Invoke(clue.summary, characterPortrait); };

		locked.SetActive(true);
		unlocked.SetActive(false);
		selected.SetActive(false);

		lockedKanji.Skin();
		unlockedBack.Skin();
		selectedBack.Skin();

		selected.SetActive(false);
		isSelected = false;

		initializableInterface.InitInternal();
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;
		Debug.Log(debuguableInterface.debugLabel + "Initializing done");
	}

	// called when you hover with mouse and you started a path
	public void SelectForPath()
	{
		if(!initialized)
		{
			Debug.LogError(debuguableInterface.debugLabel + "Not initialized");
			return;
		}

		isSelected = !isSelected;

		selected.SetActive(isSelected);
		unlocked.SetActive(!isSelected);

		AudioManager.PlaySound("Knob");

		Debug.Log(debuguableInterface.debugLabel + (isSelected? "Selected knob": "Unselected knob"));
	}

	public void DeselectKnob(bool playSound = true)
	{
		if(!initialized)
		{
			Debug.LogError(debuguableInterface.debugLabel + "Not initialized");
			return;
		}

		isSelected = false;

		selected.SetActive(false);
		unlocked.SetActive(!isLocked);

		if(playSound)
			AudioManager.PlaySound("Knob");

		Debug.Log(debuguableInterface.debugLabel + "Unselected knob");
	}

	public void Unlock()
	{
		if(!initialized)
		{
			Debug.LogError(debuguableInterface.debugLabel + "Not initialized");
			return;
		}

		isUnlocked = true;

		locked.SetActive(false);
		unlocked.SetActive(true);
	}

	public SubCategory GetSubCategory()
	{
		if(!initialized)
		{
			Debug.LogError(debuguableInterface.debugLabel + "Not initialized");
			return SubCategory.EMPTY;
		}

		return clue.correctedSubCategory;
	}

	public bool CompareClue(Clue clue)
	{
		if(!initialized)
		{
			Debug.LogError(debuguableInterface.debugLabel + "Not initialized");
			return false;
		}

		return this.clue == clue;
	}

	public Clue GetClue()
	{
		return clue;
	}
}