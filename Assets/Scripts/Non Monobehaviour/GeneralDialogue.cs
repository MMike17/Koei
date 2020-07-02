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
	[Space]
	[TextArea]
	public string goodDeityFeedback, badDeityFeedback;
	[Header("Dialogue")]
	public List<CharacterDialogue> charactersDialogues;
	[Space]
	public CharacterDialogue shogunReturnDialogues;

	public bool initialized => initializableInterface.initializedInternal;

	public enum Character
	{
		SHOGUN,
		FAMILLY,
		RELIGION,
		MONEY,
		WAR
	}

	IInitializable initializableInterface => (IInitializable) this;
	IDebugable debugableInterface => (IDebugable) this;

	bool IInitializable.initializedInternal { get; set; }
	string IDebugable.debugLabel => "<b>[Dialogue] : </b>";

	public void Init()
	{
		charactersDialogues.ForEach(item => item.Init());

		shogunReturnDialogues.Init();

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
				isValid = false;
		}

		return isValid;
	}

	// reset slots for editor
	public void FixSlots()
	{
		charactersDialogues = new List<CharacterDialogue>();

		// resets slots with all characters
		foreach (Character character in Enum.GetValues(typeof(Character)))
			charactersDialogues.Add(new CharacterDialogue(character));
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
						result.Add(clue);
				}
			}
		}

		return result;
	}

	List<Clue> GetClue(Dialogue dialogue)
	{
		List<Clue> result = new List<Clue>();

		foreach (Dialogue nextDialogue in dialogue.nextDialogues)
			result.AddRange(GetClue(nextDialogue));

		if(dialogue.hasClue)
			result.Add(dialogue.clue);

		return result;
	}

	// class representing character's dialogues
	[Serializable]
	public class CharacterDialogue : IInitializable, IDebugable
	{
		public Character character;
		[TextArea(1, 10)]
		public string firstLine;
		public List<Dialogue> initialDialogues;

		[Header("Debug")]
		public List<int> indexesPath;

		public bool initialized => initializableInterface.initializedInternal;

		IInitializable initializableInterface => (IInitializable) this;
		IDebugable debugableInterface => (IDebugable) this;

		bool IInitializable.initializedInternal { get; set; }

		string IDebugable.debugLabel => "<b>[" + GetType() + "] :</b>";

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
				InitDialogue(nextDialogue);
		}

		void IInitializable.InitInternal()
		{
			initializableInterface.initializedInternal = true;
		}

		// resets dialogue path when we change character
		public void ResetDialoguePath()
		{
			indexesPath.Clear();
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
				dialogueInPath = dialogueInPath.nextDialogues[indexesPath[i]];

			return dialogueInPath;
		}

		public void ForceSetAsDone()
		{
			foreach (Dialogue dialogue in initialDialogues)
				SetDialogueDoneRecursive(dialogue);
		}

		public bool IsDone()
		{
			foreach (Dialogue dialogue in initialDialogues)
			{
				if(!CheckDialogueRecursive(dialogue))
					return false;
			}

			return true;
		}

		public Dialogue FindDialogueWithLine(string line)
		{
			foreach (Dialogue dialogue in initialDialogues)
			{
				Dialogue result = FindDialogueWithLineRecursive(dialogue, line);

				if(result != null)
					return result;
			}

			return null;
		}

		Dialogue FindDialogueWithLineRecursive(Dialogue initial, string line)
		{
			if(initial.playerQuestion == line)
				return initial;

			if(initial.nextDialogues.Length > 0)
			{
				foreach (Dialogue dialogue in initial.nextDialogues)
				{
					Dialogue found = FindDialogueWithLineRecursive(dialogue, line);

					if(found != null)
						return found;
				}
			}

			return null;
		}

		void SetDialogueDoneRecursive(Dialogue dialogue)
		{
			dialogue.SetAsDone();

			if(dialogue.nextDialogues != null && dialogue.nextDialogues.Length > 0)
			{
				foreach (Dialogue next in dialogue.nextDialogues)
					SetDialogueDoneRecursive(next);
			}
		}

		bool CheckDialogueRecursive(Dialogue dialogue)
		{
			if(dialogue.nextDialogues == null || dialogue.nextDialogues.Length == 0)
				return dialogue.IsDone();
			else
			{
				foreach (Dialogue nextDialogue in dialogue.nextDialogues)
				{
					if(!CheckDialogueRecursive(nextDialogue))
						return false;
				}

				return true;
			}
		}
	}
}