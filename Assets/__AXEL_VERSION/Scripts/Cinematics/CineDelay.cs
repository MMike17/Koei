using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CineDelay : MonoBehaviour
{
    public Animator animator;
    public bool state;
    public string variableAnim;

    public float startDelay;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("PlayAnim", startDelay);
    }

    private void PlayAnim()
    {
        animator.SetBool(variableAnim, state);
    }
}
