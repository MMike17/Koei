using UnityEngine;

public class DeckbuildingManager : MonoBehaviour, IDebugable
{
    public string debugLabel => "<b>[DeckbuildingManager] : </b>";

    IDebugable debugableInterface => (IDebugable) this;

    public void Init()
    {
        Debug.Log(debugLabel + "Initializing done");
    }
}