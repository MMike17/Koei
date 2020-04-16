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
		if(!initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "Not initialized !");
			return;
		}

		StopAll();
		panelMusics.Find(item => { return item.phase == phase; }).Play();
	}

	public void PlayMusic(GamePopup popup)
	{
		if(!initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "Not initialized !");
			return;
		}

		StopAll();
		popupMusics.Find(item => { return item.popup == popup; }).Play();
	}

	public void StopMusic(GamePhase phase)
	{
		if(!initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "Not initialized !");
			return;
		}

		panelMusics.Find(item => { return item.phase == phase; }).Stop();
	}

	public void StopMusic(GamePopup popup)
	{
		if(!initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "Not initialized !");
			return;
		}

		popupMusics.Find(item => { return item.popup == popup; }).Stop();
	}

	public void StopAll()
	{
		panelMusics.ForEach(item => item.Stop());
		popupMusics.ForEach(item => item.Stop());
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

		public void Stop()
		{
			source.Stop();
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

		public void Stop()
		{
			source.Stop();
		}
	}
}