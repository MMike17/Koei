using System;
using UnityEngine;
using UnityEngine.UI;

public class ClueKnob : MonoBehaviour, IInitializable, IDebugable
{
	[Header("Assign in Inspector")]
	public GameObject locked;
	public GameObject unlocked, selected;
	public Button showClueButton;

	IDebugable debuguableInterface => (IDebugable) this;
	IInitializable initializableInterface => (IInitializable) this;

	public bool initialized => initializableInterface.initializedInternal;
	public bool isLocked => !isUnlocked;

	bool IInitializable.initializedInternal { get; set; }
	string IDebugable.debugLabel => "<b>[ClueKnob] : </b>";

	Clue clue;
	bool isSelected, isUnlocked;

	public void Init(bool isUnlocked, Clue clue, Sprite characterPortrait, Action<string, Sprite> showClue)
	{
		this.isUnlocked = isUnlocked;
		this.clue = clue;

		CheckState();

		showClueButton.onClick.AddListener(() => { showClue.Invoke(clue.summary, characterPortrait); });

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
		CheckState();

		Debug.Log(debuguableInterface.debugLabel + (isSelected? "Selected knob": "Unselected knob"));
	}

	public void DeselectKnob()
	{
		if(!initialized)
		{
			Debug.LogError(debuguableInterface.debugLabel + "Not initialized");
			return;
		}

		isSelected = false;

		selected.SetActive(false);
		CheckState();

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

		CheckState();
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

	void CheckState()
	{
		locked.SetActive(!isUnlocked);
		unlocked.SetActive(isUnlocked);
	}
}