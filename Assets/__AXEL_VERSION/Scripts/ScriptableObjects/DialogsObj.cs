using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialog", menuName = "Koei/Battle/Dialog")]
public class DialogsObj : ScriptableObject
{
    [Header("- - - PLAYER - - -")]
    [TextArea]
    public string[] dialogsPlayer;

    [Header("- - - ENEMY - - -")]
    [TextArea]
    public string[] dialogsEnemy;
}
