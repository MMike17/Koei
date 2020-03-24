using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new_battle_attacks", menuName = "Koei/Battle/Battle Attack")]
public class AttackFightObj : ScriptableObject
{
    [Tooltip("Add an ID to save data's object")]
    [SerializeField]
    public string id = "aaaa_0000_zzzz";

    [SerializeField]
    public int choiceAttack;
    [SerializeField]
    public int numberOfDialogsWar;
    [SerializeField]
    public int numberOfDialogsFamily;
    [SerializeField]
    public int numberOfDialogsReligion;
    [SerializeField]
    public int numberOfDialogsMoney;


    [SerializeField]
    public int currentDialogChoice;

    [SerializeField]
    public List<string> attackWar = new List<string>();
    [SerializeField]
    public List<string> attackFamily = new List<string>();
    [SerializeField]
    public List<string> attackReligion = new List<string>();
    [SerializeField]
    public List<string> attackMoney = new List<string>();

    [SerializeField]
    public Category category;
    [SerializeField]
    public SubCategory subCategory;
}
