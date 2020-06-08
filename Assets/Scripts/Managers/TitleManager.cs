using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour, IDebugable, IInitializable
{
	[Header("Settings")]
	public SkinTag buttonNormal;
	public SkinTag buttonHighlight, buttonPressed;

	[Header("Assing in Inspector")]
	public Button playButton;
	public Button quitButton;

	IDebugable debuguableInterface => (IDebugable) this;
	IInitializable initializableInterface => (IInitializable) this;

	public bool initialized => initializableInterface.initializedInternal;

	bool IInitializable.initializedInternal { get; set; }
	string IDebugable.debugLabel => "<b>[TitleManager] : </b>";

	// add settings callback when we have one
	public void Init(Action playButtonCallback, Action quitButtonCallback)
	{
		if(playButtonCallback != null)
		{
			playButtonCallback += () => AudioManager.PlaySound("Button");
			playButton.onClick.AddListener(playButtonCallback.Invoke);

			ColorBlock block = new ColorBlock()
			{
				normalColor = Skinning.GetSkin(buttonNormal),
					highlightedColor = Skinning.GetSkin(buttonHighlight),
					pressedColor = Skinning.GetSkin(buttonPressed),
					colorMultiplier = 1,
					fadeDuration = 0.1f
			};

			playButton.colors = block;
		}

		if(quitButtonCallback != null)
		{
			quitButtonCallback += () => AudioManager.PlaySound("Button");
			quitButton.onClick.AddListener(quitButtonCallback.Invoke);

			ColorBlock block = new ColorBlock()
			{
				normalColor = Skinning.GetSkin(buttonNormal),
					highlightedColor = Skinning.GetSkin(buttonHighlight),
					pressedColor = Skinning.GetSkin(buttonPressed),
					colorMultiplier = 1,
					fadeDuration = 0.1f
			};

			quitButton.colors = block;
		}

		initializableInterface.InitInternal();
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;
		Debug.Log(debuguableInterface.debugLabel + "Initializing done");
	}
}