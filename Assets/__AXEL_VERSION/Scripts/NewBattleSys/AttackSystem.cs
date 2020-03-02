using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttackSystem : MonoBehaviour
{
    public AttackObj[] stepsOfBattle;
    public GameObject buttonsToMakeAppear;

    public int currentStep;

    private GameObject mainParent;
    private List<Button> buttonsToClick = new List<Button>();

    private bool alreadyAddedStep;

    private void OnEnable()
    {
        InitializeButtons();
    }

    private void Update()
    {
        // Check on each Buttons
        for(int i = 0; i < buttonsToClick.Count; i++)
        {
            // Check if button has been pressed
            if (buttonsToClick[i].GetComponent<BasicalBoolean>().hasBeenActivated)
            {
                // Next phase
                currentStep++;
                Debug.Log("Next Step - FIGHT GAMEPLAY + CURRENTSTEP >> " + currentStep);
                InitializeButtons();
            }
        }
    }

    private void InitializeButtons()
    {
        Debug.Log("Destroying buttons...");
        buttonsToClick.Clear();
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Fight/Fight Button").Length; i++)
        {
            Destroy(GameObject.FindGameObjectsWithTag("Fight/Fight Button")[i]);
        }


        Debug.Log("Intialize Buttons");
        mainParent = GameObject.FindGameObjectWithTag("Fight/Main UI Parent");
        Debug.Log("Found the main parent.");


        for (int buttons = 0; buttons < stepsOfBattle[currentStep].buttonSentences.Length; buttons++)
        {
            // Create new buttons
            GameObject newButton = Instantiate(buttonsToMakeAppear);
            newButton.transform.parent = mainParent.transform;
            buttonsToClick.Add(newButton.GetComponent<Button>());
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = stepsOfBattle[currentStep].buttonSentences[buttons];
            Debug.Log("Buttons Initialized with success");
        }
    }
}
