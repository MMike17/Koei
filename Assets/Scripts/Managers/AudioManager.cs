using System;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class AudioManager : MonoBehaviour, IDebugable, IInitializable
{
	public List<PanelMusic> panelMusics;
	public List<PopupMusic> popupMusics;

	public bool initialized => initializableInterface.initializedInternal;

	IDebugable debugableInterface => (IDebugable) this;
	IInitializable initializableInterface => (IInitializable) this;

	string IDebugable.debugLabel => "<b>[AudioManager] : </b>";
	bool IInitializable.initializedInternal { get; set; }

	public void Init()
	{
		initializableInterface.InitInternal();
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;

		Debug.Log(debugableInterface.debugLabel + "Initializing done");
	}

	public void PlayMusic(GamePhase phase)
	{
		panelMusics.Find(item => { return item.phase == phase; }).Play();
	}

	public void PlayMusic(GamePopup popup)
	{
		popupMusics.Find(item => { return item.popup == popup; }).Play();
	}

	[Serializable]
	public class PanelMusic
	{
		public GamePhase phase;
		public AudioSource source;

		public void Play()
		{
			source.Play();
		}
	}

	[Serializable]
	public class PopupMusic
	{
		public GamePopup popup;
		public AudioSource source;

		public void Play()
		{
			source.Play();
		}
	}
}