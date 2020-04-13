using System;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

// class used for the whole Shogun phase
[CreateAssetMenu(fileName = "GeneralDialogue", menuName = "Koei/GeneralDialogue")]
public class GeneralDialogue : ScriptableObject, IDebugable, IInitializable
{
	public Enemy assignedEnemy;
	[Header("Conclusions")]
	public List<Conclusion> unlockableConclusions;
	[Header("Dialogue")]
	public List<CharacterDialogue> charactersDialogues;

	public bool initialized => initializableInterface.initializedInternal;

	public enum Character
	{
		SHOGUN,
		FAMILLY,
		RELIGION,
		MONNEY,
		WAR
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

		Debug.Log(debugableInterface.debugLabel + "Initializing done");
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

	public List<Clue> GetAllClues()
	{
		List<Clue> result = new List<Clue>();

		foreach (CharacterDialogue characterDialogue in charactersDialogues)
		{
			foreach (Dialogue dialogue in characterDialogue.initialDialogues)
			{
				// prevents multiple clue spawning
				foreach (Clue clue in GetClue(dialogue))
				{
					if(!result.Contains(clue))
					{
						result.Add(clue);
					}
				}
			}
		}

		return result;
	}

	List<Clue> GetClue(Dialogue dialogue)
	{
		List<Clue> result = new List<Clue>();

		foreach (Dialogue nextDialogue in dialogue.nextDialogues)
		{
			result.AddRange(GetClue(nextDialogue));
		}

		if(dialogue.hasClue)
		{
			result.Add(dialogue.clue);
		}

		return result;
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

		public CharacterDialogue(Character character)
		{
			this.character = character;

			initialDialogues = new List<Dialogue>();
		}

		public void Init()
		{
			indexesPath = new List<int>();
			List<Dialogue> realDialogues = new List<Dialogue>();

			// resets all dialogues
			foreach (Dialogue dialogue in initialDialogues)
			{
				if(dialogue != null)
				{
					realDialogues.Add(dialogue);
					InitDialogue(dialogue);
				}
			}

			initialDialogues = realDialogues;

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
				dialogueInPath = dialogueInPath.nextDialogues[indexesPath[i]];
			}

			return dialogueInPath;
		}

		public bool IsDone()
		{
			bool done = true;

			foreach (Dialogue dialogue in initialDialogues)
			{
				if(!dialogue.IsDone())
					done = false;
			}

			return done;
		}
	}
}