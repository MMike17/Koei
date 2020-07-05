using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class AudioProjectManager : MonoBehaviour, IDebugable, IInitializable
{
	public List<PanelMusic> panelMusics;
	public List<PopupMusic> popupMusics;

	public bool initialized => initializableInterface.initializedInternal;

	IDebugable debugableInterface => (IDebugable) this;
	IInitializable initializableInterface => (IInitializable) this;

	string IDebugable.debugLabel => "<b>[" + GetType() + "] : </b>";
	bool IInitializable.initializedInternal { get; set; }

	float transitionDuration, popupFadeDuration;
	Func<GamePhase> GetActualGamePhase, GetNextGamePhase;
	Func<GamePopup> GetGamePopup;

	public void Init(float transitionDuration, float popupFadeDuration, Func<GamePhase> getNextGamePhase, Func<GamePhase> getActualGamePhase, Func<GamePopup> getGamePopup)
	{
		this.transitionDuration = transitionDuration;
		this.popupFadeDuration = popupFadeDuration;

		GetNextGamePhase = getNextGamePhase;
		GetActualGamePhase = getActualGamePhase;
		GetGamePopup = getGamePopup;

		panelMusics.ForEach(item => item.Init());
		popupMusics.ForEach(item => item.Init());

		initializableInterface.InitInternal();
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;

		Debug.Log(debugableInterface.debugLabel + "Initializing done");
	}

	public void FadeMusicIn(int layer = 0)
	{
		StartCoroutine(FadePanelMusic(true, layer));
	}

	public void FadeMusicOut(int layer = 0)
	{
		StartCoroutine(FadePanelMusic(false, layer));
	}

	public void PlayPopupMusicIn()
	{
		PopupMusic popup = popupMusics.Find(item => { return item.popup == GetGamePopup(); });

		if(popup == null)
		{
			Debug.LogError(debugableInterface.debugLabel + "There is no PopuMusic for popup " + GetGamePopup());
			return;
		}

		popup.Play();
	}

	public void FadePopupMusicIn()
	{
		StartCoroutine(FadePopupMusic(true));
	}

	public void FadePopupMusicOut()
	{
		StartCoroutine(FadePopupMusic(false));
	}

	public void StopAll()
	{
		if(!initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "Not initialized !");
			return;
		}

		panelMusics.ForEach(item => item.Stop());
	}

	IEnumerator FadePanelMusic(bool inOut, int layer)
	{
		if(!initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "Not initialized !");
			yield break;
		}

		GamePhase current = inOut? GetNextGamePhase() : GetActualGamePhase();

		PanelMusic panel = panelMusics.Find(item => { return item.phase == current; });

		if(panel == null || !panel.IsValid())
		{
			Debug.LogError(debugableInterface.debugLabel + "There is no audio source for panel " + current);
			yield break;
		}

		PanelMusic.Layer selectedLayer = panel.layers.Find(item => { return item.layerIndex == layer; });

		panel.Play(layer);

		float step = inOut ? selectedLayer.maxVolume / transitionDuration : -selectedLayer.maxVolume / transitionDuration;

		while (inOut?selectedLayer.source.volume<selectedLayer.maxVolume : selectedLayer.source.volume> 0)
		{
			selectedLayer.source.volume += step * Time.deltaTime;

			yield return null;
		}

		if(inOut)
			selectedLayer.source.volume = selectedLayer.maxVolume;
		else
		{
			selectedLayer.source.Stop();

			PopupMusic selected = popupMusics.Find(item => { return item.popup.ToString().Contains(GetActualGamePhase().ToString()); });

			if(selected != null)
				selected.Init();
		}

		yield break;
	}

	IEnumerator FadePopupMusic(bool inOut)
	{
		if(!initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "Not initialized !");
			yield break;
		}

		PanelMusic panel = panelMusics.Find(item => { return item.phase == GetActualGamePhase(); });
		PopupMusic popup = popupMusics.Find(item => { return item.popup == GetGamePopup(); });

		if(panel == null || !panel.IsValid())
		{
			Debug.LogError(debugableInterface.debugLabel + "There is no audio source for panel " + GetActualGamePhase());
			yield break;
		}

		if(popup == null)
		{
			Debug.LogError(debugableInterface.debugLabel + "There is no PopuMusic for popup " + GetGamePopup());
			yield break;
		}

		if(inOut)
			FadeMusicOut();
		else
			FadeMusicIn();

		float step = inOut ? popup.maxVolume / transitionDuration : -popup.maxVolume / transitionDuration;

		while (inOut?popup.source.volume<popup.maxVolume : popup.source.volume> 0)
		{
			popup.SetVolume(popup.GetVolume() + step);

			yield return null;
		}

		if(inOut)
			popup.SetVolume(popup.maxVolume);
		else
			popup.Init();

		yield break;
	}

	[Serializable]
	public class PanelMusic
	{
		public GamePhase phase;
		public List<Layer> layers;

		public void Init()
		{
			layers.ForEach(item => item.source.volume = 0);
		}

		public void SetVolume(float level, int index = 0)
		{
			Layer selectedLayer = layers.Find(item => { return item.layerIndex == index; });

			if(selectedLayer != null)
				selectedLayer.source.volume = level;
			else
				Debug.Log("Couldn't play sound of phase " + phase + " at index " + index);
		}

		public void Play(int index)
		{
			Layer selectedLayer = layers.Find(item => { return item.layerIndex == index; });

			if(selectedLayer != null && !selectedLayer.source.isPlaying)
				selectedLayer.source.Play();
			else
				Debug.Log("Couldn't play sound of phase " + phase + " at index " + index);
		}

		public void Stop(int index = 0)
		{
			Layer selectedLayer = layers.Find(item => { return item.layerIndex == index; });

			if(selectedLayer != null)
				selectedLayer.source.Stop();
			else
				Debug.Log("Couldn't play sound of phase " + phase + " at index " + index);
		}

		public bool IsValid()
		{
			bool hasAllSources = true;

			layers.ForEach(item =>
			{
				if(item.source == null)
					hasAllSources = false;
			});

			return hasAllSources;
		}

		[Serializable]
		public class Layer
		{
			public int layerIndex;
			public AudioSource source;
			public float maxVolume;
		}
	}

	[Serializable]
	public class PopupMusic
	{
		public GamePopup popup;
		public AudioSource source;
		public float maxVolume;

		public void Init()
		{
			source.volume = 0;
		}

		public float GetVolume()
		{
			return source.volume;
		}

		public void SetVolume(float level)
		{
			source.volume = level;
		}

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