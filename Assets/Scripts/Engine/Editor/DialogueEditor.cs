using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Dialogue))]
public class DialogueEditor : Editor
{
	Dialogue editorTarget;

	public override void OnInspectorGUI()
	{
		editorTarget = target as Dialogue;

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