using UnityEngine;
using UnityEngine.UI;
using static ShogunManager;

public class UICharacter : MonoBehaviour
{
	public Image clothes, skin, detail, eyes, over;

	public void SetCharacterPortrait(ShogunCharacter shogunCharacter, Button button = null)
	{
		clothes.sprite = shogunCharacter.characterClothes;
		skin.sprite = shogunCharacter.characterSkin;
		detail.sprite = shogunCharacter.characterDetail;
		eyes.sprite = shogunCharacter.characterEyes;
		over.sprite = shogunCharacter.characterOver;

		if(button != null)
		{
			ColorBlock block = button.colors;
			block.highlightedColor = GameData.GetColorFromCharacter(shogunCharacter.character);
			block.disabledColor = Skinning.GetSkin(SkinTag.BACKGROUND);
			button.colors = block;
		}

		Show();
	}

	public void Hide()
	{
		clothes.enabled = false;
		skin.enabled = false;
		detail.enabled = false;
		eyes.enabled = false;
		over.enabled = false;
	}

	public void Show()
	{
		clothes.enabled = true;
		skin.enabled = true;
		detail.enabled = true;
		eyes.enabled = true;
		over.enabled = true;
	}
}