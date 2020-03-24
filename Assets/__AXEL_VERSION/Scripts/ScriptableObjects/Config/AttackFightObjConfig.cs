using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AttackFightObj))]
public class AttackFightObjConfig : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("The ID of the object is very important. If you register an object with the same ID, even if it is not the same ScriptableObject, your data will despair or be corrupt. \nDefine a very unique ID.", MessageType.Info);

        AttackFightObj obj = target as AttackFightObj;

        obj.id = EditorGUILayout.TextField("ID", obj.id, GUILayout.ExpandHeight(true));
        
        EditorGUILayout.LabelField("Category");
        obj.choiceAttack = EditorGUILayout.Popup(obj.choiceAttack, obj.attackArray);

        
    }
}
