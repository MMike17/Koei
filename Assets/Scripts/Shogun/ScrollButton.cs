using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollButton : MonoBehaviour, IInitializable, IDebugable
{
	[Header("Assign in Inspector")]
	public Button button;
	public Animator animator;

	IInitializable initializableInterface => (IInitializable) this;
	IDebugable debugableInterface => (IDebugable) this;

	bool IInitializable.initialized => initializableInterface.initializedInternal;
	string IDebugable.debugLabel => "<b>[" + GetType() + "] : </b>";

	bool IInitializable.initializedInternal { get; set; }

	ShogunManager shogunManager;

	public void Init(ShogunManager manager)
	{
		shogunManager = manager;

		initializableInterface.InitInternal();
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;

		Debug.Log(debugableInterface.debugLabel + "Initializing done");
	}

	void Update()
	{
		PointerEventData eventData = ActuallyUsefulInputModule.GetPointerEventData();

		bool hoveredButton = false;

		foreach (GameObject hovered in eventData.hovered)
		{
			if(hovered == button.gameObject)
				hoveredButton = true;
		}

		if(hoveredButton && !shogunManager.cluesOpen)
			animator.Play("Open");
		else
			animator.Play("Close");
	}
}