using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutOnClick : MonoBehaviour
{
    private AttackSystem attack;

    private void Start()
    {
        attack = FindObjectOfType<AttackSystem>();
    }

    public void OnClickButton()
    {
        Debug.Log("Prepare to fade buttons", gameObject);
        attack.OnClick();
    }

}
