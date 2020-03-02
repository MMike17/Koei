using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player_Attack", menuName = "Koei/Battle/Player Attacks")]
public class AttackObj : ScriptableObject
{
    public float delayBeforeEnd = 5f;
    [TextArea]
    public string[] buttonSentences;
}
