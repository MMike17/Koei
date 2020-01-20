using UnityEngine;

// class managing gameplay of Deckbuilding panel
public class DeckbuildingManager : MonoBehaviour, IDebugable, IInitializable
{
	IInitializable initializableInterface => (IInitializable) this;
	IDebugable debugableInterface => (IDebugable) this;

	public bool initialized => initializableInterface.initializedInternal;

	bool IInitializable.initializedInternal { get; set; }
	string IDebugable.debugLabel => "<b>[DeckbuildingManager] : </b>";

	public void Init()
	{
		initializableInterface.InitInternal();

		Debug.Log(debugableInterface.debugLabel + "Initializing done");
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;
	}
}