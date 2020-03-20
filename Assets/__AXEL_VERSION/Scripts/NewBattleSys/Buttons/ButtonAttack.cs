using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonAttack : MonoBehaviour
{
    [Header("Animation System")]
    public string variableName = "WatchAbove";


    public float timeBeforeRemoveSentence;
    public float delayBands;

    [HideInInspector]
    public int order;

    private TextMeshProUGUI textPlayer;

    private NewDrawSentences sentenceDrawner;
    private GameObject blackPanel;
    private Animator worldAnim;
    private Animator blackBands;

    private void OnEnable()
    {
        textPlayer = GameObject.FindGameObjectWithTag("Sentences/Player").GetComponent<TextMeshProUGUI>();

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
                    worldAnim.SetBool(variableName, false);
                    blackBands.SetBool("IsDisplayed", true);

                    StartCoroutine(WriteSentence());
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
}
