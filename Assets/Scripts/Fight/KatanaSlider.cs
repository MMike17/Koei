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

	public void OnPointerUp(PointerEventData eventData)
	{
		StartCoroutine(Move());
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