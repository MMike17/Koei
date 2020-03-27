using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New_Fatality", menuName = "Koei/Battle/Fatality")]
public class FatalityObj : ScriptableObject
{
    public int goodFatality;
    public string[] fatalitiesSentences;
    public string badAnswerResponseFromEnemy;
}
