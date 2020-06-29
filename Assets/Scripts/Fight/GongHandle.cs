using UnityEngine;
using UnityEngine.EventSystems;

public class GongHandle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	[Header("Assign in Inspector")]
	public Animator animator;

	public RectTransform rectTransform => GetComponent<RectTransform>();

	public bool selected { get; private set; }

	bool blockEffect;

	public void Init()
	{
		blockEffect = false;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if(!blockEffect)
			animator.Play("Idle");

		if(gameObject.activeInHierarchy)
			selected = true;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if(!blockEffect)
			animator.Play("Fade");

		if(gameObject.activeInHierarchy)
			selected = false;
	}

	public void BlockEffect()
	{
		blockEffect = true;
	}
}