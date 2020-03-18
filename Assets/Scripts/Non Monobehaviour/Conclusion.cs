using System;
using UnityEngine;

// class to unlock cards visualy
[Serializable]
public class Conclusion
{
	public Category category;
	[SerializeField]
	SubCategory subCategory;
	[HideInInspector]
	public DesignedCard cardObject;

	public SubCategory correctedSubCategory => GameData.CorrectSubCategory(subCategory, category);

	public Conclusion(Category category, SubCategory subCategory, DesignedCard card)
	{
		this.category = category;
		this.subCategory = subCategory;

		cardObject = card;
	}
}