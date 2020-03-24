using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AttackFightObj))]
public class AttackFightObjConfig : Editor
{
    public override void OnInspectorGUI()
    {
        AttackFightObj obj = target as AttackFightObj;

        /*
        EditorGUILayout.HelpBox("The ID of the object is very important. If you register an object with the same ID, even if it is not the same ScriptableObject, your data will despair or be corrupt. \nDefine a very unique ID.", MessageType.Info);
        
        obj.id = EditorGUILayout.TextField("ID", obj.id, GUILayout.ExpandHeight(true));
        */

        EditorGUILayout.LabelField("Category");
        // Define the current Category into a choice
        string choice = string.Join(",", Enum.GetNames(typeof(Category)));
        // Define each choices
        string[] choices = choice.Split(',');
        // Display the popup
        obj.choiceAttack = EditorGUILayout.Popup(obj.choiceAttack, choices);

        switch (obj.choiceAttack)
        {
            case 0:
                obj.numberOfDialogsWar = EditorGUILayout.IntField("Number of dialogs", obj.numberOfDialogsWar);
                obj.currentDialogChoice = obj.numberOfDialogsWar;
                DisplayList(obj.currentDialogChoice, obj.attackWar, obj);
                break;
            case 1:
                obj.numberOfDialogsFamily = EditorGUILayout.IntField("Number of dialogs", obj.numberOfDialogsFamily);
                obj.currentDialogChoice = obj.numberOfDialogsFamily;
                DisplayList(obj.currentDialogChoice, obj.attackFamily, obj);
                break;
            case 2:
                obj.numberOfDialogsReligion = EditorGUILayout.IntField("Number of dialogs", obj.numberOfDialogsReligion);
                obj.currentDialogChoice = obj.numberOfDialogsReligion;
                DisplayList(obj.currentDialogChoice, obj.attackReligion, obj);
                break;
            case 3:
                obj.numberOfDialogsMoney = EditorGUILayout.IntField("Number of dialogs", obj.numberOfDialogsMoney);
                obj.currentDialogChoice = obj.numberOfDialogsMoney;
                DisplayList(obj.currentDialogChoice, obj.attackMoney, obj);
                break;
        }

    }

    private void DisplayList(int catIndex, List<string> attackType, AttackFightObj obj)
    {
        // Display all options
        for(int i = 0; i < obj.currentDialogChoice; i++)
        {
            EditorGUILayout.LabelField("Dialog " + (i + 1));

            attackType.Add("");

            attackType[i] = EditorGUILayout.TextField(attackType[i]);
        }
    }
}
