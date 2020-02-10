using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CardSystem;

public class CardTester : MonoBehaviour
{
    public string cardsPath;
    public List<Card> totalCards;

    // Set the hand
    public Transform handObject;
    List<Transform> handChildren;

    // Start is called before the first frame update
    void Start()
    {
        CardSys.GenerateDeckFullRandom(5, totalCards);

        handChildren = new List<Transform>();
        foreach(Transform child in handObject)
        {
            handChildren.Add(child);
        }

        // Set the text
        for(int i = 0; i < handChildren.Count; i++)
        {
            Text title = GetComponentInChildren<Text>();
            // handChildren[i]
        }
    }
}
