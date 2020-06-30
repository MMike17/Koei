using System;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static GeneralDialogue;

// class representing global game data
public class GameData : MonoBehaviour, IDebugable, IInitializable
{
	[Header("Assign in Inspector")]
	public List<EnemyBundle> enemyContent;

	[Header("Debug")]
	public List<Clue> playerClues;

	public static bool shogunTutorialDone, fightTutorialDone;

	public bool initialized => initializableInterface.initializedInternal;

	public enum GameState
	{
		NORMAL,
		GAME_OVER_GENERAL,
		GAME_OVER_FINISHER,
		RETURN_GENERAL,
		RETURN_FINISHER
	}

	IInitializable initializableInterface => (IInitializable) this;
	IDebugable debugableInterface => (IDebugable) this;

	bool IInitializable.initializedInternal { get; set; }
	string IDebugable.debugLabel => "<b>[GameData] : </b>";

	public void Init()
	{
		playerClues = new List<Clue>();

		enemyContent.ForEach(item => item.Init());

		shogunTutorialDone = false;
		fightTutorialDone = false;

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

	public static string PrettyCategory(Category category)
	{
		switch(category)
		{
			case Category.EMPTY:
				return "ERROR";
			case Category.FAMILY:
				return "Famille";
			case Category.MONEY:
				return "Argent";
			case Category.RELIGION:
				return "Religion";
			case Category.WAR:
				return "Guerre";
		}

		return null;
	}

	public static Category GetCategoryFromSubCategory(SubCategory subCategory)
	{
		foreach (Category category in Enum.GetValues(typeof(Category)))
		{
			if(subCategory.ToString().Contains(category.ToString()))
				return category;
		}

		return Category.EMPTY;
	}

	public static Color GetColorFromCharacter(Character character)
	{
		switch(character)
		{
			case Character.FAMILLY:
				return GetColorFromCategory(Category.FAMILY);
			case Character.MONEY:
				return GetColorFromCategory(Category.MONEY);
			case Character.RELIGION:
				return GetColorFromCategory(Category.RELIGION);
			case Character.SHOGUN:
				return GetColorFromCategory(Category.EMPTY);
			case Character.WAR:
				return GetColorFromCategory(Category.WAR);
		}

		return default(Color);
	}

	public static Color GetColorFromCategory(Category category)
	{
		return GameManager.Get.colors.Find(item => { return item.category == category; }).color;
	}

	public static Color LerpColorHSV(Color color, float newHue = 0, float newSaturation = 0, float newValue = 0)
	{
		Color.RGBToHSV(color, out float h, out float s, out float v);

		h += newHue;
		s += newSaturation;
		v += newValue;

		return Color.HSVToRGB(h, s, v);
	}

	[Serializable]
	public class EnemyBundle
	{
		public Enemy enemy;
		public CombatDialogue combatDialogue;
		public GeneralPunchlines punchlines;
		public GeneralDialogue shogunDialogue;

		public void Init()
		{
			combatDialogue.Init();
			punchlines.Init();
		}
	}
}