using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CineStart : MonoBehaviour
{
    public Animator animator;
    public string variableAnim;

    // Start is called before the first frame update
    void Start()
    {
        animator.SetBool(variableAnim, true);
    }
}
