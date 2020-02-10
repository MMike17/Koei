using System;
using UnityEngine;

[Serializable]
public class Clue
{
	public Category category;
	[SerializeField]
	SubCategory subCategory;
	[TextArea(1, 10)]
	public string summary;

	public SubCategory correctedSubCategory => GameData.CorrectSubCategory(subCategory, category);
}