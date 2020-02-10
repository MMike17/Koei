using UnityEngine;

// class representing cards in the game
[CreateAssetMenu(fileName = "Card", menuName = "Koei/Card")]
public class Card : ScriptableObject
{
	public enum Type
	{
		ATTACK,
		DEFENSE
	}

	[Header("Settings")]
	public Type type;
	public Category strength;
	public SubCategory subStrength;
	[Space]
	[TextArea]
	public string line;
	[TextArea]
	public string subLine;

	public void Init()
	{
		// corrects subcategory if not done before (it's only safety)
		subStrength = GameData.CorrectSubCategory(subStrength, strength);
	}
}