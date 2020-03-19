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

    private TextMeshProUGUI textPlayer;

    private GameObject blackPanel;
    private Animator worldAnim;
    private Animator blackBands;

    private void OnEnable()
    {
        textPlayer = GameObject.FindGameObjectWithTag("Sentences/Player").GetComponent<TextMeshProUGUI>();

        //blackPanel = GameObject.FindGameObjectWithTag("GUI/Black Panel");
        //Debug.Log(blackPanel.name);
        blackBands = GameObject.FindGameObjectWithTag("GUI/Black Bands").GetComponent<Animator>();
        worldAnim = GameObject.FindGameObjectWithTag("Fight/World").GetComponent<Animator>();
    }

    // Update is called once per frame
    public void OnClickButton()
    {
        //blackPanel.SetActive(true);

        worldAnim.SetBool(variableName, false);
        blackBands.SetBool("IsDisplayed", true);

        StartCoroutine(WriteSentence());

        //blackPanel.GetComponent<Animator>().SetBool("IsHalfBlack", true);
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
    }
}
