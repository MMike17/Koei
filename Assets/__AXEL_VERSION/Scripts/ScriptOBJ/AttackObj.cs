using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player_Attack_Family", menuName = "Koei/Battle/Player Attacks Family")]
public class AttackObj : ScriptableObject
{
    [TextArea]
    public string[] buttonSentences;
}
