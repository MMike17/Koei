using TMPro;
using UnityEngine;

public class DesignedCard : MonoBehaviour, IInitializable, IDebugable
{
	[Header("Assign in Inspector")]
	public GameObject greyed;
	public TextMeshProUGUI category;
	public TextMeshProUGUI subcategory;

	public CardTaker cardTaker => GetComponent<CardTaker>();
	public bool initialized => initializableInterface.initializedInternal;

	IInitializable initializableInterface => (IInitializable) this;
	IDebugable debugableInterface => (IDebugable) this;

	string IDebugable.debugLabel => "<b>[DesignedCard] : </b>";
	bool IInitializable.initializedInternal { get; set; }

	public void Init(Card data)
	{
		// spawns as not greyed out
		greyed.SetActive(false);

		category.text = data.strength.ToString();
		subcategory.text = data.subStrength.ToString();

		initializableInterface.InitInternal();
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;

		Debug.Log(debugableInterface.debugLabel + "Initializing done");
	}

	public void GreyCard()
	{
		if(!initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "Not initialized");
			return;
		}

		greyed.SetActive(true);
	}

	public void ShowCard()
	{
		if(!initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "Not initialized");
			return;
		}

		greyed.SetActive(false);
	}
}