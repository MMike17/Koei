﻿using System;
using UnityEngine;
using UnityEngine.UI;

// script used for 
public class ConsequencesManager : MonoBehaviour, IDebugable, IInitializable
{
	[Header("Assign in Inspector")]
	public DialogueWriter writer;
	public Animator canvas;

	IDebugable debuguableInterface => (IDebugable) this;
	IInitializable initializableInterface => (IInitializable) this;

	public bool initialized => initializableInterface.initializedInternal;

	bool IInitializable.initializedInternal { get; set; }
	string IDebugable.debugLabel => "<b>[TitleManager] : </b>";

	Action toShogun;
	int combatIndex;
	bool isStarted;

	// add settings callback when we have one
	public void Init(bool gameOver, int combatIndex, Action toShogunCallback, string textToShow = null)
	{
		if(!gameOver && !string.IsNullOrEmpty(textToShow))
			writer.line = textToShow;

		writer.Play();

		isStarted = false;
		this.combatIndex = combatIndex;
		toShogun = toShogunCallback;

		initializableInterface.InitInternal();
	}

	void Update()
	{
		if(writer.isDone)
		{
			if(!isStarted)
			{
				isStarted = true;

				canvas.Play("Fade");

				Invoke("AdvanceMap", 3);
			}
			else if(Input.GetMouseButtonDown(0))
				toShogun.Invoke();
		}
	}

	void AdvanceMap()
	{
		canvas.Play("Show" + combatIndex);
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;
		Debug.Log(debuguableInterface.debugLabel + "Initializing done");
	}
}