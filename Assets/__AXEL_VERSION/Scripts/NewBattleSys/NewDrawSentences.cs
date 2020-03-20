using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

public class NewDrawSentences : MonoBehaviour
{
    [Header("Choice System")]
    public GameObject buttonPrefab;
    public Transform parent;

    public AttackObj war;
    public AttackObj family;
    public AttackObj money;
    public AttackObj religion;

    public FatalityObj fatalitySentences;

    public GameObject buttonWar;
    public GameObject buttonFamily;
    public GameObject buttonMoney;
    public GameObject buttonReligion;

    private bool hasDisplayedFatality;

    private List<GameObject> bttAttacks = new List<GameObject>();

    /*
    private void Start()
    {
        DrawSentencesWar();
    }
    */

    private void Update()
    {
        if(FightSettings.currentHpEnemy <= 0)
        {
            if (!hasDisplayedFatality)
            {
                buttonFamily.SetActive(false);
                buttonMoney.SetActive(false);
                buttonReligion.SetActive(false);
                buttonWar.SetActive(false);

                Debug.Log("Display Fatality Buttons");

                bttAttacks.AddRange(GameObject.FindGameObjectsWithTag("Fight/Fight Button"));

                if (parent.childCount != 0)
                {
                    for (int btt = 0; btt < parent.childCount; btt++)
                    {
                        Destroy(bttAttacks[btt]);
                        Debug.Log(btt);
                    }
                }

                for (int i = 0; i < fatalitySentences.fatalitiesSentences.Length; i++)
                {
                    GameObject button = Instantiate(buttonPrefab);
                    button.transform.SetParent(parent, false);
                    button.transform.localScale = Vector3.one;
                    button.GetComponentInChildren<TextMeshProUGUI>().text = fatalitySentences.fatalitiesSentences[i];
                }
                hasDisplayedFatality = true;
            }
        }
    }

    public void DrawSentencesWar()
    {
        DrawSentences(war, EditorPrefs.GetInt("totalDialogs_" + war.cat.ToString()));
    }
    public void DrawSentencesFamily()
    {
        DrawSentences(family, EditorPrefs.GetInt("totalDialogs_" + family.cat.ToString()));
    }
    public void DrawSentencesMoney()
    {
        DrawSentences(money, EditorPrefs.GetInt("totalDialogs_" + money.cat.ToString()));
    }
    public void DrawSentencesReligion()
    {
        DrawSentences(religion, EditorPrefs.GetInt("totalDialogs_" + religion.cat.ToString()));
    }

    private void DrawSentences(AttackObj attack, int buttonsToDisplay)
    {
        if(FightSettings.currentHpEnemy > 0)
        {
            bttAttacks.AddRange(GameObject.FindGameObjectsWithTag("Fight/Fight Button"));

            if (parent.childCount != 0)
            {
                Debug.Log("Buttons to remove (number only): " + parent.childCount + " | Buttons to display: " + buttonsToDisplay);

                for (int btt = 0; btt < parent.childCount; btt++)
                {
                    Destroy(bttAttacks[btt]);
                    Debug.Log(btt);
                }
            }

            for (int i = 0; i < buttonsToDisplay; i++)
            {
                GameObject button = Instantiate(buttonPrefab);
                button.transform.SetParent(parent, false);
                button.transform.localScale = Vector3.one;
                button.GetComponentInChildren<TextMeshProUGUI>().text = attack.buttonSentences[i];
            }
            bttAttacks.Clear();
        }
    }
}
