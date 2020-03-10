using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RemoveEditorPrefs : Editor
{

    [MenuItem("Toolbox/EditorPrefs/Remove All")]
    public static void RemoveAll()
    {
        EditorPrefs.DeleteAll();
    }

}
