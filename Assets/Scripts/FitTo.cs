using UnityEngine;

public class FitTo : MonoBehaviour
{
    public enum Fit { WIDTH, HEIGHT, PARENT }

    public Fit fit_mode;
    public float padding;

    RectTransform rect_transform;

    void Awake()
    {
        rect_transform = GetComponent<RectTransform>();
    }

    void OnDrawGizmos()
    {
        if(!enabled)
            return;

        if(rect_transform == null)
            Awake();
        else
            Update();
    }

    void Update()
    {
        switch(fit_mode)
        {
            case Fit.HEIGHT:
                if(rect_transform.rect.width != rect_transform.rect.height - padding)
                    rect_transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect_transform.rect.height - padding);
                break;

            case Fit.WIDTH:
                if(rect_transform.rect.height != rect_transform.rect.width - padding)
                    rect_transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect_transform.rect.width - padding);
                break;

            case Fit.PARENT:
                if(rect_transform.rect.width != rect_transform.rect.height - padding)
                    rect_transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, GetLowestSize() - padding);

                if(rect_transform.rect.height != rect_transform.rect.width - padding)
                    rect_transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, GetLowestSize() - padding);
                break;
        }
    }

    float GetLowestSize()
    {
        RectTransform parent = rect_transform.parent.GetComponent<RectTransform>();

        return Mathf.Min(parent.rect.width, parent.rect.height);
    }
}