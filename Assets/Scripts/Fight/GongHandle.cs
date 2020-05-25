using UnityEngine;
using UnityEngine.EventSystems;

public class GongHandle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public RectTransform rectTransform => GetComponent<RectTransform>();

	public bool selected { get; private set; }

	public void OnPointerDown(PointerEventData eventData)
	{
		if(gameObject.activeInHierarchy)
			selected = true;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if(gameObject.activeInHierarchy)
			selected = false;
	}
}