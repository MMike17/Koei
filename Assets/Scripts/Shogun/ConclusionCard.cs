using TMPro;
using UnityEngine;

public class ConclusionCard : MonoBehaviour, IInitializable, IDebugable
{
	[Header("Assign in Inspector")]
	public GameObject hidden;
	public TextMeshProUGUI comment;
	public TextMeshProUGUI category;

	[Header("Debug")]
	public Conclusion conclusion;

	public bool initialized => initializableInterface.initializedInternal;

	IInitializable initializableInterface => (IInitializable) this;
	IDebugable debugableInterface => (IDebugable) this;

	string IDebugable.debugLabel => "<b>[DesignedCard] : </b>";
	bool IInitializable.initializedInternal { get; set; }

	public void Init(Conclusion data)
	{
		conclusion = data;

		comment.text = data.comment;
		comment.color = GameData.GetColorFromCategory(data.category);

		category.text = GameData.PrettyCategory(data.category);
		category.color = GameData.GetColorFromCategory(data.category);

		initializableInterface.InitInternal();

		ShowCard();
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;

		Debug.Log(debugableInterface.debugLabel + "Initializing done");
	}

	public void HideCard()
	{
		if(!initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "Not initialized");
			return;
		}

		hidden.SetActive(true);
	}

	public void ShowCard()
	{
		if(!initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "Not initialized");
			return;
		}

		hidden.SetActive(false);
	}
}