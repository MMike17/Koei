using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonAttack : MonoBehaviour
{
    [Header("Animation System")]
    public string variableName = "WatchAbove";


    public float timeBeforeRemoveSentence;
    public float timeBeforePlayerLoose;
    public float delayBands;

    [HideInInspector]
    public int order;

    private TextMeshProUGUI textPlayer;
    private TextMeshProUGUI textEnemy;

    private NewDrawSentences sentenceDrawner;
    private GameObject blackPanel;
    private Animator worldAnim;
    private Animator blackBands;

    private void OnEnable()
    {
        textPlayer = GameObject.FindGameObjectWithTag("Sentences/Player").GetComponent<TextMeshProUGUI>();
        textEnemy = GameObject.FindGameObjectWithTag("Sentences/Enemy").GetComponent<TextMeshProUGUI>();

        sentenceDrawner = FindObjectOfType<NewDrawSentences>();

        //blackPanel = GameObject.FindGameObjectWithTag("GUI/Black Panel");
        //Debug.Log(blackPanel.name);
        blackBands = GameObject.FindGameObjectWithTag("GUI/Black Bands").GetComponent<Animator>();
        worldAnim = GameObject.FindGameObjectWithTag("Fight/World").GetComponent<Animator>();
    }

    // Update is called once per frame
    public void OnClickButton()
    {
        if (FightSettings.currentHpEnemy > 0)
        {
            //blackPanel.SetActive(true);

            worldAnim.SetBool(variableName, false);
            blackBands.SetBool("IsDisplayed", true);

            StartCoroutine(WriteSentence());

            //blackPanel.GetComponent<Animator>().SetBool("IsHalfBlack", true);
        }
        else
        {
            // FATALITY PART
            Debug.Log("Just displayed Fatality Sentences");

            for(int i = 0; i < sentenceDrawner.fatalitySentences.fatalitiesSentences.Length; i++)
            {
                if (order == sentenceDrawner.fatalitySentences.goodFatality)
                {
                    // RIGHT FATALITY
                    worldAnim.SetBool(variableName, false);
                    blackBands.SetBool("IsDisplayed", true);

                    StartCoroutine(WriteSentence());
                }
                else
                {
                    // WRONG FATALITY

                    worldAnim.SetBool(variableName, false);
                    blackBands.SetBool("IsDisplayed", true);

                    StartCoroutine(WriteSentenceWrongFatality());

                }
            }

        }
    }

    private IEnumerator WriteSentence()
    {

        yield return new WaitForSeconds(1f);

        textPlayer.text = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;

        yield return new WaitForSeconds(timeBeforeRemoveSentence);

        textPlayer.text = "";

        blackBands.SetBool("IsDisplayed", false);

        yield return new WaitForSeconds(delayBands);

        worldAnim.SetBool(variableName, true);


        FightSettings.currentHpEnemy--;
    }
    private IEnumerator WriteSentenceWrongFatality()
    {

        yield return new WaitForSeconds(1f);

        textPlayer.text = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;

        yield return new WaitForSeconds(timeBeforeRemoveSentence);

        textPlayer.text = "";


        yield return new WaitForSeconds(timeBeforePlayerLoose);

        textEnemy.text = sentenceDrawner.fatalitySentences.badAnswerResponseFromEnemy;

        yield return new WaitForSeconds(timeBeforeRemoveSentence);

        textEnemy.text = "";
        textPlayer.text = "Je suis déshonoré !!";

        yield return new WaitForSeconds(timeBeforeRemoveSentence);

        textPlayer.text = "";

        yield return new WaitForSeconds(delayBands);

        blackBands.SetBool("IsDisplayed", false);

        //worldAnim.SetBool(variableName, true);
        

        Debug.Log("<b>GAME OVER</b>");
    }
}
