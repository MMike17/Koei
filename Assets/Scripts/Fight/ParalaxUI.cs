using System;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxUI : MonoBehaviour
{
	[Header("Settings")]
	public MinMax width;
	public MinMax height;
	[Space]
	public Transform movableObject;
	public Color topDebugColor, downDebugColor;

	[Header("Assign in Inspector")]
	public List<Layer> layers;

	void OnDrawGizmos()
	{
		Debug.DrawLine(new Vector2(width.min, height.max), new Vector2(width.max, height.max), topDebugColor);
		Debug.DrawLine(new Vector2(width.min, height.min), new Vector2(width.max, height.min), downDebugColor);

		Debug.DrawLine(new Vector2(width.min, height.min), new Vector2(width.min, height.max), GetSidesColor());
		Debug.DrawLine(new Vector2(width.max, height.min), new Vector2(width.max, height.max), GetSidesColor());
	}

	void Update()
	{
		float widthRatio = (movableObject.localPosition.x - width.min) / width.span;
		float heightRatio = (movableObject.localPosition.y - height.min) / height.span;

		RectTransform referenceBounds = layers.Find(item => { return item.order == 0; }).rectTransform;

		foreach (Layer layer in layers)
		{
			if(layer.order != 0)
			{
				layer.rectTransform.localPosition = new Vector2(
					layer.GetWidthBounds(referenceBounds).NewPosition(widthRatio),
					layer.GetHeightBounds(referenceBounds).NewPosition(heightRatio)
				);
			}
		}
	}

	Color GetSidesColor()
	{
		float topH, topS, topV;
		Color.RGBToHSV(topDebugColor, out topH, out topS, out topV);

		float downH, downS, downV;
		Color.RGBToHSV(downDebugColor, out downH, out downS, out downV);

		return Color.HSVToRGB(Mathf.Lerp(topH, downH, 0.5f), Mathf.Lerp(topS, downS, 0.5f), Mathf.Lerp(topV, downV, 0.5f));
	}

	[Serializable]
	public class Layer
	{
		[Range(0, 10)]
		public int order;
		public RectTransform rectTransform;

		public MinMax GetWidthBounds(RectTransform reference)
		{
			return new MinMax(
				reference.localPosition.x - reference.rect.width / 2 + rectTransform.rect.width / 2,
				reference.localPosition.x + reference.rect.width / 2 - rectTransform.rect.width / 2
			);
		}

		public MinMax GetHeightBounds(RectTransform reference)
		{
			return new MinMax(
				reference.localPosition.y - reference.rect.height / 2 + rectTransform.rect.height / 2,
				reference.localPosition.y + reference.rect.height / 2 - rectTransform.rect.height / 2
			);
		}
	}

	[Serializable]
	public class MinMax
	{
		public float min;
		public float max;

		public float span => max - min;

		public MinMax(float min, float max)
		{
			this.min = min;
			this.max = max;
		}

		public float NewPosition(float ratio)
		{
			return max - span * ratio;
		}
	}
}