using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttackSystem : MonoBehaviour
{
    [Header("Objects Selection")]
    public AttackObj[] stepsOfBattle;
    public GameObject buttonsToMakeAppear;
    public TextMeshProUGUI timerText;

    [Header("Timer")]
    [Min(0)]
    public float openingCinematicDuration;
    [Space(15)]
    [Tooltip("0 to disable timer | Superior to 0 to enable timer")]
    [Min(0)]
    public float timerPerRounds;
    [Space(15)]
    [Min(0)]
    public int currentStep;

    [Header("Animation")]
    public string animVariable = "PlayFadeOut";

    // Animation system
    private List<Animator> anims = new List<Animator>();
    
    private GameObject mainParent;
    private List<Button> buttonsToClick = new List<Button>();


    private float remainingTimer;
    private bool isTimerDisabled;

    private bool alreadyLaunchedAnimation;

    private bool hasEndCinematic;

    private bool startCoroutine = true;
    private bool timerCoroutine;

    private void OnEnable()
    {
        if(timerPerRounds == 0)
        {
            isTimerDisabled = true;
            timerText.text = "";
        }
        else
        {
            remainingTimer = timerPerRounds;
            timerText.text = remainingTimer.ToString();
        }


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
        if (!isTimerDisabled)
        {
            if (startCoroutine)
            {
                if (!timerCoroutine)
                {
                    StartCoroutine(UseTimer());
                    timerCoroutine = true;
                }
            }
        }
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

        Debug.Log("Reload timer");
        remainingTimer = timerPerRounds;

        Debug.Log("Intialize Buttons");
        mainParent = GameObject.FindGameObjectWithTag("Fight/Main UI Parent");
        Debug.Log("Found the main parent.");


        for (int buttons = 0; buttons < stepsOfBattle[currentStep].buttonSentences.Length; buttons++)
        {
            // Create new buttons
            GameObject newButton = Instantiate(buttonsToMakeAppear);
            newButton.transform.SetParent(mainParent.transform);

            // Set the scale
            newButton.transform.localScale = Vector3.one;

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
        startCoroutine = false;
        for (int i = 0; i < anims.Count; i++)
        {
            anims[i].SetBool("Play", true);
        }
    }

    private IEnumerator UseTimer()
    {
        if (!hasEndCinematic)
        {
            yield return new WaitForSeconds(openingCinematicDuration);
            hasEndCinematic = true;
        }

        if (hasEndCinematic)
        {
            yield return new WaitForSeconds(1);

            remainingTimer--;
            timerText.text = remainingTimer.ToString();

            timerCoroutine = false;
        }
    }
}
