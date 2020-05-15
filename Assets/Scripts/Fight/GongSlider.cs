using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GongSlider : MonoBehaviour
{
	[Header("Settings")]
	public float maxAngle;

	[Header("Assign in Inspector")]
	public RectTransform handle;
	public Slider slider;

	Action callback;
	float previousValue;

	public void Init(Action callback)
	{
		if(slider == null)
		{
			Debug.LogError("Can't find Slider");
			return;
		}

		this.callback = callback;

		gameObject.SetActive(false);
	}

	void OnDrawGizmos()
	{
		Update();
	}

	void Update()
	{
		if(Input.GetMouseButton(0) && slider.value != slider.maxValue)
			handle.position = Input.mousePosition;

		handle.rotation = Quaternion.Euler(0, 0, maxAngle * slider.value);

		if(slider.value >= slider.maxValue)
		{
			Vector2 newPos = handle.parent.position;
			newPos.x += handle.parent.GetComponent<RectTransform>().rect.width / 2 - handle.rect.width / 2;

			handle.position = newPos;

			if(previousValue < slider.value && callback != null)
				Invoke("InvokeCallback", 1);
		}

		previousValue = slider.value;
	}

	void InvokeCallback()
	{
		callback.Invoke();
	}
}