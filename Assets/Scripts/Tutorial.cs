using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour, IDebugable, IInitializable
{
	[Header("Settings")]
	public GameManager.GamePhase phase;
	public KeyCode debug;
	[Space]
	[TextArea]
	public string[] tutos;
	[Header("Assign in Inspector")]
	public GameObject scrollPrefab;

	IInitializable initializableInterface => (IInitializable) this;
	IDebugable debugableInterface => (IDebugable) this;
	Animator panel => GetComponent<Animator>();

	public bool initialized => initializableInterface.initializedInternal;

	string IDebugable.debugLabel => "<b>[" + GetType() + "] : </b>";
	bool IInitializable.initializedInternal { get; set; }

	List<Animator> scrolls;
	Action startGame;
	int actualScroll;

	public void Init(Action startGameCallback)
	{
		startGame = startGameCallback;

		scrolls = new List<Animator>();
		actualScroll = 0;

		foreach (string tuto in tutos)
		{
			GameObject scroll = Instantiate(scrollPrefab, transform);
			scroll.SetActive(false);
			scroll.transform.localScale = Vector3.one * 1.2f;

			scroll.GetComponent<Button>().enabled = false;
			scroll.transform.GetChild(1).GetComponent<Image>().color = Skinning.GetSkin(SkinTag.PRIMARY_ELEMENT);

			TextMeshProUGUI scrollText = scroll.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
			scrollText.text = tuto;
			scrollText.fontSize = 22;
			scrollText.fontStyle = FontStyles.Bold;

			Animator scrollAnimator = scroll.GetComponent<Animator>();
			scrollAnimator.speed = 0;

			scrolls.Add(scrollAnimator);
		}

		if(ShouldSkip())
		{
			panel.Play("Fade");
			Invoke("StartGame", 0.5f);
		}
		else
			StartAnimation(scrolls[0]);

		initializableInterface.InitInternal();
	}

	void IInitializable.InitInternal()
	{
		Debug.Log(debugableInterface.debugLabel + "Initializing done");

		initializableInterface.initializedInternal = true;
	}

	void Update()
	{
		if(!initialized)
			return;

		AnimatorStateInfo current = scrolls[actualScroll].GetCurrentAnimatorStateInfo(0);

		// is animation done
		if(current.normalizedTime >= current.length && Input.GetMouseButtonDown(0))
		{
			// fade panel out
			if(actualScroll >= scrolls.Count - 1)
			{
				panel.Play("Fade");
				Invoke("StartGame", 0.5f);
			}
			else // open next scroll
			{
				actualScroll++;

				StartAnimation(scrolls[actualScroll]);
			}
		}

		if(Input.GetKeyDown(debug))
		{
			panel.Play("Fade");
			Invoke("StartGame", 0.5f);
		}
	}

	bool ShouldSkip()
	{
		switch(phase)
		{
			case GameManager.GamePhase.SHOGUN:
				return GameData.shogunTutorialDone;
			case GameManager.GamePhase.FIGHT:
				return GameData.fightTutorialDone;
		}

		return false;
	}

	void StartGame()
	{
		startGame.Invoke();
		enabled = false;

		switch(phase)
		{
			case GameManager.GamePhase.SHOGUN:
				GameData.shogunTutorialDone = true;
				break;
			case GameManager.GamePhase.FIGHT:
				GameData.fightTutorialDone = true;
				break;
		}
	}

	void StartAnimation(Animator anim)
	{
		anim.gameObject.SetActive(true);
		anim.speed = 1;
		anim.Play("Open");

		Invoke("PlaySound", 0.4f);
	}

	void PlaySound()
	{
		AudioManager.PlaySound("Swish");
	}
}