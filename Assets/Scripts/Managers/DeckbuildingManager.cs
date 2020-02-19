using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// class managing gameplay of Deckbuilding panel
public class DeckbuildingManager : MonoBehaviour, IDebugable, IInitializable
{
	[Header("Assign in Inspector")]
	public ScrollRect cardPoolScroll;

	IInitializable initializableInterface => (IInitializable) this;
	IDebugable debugableInterface => (IDebugable) this;

	public bool initialized => initializableInterface.initializedInternal;

	bool IInitializable.initializedInternal { get; set; }
	string IDebugable.debugLabel => "<b>[DeckbuildingManager] : </b>";

	public void Init()
	{
		initializableInterface.InitInternal();
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;

		Debug.Log(debugableInterface.debugLabel + "Initializing done");
	}

	void Update()
	{
		ManageCardPoolScroll();
	}

	void ManageCardPoolScroll()
	{
		PointerEventData pointerData = ActuallyUsefulInputModule.GetPointerEventData();

		// scroll pool cards here
	}
}