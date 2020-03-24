using System;
using UnityEngine;

[Serializable]
public class Clue
{
	public Category category;
	[SerializeField]
	SubCategory subCategory;
	public GeneralDialogue.Character giverCharacter;
	[TextArea(1, 10)]
	public string summary, deductionLine;

	public SubCategory correctedSubCategory => GameData.CorrectSubCategory(subCategory, category);
}