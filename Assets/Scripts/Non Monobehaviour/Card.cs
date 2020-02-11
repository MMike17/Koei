using UnityEngine;

// class representing cards in the game
[CreateAssetMenu(fileName = "Card", menuName = "Koei/Card")]
public class Card : ScriptableObject
{
	[Header("Settings")]
	public Category strength;
	public SubCategory subStrength;
	[Space]
	[TextArea]
	public string line;

	public void Init()
	{
		// corrects subcategory if not done before (it's only safety)
		subStrength = GameData.CorrectSubCategory(subStrength, strength);
	}
}