using System;
using System.Collections.Generic;
using UnityEngine;

// class representing Dialogues
[CreateAssetMenu(fileName = "Dialogue", menuName = "Koei/Dialogue")]
public class Dialogue : ScriptableObject, IDebugable
{
	public enum Character
	{
		SHOGUN,
		TEST1,
		TEST2,
		TEST3
	}

	[Header("Assign in Inspector")]
	[TextArea]
	// first line to appear on the panel
	public string introLine;
	[Space]
	// list of dialogues with characters
	public List<CharacterDialogue> characterDialogues;
	[Space]
	[TextArea]
	// choice that leads to quitting discussion
	public string endLine;

	// how many choices can we show (possible choices -1 + the end choice)
	public int choicesCountToShow { get { return characterDialogues.Count; } }

	IDebugable debugableInterface => (IDebugable) this;

	string IDebugable.debugLabel => "<b>[Dialogue] : </b>";

	void OnDrawGizmos()
	{
		// generate ID's in the inspector for checking
		Init();
	}

	public void Init()
	{
		// resets ID factory (and base index)
		DialogueIDFactory.Init();

		// gives ID to every choice
		characterDialogues.ForEach(choice => { choice.GenerateIndex(); });
	}

	public bool IsCharacterDone(Character selectedCharacter, int dialogueIndex)
	{
		CharacterDialogue selected = characterDialogues.Find(item => { return item.character == selectedCharacter; });

		if(selected == null)
		{
			Debug.LogError(debugableInterface.debugLabel + "Couldn't find dialogue for character " + selectedCharacter.ToString());
			return false;
		}

		return dialogueIndex >= selected.dialogueLines.Length - 1;
	}

	public string[] GetCharacterLines(Character selectedCharacter)
	{
		CharacterDialogue selected = characterDialogues.Find(item => { return item.character == selectedCharacter; });

		if(selected == null)
		{
			Debug.LogError(debugableInterface.debugLabel + "Couldn't find dialogue for character " + selectedCharacter.ToString());
			return null;
		}

		return selected.dialogueLines;
	}

	// checks if all slots are valid
	public bool IsValid()
	{
		bool isValid = true;

		if(characterDialogues == null || characterDialogues.Count < 1)
			return false;

		foreach (Character character in Enum.GetValues(typeof(Character)))
		{
			List<CharacterDialogue> selected = characterDialogues.FindAll(item => { return item.character == character; });

			if(selected.Count != 1)
			{
				isValid = false;
			}
		}

		return isValid;
	}

	// reset slots for editor
	public void FixSlots()
	{
		characterDialogues = new List<CharacterDialogue>();

		// resets slots with all characters
		foreach (Character character in Enum.GetValues(typeof(Character)))
		{
			characterDialogues.Add(new CharacterDialogue(character));
		}
	}
}

[Serializable]
public class CharacterDialogue
{
	// protects index so that they are not overwritten
	public int index { get; private set; }

	// which character it is
	public Dialogue.Character character;
	[TextArea]
	// line displayed in the selection bubble
	public string[] dialogueLines;

	public CharacterDialogue(Dialogue.Character character)
	{
		this.character = character;
	}

	// generates unique ID for player choice
	public void GenerateIndex()
	{
		index = DialogueIDFactory.GetIndex();
	}
}

// class used to generate unique ID's for PlayerChoice's of Dialogue objects
public static class DialogueIDFactory
{
	static int lastIndex;

	// resets indexes
	public static void Init()
	{
		lastIndex = 0;
	}

	// generates a new index
	public static int GetIndex()
	{
		int actualIndex = lastIndex;
		lastIndex++;

		return actualIndex;
	}
}