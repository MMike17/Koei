using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(AttackObj))]
public class CustomDialogFight : Editor
{
    
    private int[] selectedCategory;

    private bool getDialogsNbOneTime;

    public override void OnInspectorGUI()
    {
        AttackObj newAttack = target as AttackObj;
        List<string> dialogs = newAttack.buttonSentences;
        
        SubCategory[] newSubCat = newAttack.subCat;

        Debug.Log(newAttack.dialogNumber + " << Cas 1");

        if (!getDialogsNbOneTime)
        {
            newAttack.dialogNumber = EditorPrefs.GetInt("totalDialogs_" + newAttack.cat.ToString());
            getDialogsNbOneTime = true;
        }

        Debug.Log(newAttack.dialogNumber + " << Cas 2");

        newAttack.currentDialogsNumber = newAttack.dialogNumber;

        newAttack.cat = (Category)EditorGUILayout.EnumPopup("Category: ", newAttack.cat);

        newAttack.dialogNumber = EditorGUILayout.IntField("Number of Dialogs: ", newAttack.dialogNumber);
        

        EditorStyles.textField.wordWrap = true;
        
        if (newAttack.subCat.Length < newAttack.currentDialogsNumber)
        {
            List<SubCategory> newList = new List<SubCategory>();
            newList.AddRange(newAttack.subCat);
            for(int i = newAttack.subCat.Length; i < newAttack.dialogNumber; i++)
            {
                newList.Add(GameData.CorrectSubCategory(SubCategory.EMPTY, newAttack.cat));
            }
            newAttack.subCat = newList.ToArray();
        }

        for (int i = 0; i < EditorPrefs.GetInt("totalDialogs_" + newAttack.cat.ToString()); i++)
        {
            dialogs[i] = EditorPrefs.GetString("dialog_" + i + "__" + newAttack.cat.ToString());

            EditorGUILayout.LabelField("Answer " + (i + 1) + ":");
            //EditorGUILayout.BeginHorizontal();
            
            newAttack.subCat[i] = (SubCategory)EditorGUILayout.EnumPopup(newAttack.subCat[i]);


            dialogs[i] = GUILayout.TextArea(dialogs[i]);

            EditorPrefs.SetString("dialog_" + i + "__" + newAttack.cat.ToString(), dialogs[i]);

            //EditorGUILayout.EndHorizontal();
        }
        EditorPrefs.SetInt("totalDialogs_" + newAttack.cat.ToString(), newAttack.dialogNumber);

        EditorUtility.SetDirty(newAttack);

        Debug.Log("<b>Number of buttons >> </b>" + newAttack.currentDialogsNumber);
    }
}
