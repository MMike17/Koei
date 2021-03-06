﻿using System;
using TMPro;
using UnityEngine;

// class managing gameplay of intro panel
public class IntroManager : MonoBehaviour, IDebugable, IInitializable
{
	[Header("Settings")]
	public SkinTag playerText;
	public SkinTag playerHighlight, deityText, deityHighlight;
	public float playerSpeed, deitySpeed;
	public int playerTrail, deityTrail;
	public bool isTesting;
	public TMP_FontAsset deityFont, playerFont;
	public KeyCode debug;
	[Space]
	public IntroDialogue[] dialogues;

	[Header("Assign in Inspector")]
	public DialogueWriter actualWriter;
	public Animator deityAnim, mouseIcon;
	public SkinData test;

	IDebugable debugableInterface => (IDebugable) this;
	IInitializable initializableInterface => (IInitializable) this;

	public bool initialized => initializableInterface.initializedInternal;

	bool IInitializable.initializedInternal { get; set; }
	string IDebugable.debugLabel => "<b>[IntroManager] : </b>";

	Action toShogunCallback;
	int actualDialogue;
	bool isPlayerNext, blockInput, spawnAsked, useCheats;

	void Awake()
	{
		if(!isTesting)
			return;

		actualDialogue = 0;
		blockInput = true;
		isPlayerNext = string.IsNullOrEmpty(dialogues[0].playerLine) ? false : true;

		toShogunCallback = () => Debug.Log(debugableInterface.debugLabel + "Can't change scen in test mode");
		Skinning.Init(test);

		StartDialogue();
	}

	public void Init(bool useCheats, Action toShogun)
	{
		this.useCheats = useCheats;
		actualDialogue = 0;
		blockInput = true;
		isPlayerNext = string.IsNullOrEmpty(dialogues[0].playerLine) ? false : true;

		toShogunCallback = toShogun;

		AudioManager.PlaySound("Rip", StartDialogue);

		mouseIcon.Play("Idle");

		initializableInterface.InitInternal();
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;

		Debug.Log(debugableInterface.debugLabel + "Initializing done");
	}

	void StartDialogue()
	{
		blockInput = false;

		if(string.IsNullOrEmpty(dialogues[0].playerLine))
			SpawnDeityLine();
		else
			SpawnPlayerLine();
	}

	void Update()
	{
		if(Input.GetKeyDown(debug) && useCheats)
			toShogunCallback.Invoke();

		if(!initialized || blockInput)
			return;

		if(actualWriter.isDone)
		{
			if(mouseIcon.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
				mouseIcon.Play("Spawn");
		}
		else
			mouseIcon.Play("Idle");

		if(Input.GetMouseButtonDown(0))
		{
			if(actualWriter.isDone)
			{
				AudioManager.PlaySound("Button");

				if(isPlayerNext)
					SpawnPlayerLine();
				else
					SpawnDeityLine();
			}
			else
				actualWriter.Finish();
		}
	}

	void SpawnPlayerLine()
	{
		if(actualDialogue >= dialogues.Length || string.IsNullOrEmpty(dialogues[actualDialogue].playerLine))
		{
			toShogunCallback.Invoke();
			blockInput = true;
			return;
		}

		FlushWriter();

		if(actualDialogue != 0)
			deityAnim.Play("Hide");

		actualWriter.SetAudio(() => AudioManager.PlaySound("Writting"), () => AudioManager.StopSound("Writting"));
		actualWriter.Play(dialogues[actualDialogue].playerLine, playerSpeed, playerTrail, Skinning.GetSkin(playerHighlight), Skinning.GetSkin(playerText));

		actualWriter.GetComponent<TextMeshProUGUI>().font = playerFont;
		actualWriter.GetComponent<TextMeshProUGUI>().wordSpacing = 5;

		isPlayerNext = false;
		spawnAsked = false;
	}

	void SpawnDeityLine()
	{
		if(actualDialogue >= dialogues.Length || string.IsNullOrEmpty(dialogues[actualDialogue].deityLine))
		{
			toShogunCallback.Invoke();
			blockInput = true;
			return;
		}

		FlushWriter();

		if(actualDialogue == 0)
		{
			deityAnim.Play("Spawn");
			AudioManager.PlaySound("DeityIntro");
		}
		else
			deityAnim.Play("Show");

		actualWriter.SetAudio(() => AudioManager.PlaySound("Writting"), () => AudioManager.StopSound("Writting"));
		actualWriter.Play(dialogues[actualDialogue].deityLine, deitySpeed, deityTrail, Skinning.GetSkin(deityHighlight), Skinning.GetSkin(deityText));

		actualWriter.GetComponent<TextMeshProUGUI>().font = deityFont;
		actualWriter.GetComponent<TextMeshProUGUI>().wordSpacing = 0;

		spawnAsked = false;
		isPlayerNext = true;
		actualDialogue++;
	}

	void FlushWriter()
	{
		if(actualWriter != null)
			actualWriter.Reset();
	}

	[Serializable]
	public class IntroDialogue
	{
		[TextArea]
		public string playerLine, deityLine;
	}
}