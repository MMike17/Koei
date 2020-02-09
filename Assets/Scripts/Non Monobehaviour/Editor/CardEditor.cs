using UnityEditor;

// this class is used for displaying the "Card" scriptable objects
[CustomEditor(typeof(Card))]
public class CardEditor : Editor
{
	Card thisCard => (Card) target;

	public override void OnInspectorGUI()
	{
		// corrects sub category so that it's always in the right category
		thisCard.subStrength = GameData.CorrectSubCategory(thisCard.subStrength, thisCard.strength);

		// prevents attack cards from having sub line (used for defense cards only)
		if(thisCard.type == Card.Type.ATTACK)
		{
			thisCard.subLine = string.Empty;
		}

		base.OnInspectorGUI();
	}
}