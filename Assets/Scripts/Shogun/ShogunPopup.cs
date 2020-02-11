using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShogunPopup : Popup
{
	[Header("Assing in Inspector")]
	public ClueKnob clueKnobPrefab;
	public TextMeshProUGUI lineCounter, clueDescription;
	public Image popupCharacterPortrait;
	public Button returnButton;
	public DesignedCard cardPrefab;

	public override void ResetPopup()
	{
		lineCounter.text = "0 / 3";
		clueDescription.text = string.Empty;

		popupCharacterPortrait.sprite = null;

		base.ResetPopup();
	}
}