using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(AttackObj))]
public class CustomDialogFight : Editor
{
    
    private int[] selectedCategory;
    
    private int dialogNumber;

    private bool getDialogsNbOneTime;

    public override void OnInspectorGUI()
    {
        AttackObj newAttack = target as AttackObj;
        List<string> dialogs = newAttack.buttonSentences;
        
        SubCategory[] newSubCat = newAttack.subCat;

        if (!getDialogsNbOneTime)
        {
            dialogNumber = EditorPrefs.GetInt("totalDialogs_" + newAttack.cat.ToString());
            getDialogsNbOneTime = true;
        }
        

        newAttack.cat = (Category)EditorGUILayout.EnumPopup("Category: ", newAttack.cat);

        dialogNumber = EditorGUILayout.IntField("Number of Dialogs: ", dialogNumber);
        

        EditorStyles.textField.wordWrap = true;
        
        if (newAttack.subCat.Length < dialogNumber)
        {
            List<SubCategory> newList = new List<SubCategory>();
            newList.AddRange(newAttack.subCat);
            for(int i = newAttack.subCat.Length; i < dialogNumber; i++)
            {
                newList.Add(GameData.CorrectSubCategory(SubCategory.EMPTY, newAttack.cat));
            }
            newAttack.subCat = newList.ToArray();
        }

        for (int i = 0; i < dialogNumber; i++)
        {
            dialogs[i] = EditorPrefs.GetString("dialog_" + i + "__" + newAttack.cat.ToString());

            EditorGUILayout.LabelField("Answer " + (i + 1) + ":");
            //EditorGUILayout.BeginHorizontal();
            
            newAttack.subCat[i] = (SubCategory)EditorGUILayout.EnumPopup(newAttack.subCat[i]);


            dialogs[i] = GUILayout.TextArea(dialogs[i]);

            EditorPrefs.SetString("dialog_" + i + "__" + newAttack.cat.ToString(), dialogs[i]);

            //EditorGUILayout.EndHorizontal();
        }
        EditorPrefs.SetInt("totalDialogs_" + newAttack.cat.ToString(), dialogNumber);
        EditorUtility.SetDirty(newAttack);
    }
}
