using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player_Attack_Family", menuName = "Koei/Battle/Player Attacks Family")]
public class AttackObj : ScriptableObject
{
    [TextArea]
    [SerializeField]
    public List<string> buttonSentences;

    [HideInInspector]
    public int dialogNumber;

    [HideInInspector]
    public int currentDialogsNumber;

    [SerializeField]
    public Category cat;
    [SerializeField]
    public SubCategory[] subCat;

}
