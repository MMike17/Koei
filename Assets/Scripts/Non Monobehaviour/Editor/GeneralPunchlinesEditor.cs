using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GeneralPunchlines))]
public class GeneralPunchlinesEditor : Editor
{
	GeneralPunchlines editorTarget;

	public override void OnInspectorGUI()
	{
		editorTarget = target as GeneralPunchlines;

		if(!editorTarget.IsValid())
		{
			if(GUILayout.Button("Resets slots"))
			{
				editorTarget.FixPunchlines();
			}
		}

		base.OnInspectorGUI();

	}
}