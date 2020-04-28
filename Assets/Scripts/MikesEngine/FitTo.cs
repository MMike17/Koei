using UnityEngine;
using UnityEngine.UI;

public class FitTo : MonoBehaviour
{
	public enum Fit { WIDTH, HEIGHT, PARENT }

	public Fit fit_mode;
	public float padding;
	public bool keepRatio;

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
		float targetSize = 0;
		float ratio = 0;

		switch(fit_mode)
		{
			case Fit.HEIGHT:
				targetSize = rect_transform.GetComponent<Image>().sprite.textureRect.height - padding;
				ratio = rect_transform.GetComponent<Image>().sprite.textureRect.height / rect_transform.GetComponent<Image>().sprite.textureRect.width;

				if(rect_transform.rect.width != targetSize)
				{
					rect_transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetSize);

					if(keepRatio)
						rect_transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetSize * ratio);
				}
				break;

			case Fit.WIDTH:
				targetSize = rect_transform.GetComponent<Image>().sprite.textureRect.width - padding;
				ratio = rect_transform.GetComponent<Image>().sprite.textureRect.width / rect_transform.GetComponent<Image>().sprite.textureRect.height;

				if(rect_transform.rect.height != targetSize)
				{
					rect_transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetSize);

					if(keepRatio)
						rect_transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetSize * ratio);
				}
				break;

			case Fit.PARENT:
				targetSize = GetLowestSize() - padding;

				if(rect_transform.rect.width != rect_transform.rect.height - padding)
					rect_transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetSize);

				if(rect_transform.rect.height != rect_transform.rect.width - padding)
					rect_transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetSize);
				break;
		}
	}

	float GetLowestSize()
	{
		RectTransform parent = rect_transform.parent.GetComponent<RectTransform>();

		return Mathf.Min(rect_transform.GetComponent<Image>().sprite.textureRect.width, rect_transform.GetComponent<Image>().sprite.textureRect.height);
	}
}