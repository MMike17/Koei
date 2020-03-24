using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicalBoolean : MonoBehaviour
{
    [HideInInspector]
    public bool hasBeenActivated;

    public void HasBeenActivated()
    {
        hasBeenActivated = true;
    }
}
