﻿using System;
using System.Collections.Generic;
using UnityEngine;

// class representing global game data
public class GameData : MonoBehaviour, IDebugable
{
	[Header("Assign in Inspector")]
	public List<Card> cardList;

	[Header("Debug")]
	public PlayerData playerData;

	public IDebugable debugableInterface => (IDebugable) this;

	string IDebugable.debugLabel => "<b>[GameData] : </b>";

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
}