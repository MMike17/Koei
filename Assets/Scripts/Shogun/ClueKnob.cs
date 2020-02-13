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
	public bool isUnlocked => unlocked.activeSelf;

	bool IInitializable.initializedInternal { get; set; }
	string IDebugable.debugLabel => "<b>[ClueKnob] : </b>";

	Clue clue;
	bool isSelected;

	public void Init(bool isUnlocked, Clue clue, Sprite characterPortrait, Action<string, Sprite> showClue)
	{
		locked.SetActive(!isUnlocked);
		unlocked.SetActive(isUnlocked);

		this.clue = clue;

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
		isSelected = !isSelected;

		selected.SetActive(isSelected);
		unlocked.SetActive(!isSelected);

		Debug.Log(debuguableInterface.debugLabel + (isSelected? "Selected knob": "Unselected knob"));
	}

	public SubCategory GetSubCategory()
	{
		return clue.correctedSubCategory;
	}
}