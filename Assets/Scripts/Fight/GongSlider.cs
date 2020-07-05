using System;
using UnityEditor;
using UnityEngine;

public class GongSlider : MonoBehaviour
{
	[Header("Settings")]
	public float minAngle;
	public float maxAngle, maxDistance;
	public Color leftColor, rightColor, anchorColor;
	[Range(0, 1)]
	public float value;

	[Header("Assign in Inspector")]
	public GongHandle handle;
	public RectTransform slidingRect, graphic;
	public Animator anim;

	Vector2 leftAnchor => new Vector2(slidingRect.position.x - slidingRect.rect.width / 2, slidingRect.position.y);
	Vector2 rightAnchor => new Vector2(slidingRect.position.x + slidingRect.rect.width / 2, slidingRect.position.y);
	Vector2 topAnchor => new Vector2(slidingRect.position.x, slidingRect.position.y - slidingRect.rect.height / 2);
	Vector2 bottomAnchor => new Vector2(slidingRect.position.x, slidingRect.position.y + slidingRect.rect.height / 2);

	float amplitude => rightAnchor.x - leftAnchor.x;

	Action callback;
	float previousValue;

	void OnDrawGizmos()
	{
#if UNITY_EDITOR
		bool selection = Selection.Contains(gameObject) ? true : Selection.Contains(slidingRect.gameObject) ? true : Selection.Contains(graphic.gameObject) ? true : false;

		if(!selection)
			return;

		if(slidingRect != null)
		{
			Gizmos.color = leftColor;
			Gizmos.DrawCube(new Vector2(slidingRect.position.x - slidingRect.rect.width / 4, slidingRect.position.y), new Vector2(slidingRect.rect.width / 2, slidingRect.rect.height));

			Gizmos.color = rightColor;
			Gizmos.DrawCube(new Vector2(slidingRect.position.x + slidingRect.rect.width / 4, slidingRect.position.y), new Vector2(slidingRect.rect.width / 2, slidingRect.rect.height));

			Gizmos.color = anchorColor + leftColor;
			Gizmos.DrawCube(leftAnchor, Vector2.one * slidingRect.rect.height / 3);
			Gizmos.color = anchorColor + rightColor;
			Gizmos.DrawCube(rightAnchor, Vector2.one * slidingRect.rect.height / 3);
			Gizmos.color = anchorColor;
			Gizmos.DrawCube(topAnchor, Vector2.one * slidingRect.rect.height / 3);
			Gizmos.DrawCube(bottomAnchor, Vector2.one * slidingRect.rect.height / 3);
		}

		if(graphic != null)
			graphic.position = leftAnchor;

		if(handle != null && slidingRect != null && graphic != null)
			Update();
#endif
	}

	public void Init(Action callback)
	{
		if(slidingRect == null)
		{
			Debug.LogError("Can't find Slider");
			return;
		}

		value = 0;

		this.callback = callback;

		handle.Init();

		gameObject.SetActive(false);
	}

	public void SetInitialValue()
	{
		graphic.position = leftAnchor;
		Update();
	}

	void Update()
	{
		if(handle.selected)
		{
			graphic.position = Input.mousePosition;

			if(Mathf.Abs(graphic.position.y - graphic.transform.parent.position.y) <= maxDistance)
				value = (graphic.position.x - leftAnchor.x) / amplitude;
			else
				value = 0;

			if(value == 1)
				graphic.position = rightAnchor;
		}
		else
			graphic.position = new Vector2(value * amplitude + leftAnchor.x, slidingRect.position.y);

		float actualAngle = value * (maxAngle - minAngle) + minAngle;
		graphic.rotation = Quaternion.Euler(0, 0, actualAngle);

		if(value <= 0)
			graphic.rotation = Quaternion.Euler(0, 0, minAngle);

		if(value >= 1)
		{
			graphic.rotation = Quaternion.Euler(0, 0, maxAngle);
			graphic.position = rightAnchor;

			handle.BlockEffect();

			if(previousValue < value && callback != null)
			{
				anim.Play("Sound");
				Invoke("InvokeCallback", 1);
			}
		}

		previousValue = value;
	}

	void InvokeCallback()
	{
		callback.Invoke();
	}
}