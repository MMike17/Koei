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

	public void FadeMusicIn()
	{
		StartCoroutine(FadePanelMusic(true));
	}

	public void FadeMusicOut()
	{
		StartCoroutine(FadePanelMusic(false));
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

	IEnumerator FadePanelMusic(bool inOut)
	{
		if(!initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "Not initialized !");
			yield break;
		}

		GamePhase current = inOut? GetNextGamePhase() : GetActualGamePhase();

		PanelMusic panel = panelMusics.Find(item => { return item.phase == current; });

		if(panel == null || panel.source == null)
		{
			Debug.LogError(debugableInterface.debugLabel + "There is no audio source for panel " + current);
			yield break;
		}

		if(!panel.source.isPlaying)
			panel.source.Play();

		float step = inOut ? panel.maxVolume / transitionDuration : -panel.maxVolume / transitionDuration;

		while (inOut?panel.source.volume<panel.maxVolume : panel.source.volume> 0)
		{
			panel.source.volume += step * Time.deltaTime;

			yield return null;
		}

		if(inOut)
			panel.source.volume = panel.maxVolume;
		else
		{
			panel.source.Stop();

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

		if(panel == null || panel.source == null)
		{
			Debug.LogError(debugableInterface.debugLabel + "There is no audio source for panel " + GetActualGamePhase());
			yield break;
		}

		if(popup == null)
		{
			Debug.LogError(debugableInterface.debugLabel + "There is no PopuMusic for popup " + GetGamePopup());
			yield break;
		}

		if(!panel.source.isPlaying)
			panel.source.Play();

		float stepHigh = inOut ? popup.maxHighFilter / popupFadeDuration : -popup.maxHighFilter / popupFadeDuration;
		float stepLow = inOut ? -popup.maxLowFilter / popupFadeDuration : popup.maxLowFilter / popupFadeDuration;

		bool check = inOut?(popup.highFilter.cutoffFrequency < popup.maxHighFilter && popup.lowFilter.cutoffFrequency > popup.maxLowFilter): (popup.highFilter.cutoffFrequency > 10 && popup.lowFilter.cutoffFrequency < 22000);

		while (check)
		{
			popup.highFilter.cutoffFrequency += stepHigh * Time.deltaTime;
			popup.lowFilter.cutoffFrequency += stepLow * Time.deltaTime;

			check = inOut?(popup.highFilter.cutoffFrequency < popup.maxHighFilter && popup.lowFilter.cutoffFrequency > popup.maxLowFilter): (popup.highFilter.cutoffFrequency > 10 && popup.lowFilter.cutoffFrequency < 22000);

			yield return null;
		}

		if(inOut)
		{
			popup.highFilter.cutoffFrequency = popup.maxHighFilter;
			popup.lowFilter.cutoffFrequency = popup.maxLowFilter;
		}
		else
			popup.Init();

		yield break;
	}

	[Serializable]
	public class PanelMusic
	{
		public GamePhase phase;
		public AudioSource source;
		public float maxVolume;

		public void Init()
		{
			source.volume = 0;
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

	[Serializable]
	public class PopupMusic
	{
		public GamePopup popup;
		public AudioHighPassFilter highFilter;
		public float maxHighFilter;
		public AudioLowPassFilter lowFilter;
		public float maxLowFilter;

		public void Init()
		{
			highFilter.cutoffFrequency = 10;
			lowFilter.cutoffFrequency = 22000;
		}
	}
}