using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

class StartOnScene : EditorWindow
{
	static int sceneBuildIndex;

	[MenuItem("Tools/Scene start setup")]
	private static void ShowWindow()
	{
		StartOnScene window = GetWindow<StartOnScene>();
		window.minSize = new Vector2(180, 20);
		window.maxSize = new Vector2(180, 20);
		window.titleContent = new GUIContent("Scene start setup");
		window.Show();
	}

	private void OnGUI()
	{
		sceneBuildIndex = EditorGUILayout.IntField("Scene build index to start from", sceneBuildIndex);
	}

	[MenuItem("Edit/Play from scene %0", false, 160)]
	public static void PlayFromScene0()
	{
		if(EditorApplication.isPlaying)
		{
			EditorApplication.isPlaying = false;
			return;
		}

		EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

		EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(sceneBuildIndex), OpenSceneMode.Single);

		EditorApplication.isPlaying = true;
	}
}