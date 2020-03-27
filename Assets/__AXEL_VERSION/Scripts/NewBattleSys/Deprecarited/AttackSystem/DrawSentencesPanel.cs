using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DrawSentencesPanel : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform parentOfButtons;

    [Space(20)]

    public AttackObj dialogsWar;
    public AttackObj dialogsReligion;
    public AttackObj dialogsMoney;
    public AttackObj dialogsFamily;

    private List<GameObject> objButtons = new List<GameObject>();

    private void Start()
    {
        Debug.Log(dialogsWar.currentDialogsNumber + " << Test 1");

        for (int i = 0; i < dialogsWar.currentDialogsNumber; i++)
        {
            GameObject newBtt = Instantiate(buttonPrefab);
            newBtt.transform.SetParent(parentOfButtons, false);
            newBtt.transform.localScale = Vector3.one;
            newBtt.GetComponentInChildren<TextMeshProUGUI>().text = dialogsWar.buttonSentences[i];
        }

        objButtons.AddRange(GameObject.FindGameObjectsWithTag("Fight/Fight Button"));
    }

    public void OnClickCategoryWar()
    {
        ChangeCategory(dialogsWar);
    }
    public void OnClickCategoryMoney()
    {
        ChangeCategory(dialogsMoney);
    }
    public void OnClickCategoryFamily()
    {
        ChangeCategory(dialogsFamily);
    }
    public void OnClickCategoryReligion()
    {
        ChangeCategory(dialogsReligion);
    }

    private void ChangeCategory(AttackObj attack)
    {
        Debug.Log(attack.currentDialogsNumber + " << Test 2");

        objButtons.AddRange(GameObject.FindGameObjectsWithTag("Fight/Fight Button"));

        for(int oldButton = 0; oldButton < attack.currentDialogsNumber; oldButton++)
        {
            Destroy(objButtons[oldButton].gameObject);
        }

        objButtons.Clear();

        for (int i = 0; i < attack.currentDialogsNumber; i++)
        {
            GameObject newBtt = Instantiate(buttonPrefab);
            newBtt.transform.SetParent(parentOfButtons, false);
            newBtt.transform.localScale = Vector3.one;
            newBtt.GetComponentInChildren<TextMeshProUGUI>().text = attack.buttonSentences[i];
        }

    }
}
