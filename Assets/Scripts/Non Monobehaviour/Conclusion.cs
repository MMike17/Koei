using System;
using UnityEngine;

// class to unlock cards visualy
[Serializable]
public class Conclusion
{
	public Category category;
	[SerializeField]
	SubCategory subCategory;
	[TextArea]
	public string comment;
	[HideInInspector]
	public ConclusionCard cardObject;

	public SubCategory correctedSubCategory => GameData.CorrectSubCategory(subCategory, category);

	public Conclusion(Category category, SubCategory subCategory, ConclusionCard card)
	{
		this.category = category;
		this.subCategory = subCategory;

		cardObject = card;
	}
}