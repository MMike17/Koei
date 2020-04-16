using UnityEngine;
using UnityEngine.UI;
using static ShogunManager;

public class UICharacter : MonoBehaviour
{
	public Image under, over, detail;

	public void SetCharacterPortrait(ShogunCharacter shogunCharacter, Button button = null)
	{
		under.sprite = shogunCharacter.characterUnder;
		over.sprite = shogunCharacter.characterOver;
		detail.sprite = shogunCharacter.characterDetail;

		if(button != null)
		{
			ColorBlock block = button.colors;
			block.highlightedColor = GameData.GetColorFromCharacter(shogunCharacter.character);
			button.colors = block;
		}
	}

	public void Hide()
	{
		under.enabled = false;
		over.enabled = false;
		detail.enabled = false;
	}

	public void Show()
	{
		under.enabled = true;
		over.enabled = true;
		detail.enabled = true;
	}
}