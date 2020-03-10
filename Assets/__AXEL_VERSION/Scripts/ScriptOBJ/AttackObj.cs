using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player_Attack_Family", menuName = "Koei/Battle/Player Attacks Family")]
public class AttackObj : ScriptableObject
{
    public enum Category { War, Religion, Family, Money }
    public Category category;

    [TextArea]
    public string[] buttonSentences;
}
