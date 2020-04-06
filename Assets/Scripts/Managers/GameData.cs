using System;
using System.Collections.Generic;
using UnityEngine;

// class representing global game data
public class GameData : MonoBehaviour, IDebugable, IInitializable
{
	[Header("Assign in Inspector")]
	public List<CombatDialogue> combatDialogues;
	[Space]
	public GeneralPunchlines comonPunchlines;

	[Header("Debug")]
	public List<Clue> playerClues;

	public bool initialized => initializableInterface.initializedInternal;

	IInitializable initializableInterface => (IInitializable) this;
	IDebugable debugableInterface => (IDebugable) this;

	bool IInitializable.initializedInternal { get; set; }
	string IDebugable.debugLabel => "<b>[GameData] : </b>";

	public bool gameOver;

	public void Init()
	{
		playerClues = new List<Clue>();

		initializableInterface.InitInternal();
	}

	// called when player finds a clue in a dialogue
	public bool FindClue(Clue clue)
	{
		if(!playerClues.Contains(clue))
		{
			playerClues.Add(clue);
			return true;
		}

		return false;
	}

	// resets clues when starting new Shogun dialogue
	public void ResetPlayerClues()
	{
		playerClues.Clear();
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;
		Debug.Log(debugableInterface.debugLabel + "Initializing done");
	}

	// makes sure a sub-category is in the right category
	public static SubCategory CorrectSubCategory(SubCategory sub, Category parent)
	{
		List<string> listOfSubCategories = new List<string>();

		foreach (string line in Enum.GetNames(typeof(SubCategory)))
		{
			listOfSubCategories.Add(line);
		}

		string compare = parent.ToString();

		string selected = listOfSubCategories.Find(item => { return item.Contains(compare); });

		if(selected == null)
		{
			Debug.LogError("<b>[GameData] : </b>Sub-category " + sub.ToString() + " doesn't have corresponding category");
			return sub;
		}

		int firstIndex = listOfSubCategories.IndexOf(selected);

		selected = listOfSubCategories.FindLast(item => { return item.Contains(compare); });

		if(selected == null)
		{
			Debug.LogError("<b>[GameData] : </b>Sub-category " + sub.ToString() + " doesn't have corresponding category");
			return sub;
		}

		int lastIndex = listOfSubCategories.IndexOf(selected);

		if((int) sub > lastIndex || (int) sub < firstIndex)
		{
			Debug.LogWarning("<b>[GameData] : </b>Sub-category is part of wrong category ( sub is : " + sub.ToString().Split('_') [0] + " but category is : " + parent.ToString() + ")\nSub-category has been corrected to : " + listOfSubCategories[firstIndex]);
			return (SubCategory) firstIndex;
		}
		else
		{
			return sub;
		}
	}

	public static string PrettySubCategory(SubCategory subCategory)
	{
		switch(subCategory)
		{
			case SubCategory.EMPTY:
				return "ERROR";

			case SubCategory.FAMILY_CHILDREN:
				return "Enfants";
			case SubCategory.FAMILY_FIDELITY:
				return "Fidélité";
			case SubCategory.FAMILY_PARENTS:
				return "Parents";
			case SubCategory.FAMILY_SEXUALITY:
				return "Séxualité";
			case SubCategory.FAMILY_SPOUSE:
				return "Epoux";

			case SubCategory.MONEY_CORRUPTION:
				return "Corruption";
			case SubCategory.MONEY_DEBTS:
				return "Dettes";
			case SubCategory.MONEY_EXPENSES:
				return "Dépenses";
			case SubCategory.MONEY_MANAGING:
				return "Managment";
			case SubCategory.MONEY_TAXES:
				return "Taxes";

			case SubCategory.RELIGION_ATHEISM:
				return "Athéisme";
			case SubCategory.RELIGION_DEVOTION:
				return "Dévotion";
			case SubCategory.RELIGION_OCCULTISM:
				return "Occultisme";
			case SubCategory.RELIGION_OFFERING:
				return "Offrande";
			case SubCategory.RELIGION_RESPECT:
				return "Respect";

			case SubCategory.WAR_COURAGE:
				return "Courage";
			case SubCategory.WAR_LOYALTY:
				return "Loyauté";
			case SubCategory.WAR_PLUNDER:
				return "Pillage";
			case SubCategory.WAR_PROTECTION:
				return "Protection";
			case SubCategory.WAR_TALENT:
				return "Talent militaire";
		}

		return null;
	}
}