using UnityEditor;

[CustomEditor(typeof(Card))]
public class CardEditor : Editor
{
    Card thisCard => (Card) target;

    public override void OnInspectorGUI()
    {
        thisCard.subStrength = GameData.CorrectSubCategory(thisCard.subStrength, thisCard.strength);

        if(thisCard.type == Card.Type.ATTACK)
            thisCard.subLine = string.Empty;

        base.OnInspectorGUI();
    }
}