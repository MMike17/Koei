using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KatanaSlider : MonoBehaviour
{
    public Image imgAttribute;
    public Slider sld;
    public Animator anim;
    public Animator worldAnim;

    private bool hasBeenValued;
    private bool isWatchingAbove;

    public void OnMaxValue()
    {
        if (sld.value >= 1)
        {
            hasBeenValued = true;
            anim.SetBool("Appears", false);
        }
    }

    private void Update()
    {
        if (hasBeenValued)
        {
            sld.value = 1;

            if (imgAttribute.color.a < 0.15f)
            {
                gameObject.SetActive(false);
            }

            if (!isWatchingAbove)
            {
                worldAnim.SetBool("WatchAbove", true);
                isWatchingAbove = true;
            }
        }
    }
}
