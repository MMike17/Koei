using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandStructurer : MonoBehaviour
{
    public Rect[] cards;
    public float offsetBetweenCards = 1.5f;

    private void Update()
    {
        if(cards == null)
        {
            Debug.Log("Must draw cards");
        }
        
    }
}
