using UnityEngine;
using UnityEngine.EventSystems;

public class FightManager : MonoBehaviour
{
    public static FightManager Get;

    [Header("Assign in Inspector")]
    public GameObject winPanel;
    public GameObject losePanel;
    public DialoguePanelManager dialoguePanelManager;
    public EventSystem eventSystem;
    public Dialogue fights;

    public int playerValue { get; set; }
    public int fightIndex { get; set; }

    void Awake()
    {
        Get = this;

        playerValue = 0;
        fightIndex = 0;

        winPanel.SetActive(false);
        losePanel.SetActive(false);

        dialoguePanelManager.InitAll();
        dialoguePanelManager.enemyName.text = fights.otherName;

        ChangeFight();
    }

    void Update()
    {

    }

    public void NextFight()
    {
        fightIndex++;

        if(fightIndex >= fights.phases.Length)
        {
            if(playerValue >= fights.scoreLimit)
            {
                winPanel.SetActive(true);
            }
            else
            {
                losePanel.SetActive(true);
            }

            return;
        }

        ChangeFight();
    }

    void ChangeFight()
    {
        eventSystem.SetSelectedGameObject(null);
        dialoguePanelManager.AssignLines(fights.phases[fightIndex]);
    }
}