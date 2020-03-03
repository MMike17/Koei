using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSentenceSystem : MonoBehaviour
{
    [Header("Basical Settings")]
    public AttackObj[] attackCategories;
    public GameObject buttonPrefab;

    [Space(20)]

    [Tooltip("0 to disable timer ; superior to zero to activate it")]
    [Min(0)]
    public float timer;                                 // Not integrated for now


    private GameObject mainParent;
    private List<GameObject> sentencesButtons = new List<GameObject>();

    private void Start()
    {
        mainParent = GameObject.FindGameObjectWithTag("Fight/Main UI Parent");
        sentencesButtons.AddRange(GameObject.FindGameObjectsWithTag("Fight/Fight Button"));
    }

    private void Init()
    {
        sentencesButtons.Clear();

        // Generate buttons
        GameObject newBtt = GameObject.Instantiate(buttonPrefab);
        newBtt.transform.localScale = Vector3.one;
        newBtt.transform.SetParent(mainParent.transform);
        sentencesButtons.Add(newBtt);
    }

}
