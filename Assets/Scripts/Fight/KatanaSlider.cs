using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class KatanaSlider : MonoBehaviour, IPointerUpHandler
{
	[Header("Settings")]
	public float slideDuration;

	public Slider slider => GetComponent<Slider>();
	public Animator animator => GetComponent<Animator>();

	public void OnPointerUp(PointerEventData eventData)
	{
		if(slider.value > 0.1f)
			StartCoroutine(Move());
		else
			animator.Play("Fade");
	}

	void Awake()
	{
		slider.onValueChanged.AddListener((float value) => animator.Play("Idle"));
	}

	IEnumerator Move()
	{
		while (slider.value != 1)
		{
			slider.interactable = false;
			slider.value = Mathf.MoveTowards(slider.value, 1, 1 / slideDuration * Time.deltaTime);

			yield return null;
		}

		slider.interactable = true;
		yield break;
	}
}