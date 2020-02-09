using System;
using UnityEngine;
using UnityEngine.UI;

// script used for 
public class TitleManager : MonoBehaviour, IDebugable, IInitializable
{
	[Header("Title")]
	public Button playButton;
	public Button settingsButton, quitButton;

	IDebugable debuguableInterface => (IDebugable) this;
	IInitializable initializableInterface => (IInitializable) this;

	public bool initialized => initializableInterface.initializedInternal;

	bool IInitializable.initializedInternal { get; set; }
	string IDebugable.debugLabel => "<b>[TitleManager] : </b>";

	// add settings callback when we have one
	public void Init(Action playButtonCallback, Action quitButtonCallback)
	{
		if(playButtonCallback != null)
			playButton.onClick.AddListener(playButtonCallback.Invoke);

		if(quitButtonCallback != null)
			quitButton.onClick.AddListener(quitButtonCallback.Invoke);

		initializableInterface.InitInternal();
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;
		Debug.Log(debuguableInterface.debugLabel + "Initializing done");
	}
}