using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

public class NewDrawSentences : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform parent;

    public AttackObj war;
    public AttackObj family;
    public AttackObj money;
    public AttackObj religion;

    private List<GameObject> bttAttacks = new List<GameObject>();

    private void Start()
    {
        DrawSentencesWar();
    }

    public void DrawSentencesWar()
    {
        CheckAndDestroyOldButtons();
        DrawSentences(war, EditorPrefs.GetInt("totalDialogs_" + war.cat.ToString()));
    }
    public void DrawSentencesFamily()
    {
        CheckAndDestroyOldButtons();
        DrawSentences(family, EditorPrefs.GetInt("totalDialogs_" + family.cat.ToString()));
    }
    public void DrawSentencesMoney()
    {
        CheckAndDestroyOldButtons();
        DrawSentences(money, EditorPrefs.GetInt("totalDialogs_" + money.cat.ToString()));
    }
    public void DrawSentencesReligion()
    {
        CheckAndDestroyOldButtons();
        DrawSentences(religion, EditorPrefs.GetInt("totalDialogs_" + religion.cat.ToString()));
    }

    private void DrawSentences(AttackObj attack, int buttonsToDisplay)
    {
        for (int i = 0; i < buttonsToDisplay; i++)
        {
            GameObject button = Instantiate(buttonPrefab);
            button.transform.SetParent(parent, false);
            button.transform.localScale = Vector3.one;
            button.GetComponentInChildren<TextMeshProUGUI>().text = attack.buttonSentences[i];
        }
    }

    private void CheckAndDestroyOldButtons()
    {
        if(GameObject.FindGameObjectsWithTag("Fight/Fight Button").Length > 0)
        {
            bttAttacks.AddRange(GameObject.FindGameObjectsWithTag("Fight/Fight Button"));

            for (int btt = 0; btt <= bttAttacks.Count; btt++)
            {
                Destroy(bttAttacks[btt]);
            }
            bttAttacks.Clear();
        }

    }
}
