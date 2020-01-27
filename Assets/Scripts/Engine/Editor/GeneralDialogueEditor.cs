using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GeneralDialogue))]
public class GeneralDialogueEditor : Editor
{
	GeneralDialogue editorTarget;

	public override void OnInspectorGUI()
	{
		editorTarget = target as GeneralDialogue;

		if(!editorTarget.IsValid())
		{
			if(GUILayout.Button("Reset slots"))
			{
				editorTarget.FixSlots();
			}
		}

		base.OnInspectorGUI();
	}

}