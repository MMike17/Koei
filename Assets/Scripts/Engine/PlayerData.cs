using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData : IDebugable
{
	[Header("Debug")]
	public List<int> ownedCards;
	public List<int> actualDeck;
	public List<Category> categoryWeaknesses;
	public List<SubCategory> subCategoryWeaknesses;

	IDebugable debugableInterface => (IDebugable) this;
	public string debugLabel => "<b>[PlayerData] : </b>";

	public void AddWeakness(Category toAdd)
	{
		if(!categoryWeaknesses.Contains(toAdd))
			categoryWeaknesses.Add(toAdd);
		else
			Debug.LogWarning(debugLabel + "Player already has weakness from category " + toAdd.ToString());
	}

	public void AddWeakness(SubCategory toAdd)
	{
		if(!subCategoryWeaknesses.Contains(toAdd))
			subCategoryWeaknesses.Add(toAdd);
		else
			Debug.LogWarning(debugLabel + "Player already has weakness from sub-category " + toAdd.ToString());
	}
}