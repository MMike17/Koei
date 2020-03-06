using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogPlayer : MonoBehaviour
{
    public DialogsObj dialog;
    public float delayStart;
    public float delayBetweenSentences;
    public Animator blackBands;
    public Animator katanaAnim;

    public float delayBetweenAnswers = 1.75f;

    [Space(25)]

    public TextMeshProUGUI textPlayer;
    public TextMeshProUGUI textEnemy;

    [Space(25)]

    public bool isPlayerStarts;


    private int dialogNumber;
    private bool hasCoroutined;

    private void Start()
    {
        // Set dialogs to zero
        textPlayer.text = "";
        textEnemy.text = "";

        InvokeRepeating("DisplayDialog", delayStart, delayBetweenSentences);
    }

    public void DisplayDialog()
    {
        if(dialogNumber < dialog.dialogsPlayer.Length || dialogNumber < dialog.dialogsEnemy.Length)
        {
            isPlayerStarts = !isPlayerStarts;

            if (!hasCoroutined)
            {
                StartCoroutine(WaitDelay());
                hasCoroutined = true;
            }
        }
        if(dialogNumber == dialog.dialogsPlayer.Length ||dialogNumber == dialog.dialogsEnemy.Length)
        {
            // Set dialogs to zero
            textPlayer.text = "";
            textEnemy.text = "";

            blackBands.SetBool("IsDisplayed", false);
            katanaAnim.SetBool("Appears", true);

            CancelInvoke("DisplayDialog");
        }
    }

    IEnumerator WaitDelay()
    {
        // Say the sentence
        if (isPlayerStarts)
        {
            textEnemy.text = dialog.dialogsEnemy[dialogNumber];
            textPlayer.text = "";
        }
        else
        {
            textPlayer.text = dialog.dialogsPlayer[dialogNumber];
            textEnemy.text = "";
        }

        yield return new WaitForSeconds(delayBetweenAnswers);

        // Say the reversed sentence
        if (isPlayerStarts)
        {
            textPlayer.text = dialog.dialogsPlayer[dialogNumber];
            textEnemy.text = "";
        }
        else
        {
            textPlayer.text = "";
            textEnemy.text = dialog.dialogsEnemy[dialogNumber];

        }

        isPlayerStarts = !isPlayerStarts;

        Debug.Log("<b>START DIALOG >> </b> Phasis is finished");

        dialogNumber++;
        hasCoroutined = false;
    }
}
