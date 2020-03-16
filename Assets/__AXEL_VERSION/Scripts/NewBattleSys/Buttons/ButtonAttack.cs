using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAttack : MonoBehaviour
{
    [Header("Animation System")]
    public string variableName = "WatchAbove";

    private GameObject world;
    private Animator worldAnim;

    private void Start()
    {
        world = GameObject.FindGameObjectWithTag("Fight/World");
        worldAnim = world.GetComponent<Animator>();
    }

    // Update is called once per frame
    public void OnClickButton()
    {
        worldAnim.SetBool(variableName, false);
    }
}
