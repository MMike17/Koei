using System;
using System.Collections.Generic;
using UnityEngine;

// data of the player
[Serializable]
public class PlayerData : IDebugable
{
	[Header("Debug")]
	// indexes of cards stored in the GameData class
	public List<int> ownedCards;
	// indexes of cards stored in the GameData class
	public List<int> actualDeck;
	// list of weaknesses of the player
	public List<Category> categoryWeaknesses;
	public List<SubCategory> subCategoryWeaknesses;

	IDebugable debugableInterface => (IDebugable) this; // this is for dependency injection
	string IDebugable.debugLabel => "<b>[PlayerData] : </b>";

	// adds Category weakness if it doesn't already has it
	public void AddWeakness(Category toAdd)
	{
		if(!categoryWeaknesses.Contains(toAdd))
		{
			categoryWeaknesses.Add(toAdd);
		}
		else
		{
			Debug.LogWarning(debugableInterface.debugLabel + "Player already has weakness from category " + toAdd.ToString());
		}
	}

	// adds SubCategory weakness if it doesn't already has it
	public void AddWeakness(SubCategory toAdd)
	{
		if(!subCategoryWeaknesses.Contains(toAdd))
		{
			subCategoryWeaknesses.Add(toAdd);
		}
		else
		{
			Debug.LogWarning(debugableInterface.debugLabel + "Player already has weakness from sub-category " + toAdd.ToString());
		}
	}
}