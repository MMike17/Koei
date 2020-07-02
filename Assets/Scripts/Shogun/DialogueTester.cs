using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GeneralDialogue;

public class DialogueTester : MonoBehaviour, IDebugable
{
	[Header("Settings")]
	public float inputDelay;
	public KeyCode debug;

	[Header("Assign in Inspector")]
	public ShogunManager manager;

	[Header("Debug")]
	public bool shogun;
	public bool money, family, war, religion;

	IDebugable debugableInterface => (IDebugable) this;
	string IDebugable.debugLabel => "<b>[" + GetType() + "] : </b>";

	string lastInvoked;
	bool activeDebug;

	void Awake()
	{
		Application.logMessageReceived += GotErrorEvent;

		activeDebug = false;
		lastInvoked = string.Empty;
	}

	void Update()
	{
		SwitchDebug();
		UpdateBool();

		if(activeDebug)
			InvokeRepeating("ActiveDebug", 0, inputDelay);
	}

	void SwitchDebug()
	{
		if(Input.GetKeyDown(debug))
			activeDebug = !activeDebug;

		if(!activeDebug)
			CancelInvoke("ActiveDebug");
	}

	void ActiveDebug()
	{
		CheckRandomDialogue();

		// forces writers done immediately or closes clue panel if open
		if(!manager.lastWriter.isDone || manager.cluesOpen)
			manager.forceClick = true;

		// selects dialogue choice or changes character
		if(manager.waitForPlayerChoice && manager.characterDone)
			SelectObject(manager.lastSpawnedDialogueObjects);
	}

	void SelectObject(List<GameObject> choices)
	{
		List<Button> available = new List<Button>();

		// gets all dialogue choices not explored yet
		foreach (GameObject choice in choices)
		{
			TextMeshProUGUI text = choice.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

			Dialogue selected = manager.ReturnDialogueFromLine(text.text);

			// doesn't check if done when random
			bool condition = selected != null && !selected.IsDone();

			if(condition)
				available.Add(choice.GetComponent<Button>());
		}

		// selecting a dialogue option
		if(available.Count > 0)
		{
			string text = choices[0].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;

			// must wait
			if(text == lastInvoked)
				return;

			int index = available.Count > 1 ? Random.Range(0, available.Count - 1) : 0;

			Debug.Log(debugableInterface.debugLabel + "(Character : " + manager.actualCharacter + ") Selected path " + (choices.IndexOf(available[index].gameObject) + 1) + " out of " + choices.Count + " choices.");

			// force button click
			available[index].GetComponent<Button>().onClick.Invoke();
			lastInvoked = available[index].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
		}
		else
		{
			if(choices.Count > 0)
			{
				string text = string.Empty;

				if(choices[0].transform.childCount > 0)
					text = choices[0].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;

				// must wait
				if(text == lastInvoked)
					return;
			}

			// get all characters except shogun
			List<ShogunManager.ShogunCharacter> charactersExceptShogun = new List<ShogunManager.ShogunCharacter>();

			manager.characters.ForEach(item =>
			{
				if(item.character != Character.SHOGUN)
					charactersExceptShogun.Add(item);
			});

			int index = Random.Range(0, charactersExceptShogun.Count);

			// select from random character
			charactersExceptShogun[index].selectionButton.onClick.Invoke();

			// Debug.Log(debugableInterface.debugLabel + "Selected character : " + manager.actualCharacter);
		}
	}

	void CheckRandomDialogue()
	{
		bool randomDialogue = true;

		manager.actualDialogue.charactersDialogues.ForEach(item =>
		{
			if(!item.IsDone())
				randomDialogue = false;
		});

		if(randomDialogue)
			Debug.Log(debugableInterface.debugLabel + "No more undone dialogues to check");
	}

	void GotErrorEvent(string message, string stackTrace, LogType type)
	{
		if(type == LogType.Error && stackTrace.Contains(manager.GetType().ToString()) && !stackTrace.Contains("AudioManager"))
		{
			activeDebug = false;

			Debug.Log(debugableInterface.debugLabel + "Found an error while running test");
		}
	}

	void UpdateBool()
	{
		if(manager.actualDialogue == null)
		{
			shogun = false;
			money = false;
			family = false;
			war = false;
			money = false;

			return;
		}

		foreach (CharacterDialogue characterDialogue in manager.actualDialogue.charactersDialogues)
		{
			switch(characterDialogue.character)
			{
				case Character.SHOGUN:
					shogun = characterDialogue.IsDone();
					break;
				case Character.MONEY:
					money = characterDialogue.IsDone();
					break;
				case Character.FAMILLY:
					family = characterDialogue.IsDone();
					break;
				case Character.WAR:
					war = characterDialogue.IsDone();
					break;
				case Character.RELIGION:
					religion = characterDialogue.IsDone();
					break;
			}
		}
	}
}