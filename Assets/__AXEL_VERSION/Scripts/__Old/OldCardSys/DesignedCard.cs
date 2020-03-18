using TMPro;
using UnityEngine;

public class DesignedCard : MonoBehaviour, IInitializable, IDebugable
{
	[Header("Assign in Inspector")]
	public GameObject greyed;
	public GameObject normal;
	public TextMeshProUGUI category;
	public TextMeshProUGUI subcategory;

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

		category.text = data.category.ToString();
		subcategory.text = data.correctedSubCategory.ToString();

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

		greyed.SetActive(true);

		normal.SetActive(false);
		category.gameObject.SetActive(false);
		subcategory.gameObject.SetActive(false);
	}

	public void ShowCard()
	{
		if(!initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "Not initialized");
			return;
		}

		greyed.SetActive(false);

		normal.SetActive(true);
		category.gameObject.SetActive(true);
		subcategory.gameObject.SetActive(true);
	}
}