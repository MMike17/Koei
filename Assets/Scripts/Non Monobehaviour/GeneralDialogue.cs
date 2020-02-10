using System;
using System.Collections.Generic;
using UnityEngine;

// class used for the whole Shogun phase
[CreateAssetMenu(fileName = "GeneralDialogue", menuName = "Koei/GeneralDialogue")]
public class GeneralDialogue : ScriptableObject, IDebugable, IInitializable
{
	// TODO : actually add the weaknesses to PlayerData
	[Header("Weaknesses")]
	public List<SubCategory> criticalWeaknessesForPlayer;
	public List<Category> weaknessesForPlayer;
	[Header("Dialogue")]
	public List<CharacterDialogue> charactersDialogues;

	public bool initialized => initializableInterface.initializedInternal;

	public enum Character
	{
		SHOGUN,
		TEST1,
		TEST2,
		TEST3
	}

	IInitializable initializableInterface => (IInitializable) this;
	IDebugable debugableInterface => (IDebugable) this;

	bool IInitializable.initializedInternal { get; set; }
	string IDebugable.debugLabel => "<b>[Dialogue] : </b>";

	public void Init()
	{
		charactersDialogues.ForEach(item => item.Init());

		initializableInterface.InitInternal();
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;

		Debug.Log(debugableInterface.debugLabel + "Initialized");
	}

	// gets CharacterDialogue from Character variable
	public CharacterDialogue GetCharacterDialogue(Character character)
	{
		CharacterDialogue selected = charactersDialogues.Find(item => { return item.character == character; });

		if(selected == null)
		{
			Debug.LogError(debugableInterface.debugLabel + "Couldn't find CharacterDialogue with character " + character.ToString());
			return null;
		}

		return selected;
	}

	// editor checks if all slots are valid
	public bool IsValid()
	{
		bool isValid = true;

		if(charactersDialogues == null || charactersDialogues.Count < 1)
			return false;

		foreach (Character character in Enum.GetValues(typeof(Character)))
		{
			List<CharacterDialogue> selected = charactersDialogues.FindAll(item => { return item.character == character; });

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
		charactersDialogues = new List<CharacterDialogue>();

		// resets slots with all characters
		foreach (Character character in Enum.GetValues(typeof(Character)))
		{
			charactersDialogues.Add(new CharacterDialogue(character));
		}
	}

	// class representing character's dialogues
	[Serializable]
	public class CharacterDialogue : IInitializable
	{
		public Character character;
		[TextArea(1, 10)]
		public string firstLine;
		public List<Dialogue> initialDialogues;

		[Header("Debug")]
		public List<int> indexesPath;

		public bool initialized => initializableInterface.initializedInternal;

		IInitializable initializableInterface => (IInitializable) this;

		bool IInitializable.initializedInternal { get; set; }

		bool mainDone;

		public CharacterDialogue(Character character)
		{
			this.character = character;

			initialDialogues = new List<Dialogue>();
		}

		public void Init()
		{
			mainDone = false;
			indexesPath = new List<int>();

			// resets all dialogues
			foreach (Dialogue dialogue in initialDialogues)
			{
				InitDialogue(dialogue);
			}

			initializableInterface.InitInternal();
		}

		// method to recursively reset dialogues
		void InitDialogue(Dialogue dialogue)
		{
			// resets current dialogue
			dialogue.Reset();

			// calls method to reset on all next dialogues
			foreach (Dialogue nextDialogue in dialogue.nextDialogues)
			{
				InitDialogue(nextDialogue);
			}
		}

		void IInitializable.InitInternal()
		{
			initializableInterface.initializedInternal = true;
		}

		// called when player selects dialogue choice
		public void MoveToDialogue(int selectedIndex)
		{
			indexesPath.Add(selectedIndex);

			GetActualDialogue().SetAsDone();
		}

		public Dialogue GetActualDialogue()
		{
			Dialogue dialogueInPath = initialDialogues[indexesPath[0]];

			// gets the right dialogue in the arborescence
			for (int i = 1; i < indexesPath.Count; i++)
			{
				dialogueInPath = dialogueInPath.nextDialogues[i];
			}

			return dialogueInPath;
		}
	}
}