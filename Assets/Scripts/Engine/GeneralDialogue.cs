using System;
using System.Collections.Generic;
using UnityEngine;

// class used for the whole Shogun phase
[CreateAssetMenu(fileName = "GeneralDialogue", menuName = "Koei/GeneralDialogue")]
public class GeneralDialogue : ScriptableObject, IDebugable
{
	// TODO : actually add the weaknesses to PlayerData
	[Header("Weaknesses")]
	public List<SubCategory> criticalWeaknessesForPlayer;
	public List<Category> weaknessesForPlayer;
	[Header("Dialogue")]
	public List<CharacterDialogue> charactersDialogues;

	public enum Character
	{
		SHOGUN,
		TEST1,
		TEST2,
		TEST3
	}

	IDebugable debugableInterface => (IDebugable) this;

	string IDebugable.debugLabel => "<b>[Dialogue] : </b>";

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

	// checks if all slots are valid
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
		public List<Dialogue> mainDialogue;
		public List<AdditionnalDialogue> additionalDialogue;

		public bool initialized => initializableInterface.initializedInternal;

		IInitializable initializableInterface => (IInitializable) this;

		bool IInitializable.initializedInternal { get; set; }

		bool mainDone;

		public CharacterDialogue(Character character)
		{
			this.character = character;

			mainDialogue = new List<Dialogue>();
			additionalDialogue = new List<AdditionnalDialogue>();
		}

		public void Init()
		{
			mainDone = false;

			initializableInterface.InitInternal();
		}

		public void MarkMainAsDone()
		{
			mainDone = true;
		}

		public bool IsMainDone()
		{
			return mainDone;
		}

		void IInitializable.InitInternal()
		{
			initializableInterface.initializedInternal = true;
		}

		// class representing an exchange between the player and the character
		[Serializable]
		public class Dialogue
		{
			[TextArea(1, 10)]
			public string playerQuestion;
			[TextArea(1, 10)]
			public string characterAnswer;
		}

		// class representing an additionnal exchange that can be unlocked
		[Serializable]
		public class AdditionnalDialogue : Dialogue
		{
			public Trigger trigger;

			// class representing conditions to unlock additionnal dialogues
			[Serializable]
			public class Trigger
			{
				public Character character;
				public bool isMainDialogue;
				public int additionalDialogueIndex; // which additionnal dialogue unlocks the additionnal dialogue

				// does the provided mark unlocks the additionnal dialogue
				public bool IsUnlocked(Mark mark)
				{
					if(isMainDialogue)
					{
						return mark.character == character && mark.mainDialogueDone == isMainDialogue;
					}
					else
					{
						return mark.character == character && mark.lastAdditionnalDialogueIndex == additionalDialogueIndex;
					}
				}
			}
		}
	}

	// class representing marks that the player will keep to say where he is in which dialogue tree
	// (match these to the trigger)
	[Serializable]
	public class Mark
	{
		public Character character;
		public bool mainDialogueDone;
		public int lastAdditionnalDialogueIndex;

		public Mark(Character character, bool mainDialogue, int additionnalIndex)
		{
			this.character = character;
			mainDialogueDone = mainDialogue;
			lastAdditionnalDialogueIndex = additionnalIndex;
		}
	}
}