﻿using UnityEngine;
using UnityEngine.UI;
using static ShogunManager;

public class UICharacter : MonoBehaviour
{
	[Header("Settings")]
	public SkinTag normalTag;
	public SkinTag disabledTag;

	[Header("Assing in Inspector")]
	public Image clothes;
	public Image skin, detail, eyes, over;

	Button selfButton;

	public void SetCharacterPortrait(ShogunCharacter shogunCharacter, Button button = null)
	{
		clothes.sprite = shogunCharacter.characterClothes;
		skin.sprite = shogunCharacter.characterSkin;
		detail.sprite = shogunCharacter.characterDetail;
		eyes.sprite = shogunCharacter.characterEyes;
		over.sprite = shogunCharacter.characterOver;

		if(button != null)
		{
			selfButton = button;

			ColorBlock block = button.colors;
			block.normalColor = Skinning.GetSkin(normalTag);
			block.highlightedColor = GameData.GetColorFromCharacter(shogunCharacter.character);
			block.disabledColor = Skinning.GetSkin(disabledTag);
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

	public void SwitchState()
	{
		selfButton.interactable = !selfButton.interactable;
	}
}