using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// represents the buttons the player is going to click on to select a character to talk to on the Shogun panel
public class ShogunChoice : MonoBehaviour, IInitializable, IDebugable
{
	[Header("Assing in Inspector")]
	public Button choiceButton;
	public Image characterPortrait;

	IDebugable debugableInterface => (IDebugable) this;
	IInitializable initializableInterface => (IInitializable) this;

	public bool initialized => initializableInterface.initializedInternal;

	string IDebugable.debugLabel => "<b>[ShogunChoice] : </b>";
	bool IInitializable.initializedInternal { get; set; }

	// retrieves a lot of data to adjust behaviour
	public void Init(Sprite portrait, Color highlightColor, Color pressedColor, Color hideColor, Action selected)
	{
		// set up button colors
		ColorBlock buttonColors = new ColorBlock();
		buttonColors.normalColor = hideColor;
		buttonColors.highlightedColor = highlightColor;
		buttonColors.pressedColor = pressedColor;

		choiceButton.colors = buttonColors;

		// sets action to button
		choiceButton.onClick.RemoveAllListeners();
		choiceButton.onClick.AddListener(selected.Invoke);

		characterPortrait.sprite = portrait;

		gameObject.SetActive(true);

		initializableInterface.InitInternal();
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;
		Debug.Log(debugableInterface.debugLabel + "Initializing done");
	}

	// resets the panel and hides it
	public void HidePanel()
	{
		gameObject.SetActive(false);

		initializableInterface.initializedInternal = false;
	}
}