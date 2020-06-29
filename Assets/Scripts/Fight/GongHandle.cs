using UnityEngine;
using UnityEngine.EventSystems;

public class GongHandle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	[Header("Assign in Inspector")]
	public Animator animator;

	public RectTransform rectTransform => GetComponent<RectTransform>();

	public bool selected { get; private set; }

	public void OnPointerDown(PointerEventData eventData)
	{
		animator.Play("Idle");

		if(gameObject.activeInHierarchy)
			selected = true;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		animator.Play("Fade");

		if(gameObject.activeInHierarchy)
			selected = false;
	}
}