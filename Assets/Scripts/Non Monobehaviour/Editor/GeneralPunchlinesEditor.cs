using System.IO;
using UnityEditor;
using UnityEngine;
using static GeneralPunchlines;

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
				editorTarget.FixPunchlines();
		}

		if(GUILayout.Button("Export punchlines"))
		{
			ExtractPunchlines();
			Debug.Log("Punchlines extracted");
		}

		base.OnInspectorGUI();
	}

	void ExtractPunchlines()
	{
		string path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.dataPath), System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(editorTarget)), "punchlines.txt");

		string content = "Punchlines : \n\n\n";

		foreach (CategoryPunchlines categoryPunchlines in editorTarget.allPunchlines)
		{
			content += "Catégorie : " + categoryPunchlines.category.ToString() + "\n";

			foreach (Punchline punchline in categoryPunchlines.punchlines)
				content += "\n" + punchline.subCategory.ToString() + " : " + punchline.line;

			content += "\n\n";
		}

		content = content.Remove(content.Length - 2);

		File.WriteAllText(path, content);

		AssetDatabase.Refresh();
	}
}