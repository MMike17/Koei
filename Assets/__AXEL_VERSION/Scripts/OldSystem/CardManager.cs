using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public CardTester cardTester;
    public Text playerSentenceTxt;
    public Text enemySentenceTxt;

    public float durationBetweenAttacks = 0.5f;
    public float sentenceDuration = 2.75f;

    private bool hasCouroutined;
    private bool hasBeenEnemy;


    public void PlayAndTurn(string sentence)
    {
        if (!hasCouroutined)
        {
            StartCoroutine(Turn(sentence));
            hasCouroutined = true;
        }
    }

    // - - - - - - - - - - - - - - - - - //
    
    IEnumerator Turn(string playerSentence)
    {
        playerSentenceTxt.text = playerSentence;

        yield return new WaitForSeconds(sentenceDuration);

        playerSentenceTxt.text = "";
        enemySentenceTxt.text = "";

        hasCouroutined = false;
    }
}
