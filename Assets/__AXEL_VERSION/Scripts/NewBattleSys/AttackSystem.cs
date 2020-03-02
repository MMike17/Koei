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
    public string animVariable = "PlayFadeOut";

    // Animation system
    private List<Animator> anims = new List<Animator>();
    
    private GameObject mainParent;
    private List<Button> buttonsToClick = new List<Button>();

    private bool alreadyLaunchedAnimation;

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

                if (currentStep < stepsOfBattle.Length)
                {
                    InitializeButtons();
                }
                else
                {
                    if (!alreadyLaunchedAnimation)
                    {
                        mainParent.GetComponentInChildren<Animator>().SetBool(animVariable, true);
                        Debug.Log(mainParent.GetComponentInChildren<Animator>().name);
                        mainParent.GetComponent<Animator>().SetBool(animVariable, true);
                        alreadyLaunchedAnimation = true;
                    }
                }
            }
        }

        // Set active false the panel if panel is invisible
        /*
        if (mainParent.GetComponent<Image>().color == new Color(mainParent.GetComponent<Image>().color.r, mainParent.GetComponent<Image>().color.g, mainParent.GetComponent<Image>().color.b, 0))
        {
            mainParent.SetActive(false);
        }
        */
    }

    private void InitializeButtons()
    {
        anims.Clear();
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
            newButton.transform.SetParent(mainParent.transform);
            Debug.Log(newButton.name + "is initializing now.", newButton);
            buttonsToClick.Add(newButton.GetComponent<Button>());
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = stepsOfBattle[currentStep].buttonSentences[buttons];
            
            anims.Add(newButton.GetComponent<Animator>());
            anims.Add(newButton.GetComponentInChildren<Animator>());

            Debug.Log("Buttons Initialized with success");
        }
    }

    public void OnClick()
    {
        for (int i = 0; i < anims.Count; i++)
        {
            anims[i].SetBool("Play", true);
        }
    }
}
