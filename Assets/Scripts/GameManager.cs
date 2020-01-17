using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour, IDebugable
{
    public static GameManager Get;

    [Header("Settings")]

    [Header("Title")]
    public Button titlePlay;
    public Button titleSettings;
    public Button titleQuit;

    [Header("Assign in Inspector")]
    public PanelManager panelManager;
    public PopupManager popupManager;
    public EventsManager eventsManager;
    public ShogunManager shogunManager;
    public GameData gameData;

    [Header("test")]
    public Dialogue testDialogue;

    IDebugable debuguableInterface => (IDebugable) this;

    public string debugLabel => "<b>[GameManager] : </b>";

    public enum GamePhase
    {
        TITLE,
        SHOGUN,
        DECKBUILDING,
        FIGHT
    }

    public enum GamePopup
    {
        EMPTY,
        SETTINGS,
        QUIT_SHOGUN
    }

    void Awake()
    {
        if(Get == this)
            return;

        if(Get != null)
            Destroy(Get.gameObject);

        Init();
    }

    void Init()
    {
        Get = this;

        titlePlay.onClick.AddListener(() => JumpTo(GamePhase.SHOGUN));
        // titleSettings.onClick.AddListener(() => popupManager.Pop(GamePopup.SETTINGS));
        // titleQuit.onClick.AddListener(() => Application.Quit());

        panelManager.Init();
        popupManager.Init();
        eventsManager.Init();
        shogunManager.Init(() => { popupManager.CancelPop(); JumpTo(GamePhase.DECKBUILDING); },
            () => { popupManager.CancelPop(); }, () => { Pop(GamePopup.QUIT_SHOGUN); });

        PlugEvents();
    }

    void PlugEvents()
    {
        eventsManager.AddPhaseAction(GamePhase.SHOGUN, () => { shogunManager.StartDialogue(testDialogue); });

        // add events to event manager here
    }

    void JumpTo(GameManager.GamePhase phase)
    {
        panelManager.JumpTo(phase, () => eventsManager.CallPhaseActions(phase));
    }

    void Pop(GameManager.GamePopup popup)
    {
        popupManager.Pop(popup, () => eventsManager.CallPopupActions(popup));
    }
}