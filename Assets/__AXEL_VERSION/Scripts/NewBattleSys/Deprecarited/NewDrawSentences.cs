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

    public NewAttackObj attacks;

    public FatalityObj fatalitySentences;

    public GameObject buttonWar;
    public GameObject buttonFamily;
    public GameObject buttonMoney;
    public GameObject buttonReligion;

    private bool hasDisplayedFatality;
    private Category currentCat;
    private List<GameObject> bttAttacks = new List<GameObject>();

    /*
    private void Start()
    {
        DrawSentencesWar();
    }
    */
    /*
    private void Update()
    {
        if(FightSettings.currentHpEnemy == 0)
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
                    button.GetComponent<ButtonAttack>().order = i;
                }
                hasDisplayedFatality = true;
            }
        }
    }

    #region Step 3
    // Phasis 3
    public void DrawSentencesWar()
    {
        currentCat = Category.WAR;
        DrawSentences(currentCat, EditorPrefs.GetInt("totalDialogs_" + currentCat.ToString()));
    }
    public void DrawSentencesFamily()
    {
        currentCat = Category.FAMILY;
        DrawSentences(currentCat, EditorPrefs.GetInt("totalDialogs_" + currentCat.ToString()));
    }
    public void DrawSentencesMoney()
    {
        currentCat = Category.MONEY;
        DrawSentences(currentCat, EditorPrefs.GetInt("totalDialogs_" + currentCat.ToString()));
    }
    public void DrawSentencesReligion()
    {
        currentCat = Category.RELIGION;
        DrawSentences(currentCat, EditorPrefs.GetInt("totalDialogs_" + currentCat.ToString()));
    }

    private void DrawSentences(NewAttackObj attack, int buttonsToDisplay)
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
                button.GetComponentInChildren<TextMeshProUGUI>().text = attack.sentencesByCategories[i].subCategoryBySentences[i].sentence;
            }
            bttAttacks.Clear();
        }
    }
    #endregion
    */
}
