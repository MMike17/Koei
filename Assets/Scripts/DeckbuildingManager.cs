using UnityEngine;

// class managing gameplay of Deckbuilding panel
public class DeckbuildingManager : MonoBehaviour, IDebugable
{
    string IDebugable.debugLabel => "<b>[DeckbuildingManager] : </b>";

    IDebugable debugableInterface => (IDebugable) this;

    public void Init()
    {
        Debug.Log(debugableInterface.debugLabel + "Initializing done");
    }
}