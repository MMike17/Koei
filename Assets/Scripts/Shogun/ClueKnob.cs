using UnityEngine;

public class ClueKnob : MonoBehaviour, IInitializable, IDebugable
{
	[Header("Assign in Inspector")]
	public GameObject locked;
	public GameObject unlocked, selected;

	IDebugable debuguableInterface => (IDebugable) this;
	IInitializable initializableInterface => (IInitializable) this;

	public bool initialized => initializableInterface.initializedInternal;

	bool IInitializable.initializedInternal { get; set; }
	string IDebugable.debugLabel => "<b>[ClueKnob] : </b>";

	bool isSelected;

	public void Init(bool isUnlocked)
	{
		locked.SetActive(!isUnlocked);
		unlocked.SetActive(isUnlocked);

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
	public void Select()
	{
		isSelected = !isSelected;

		selected.SetActive(isSelected);
		unlocked.SetActive(!isSelected);

		Debug.Log(debuguableInterface.debugLabel + "Selected knob");
	}
}