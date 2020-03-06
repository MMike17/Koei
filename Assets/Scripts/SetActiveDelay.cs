using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveDelay : MonoBehaviour
{
    public bool state;
    public GameObject objectToActiveOrNot;
    public float delay;

    private void Start()
    {
        Invoke("DelayMade", delay);
    }

    private void DelayMade()
    {
        objectToActiveOrNot.SetActive(state);
    }
}
