using UnityEngine;

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
		subStrength = GameData.CorrectSubCategory(subStrength, strength);
	}
}