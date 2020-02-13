using TMPro;
using UnityEngine;

public class DesignedCard : MonoBehaviour
{
	[Header("Assign in Inspector")]
	public GameObject greyed;
	public TextMeshProUGUI category;
	public TextMeshProUGUI subcategory;

	public CardTaker cardTaker => GetComponent<CardTaker>();

	void Awake()
	{
		// spawns as not greyed out
		greyed.SetActive(false);
	}

	public void GreyCard()
	{
		greyed.SetActive(true);
	}

	public void ShowCard()
	{
		greyed.SetActive(false);
	}
}