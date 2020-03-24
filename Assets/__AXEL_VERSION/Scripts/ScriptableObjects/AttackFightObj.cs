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
    public string[] attackArray =
    {
        "War", "Family", "Religion", "Money"
    };

    [SerializeField]
    public int choiceAttack;

    [SerializeField]
    public List<string> attack = new List<string>();

    [SerializeField]
    public Category category;
    [SerializeField]
    public SubCategory subCategory;
}
