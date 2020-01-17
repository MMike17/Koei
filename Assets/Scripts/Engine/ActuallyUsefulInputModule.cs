using UnityEngine.EventSystems;

public class ActuallyUsefulInputModule : StandaloneInputModule
{
	public static ActuallyUsefulInputModule Get;

	public static PointerEventData GetPointerEventData(int pointerID = -1)
	{
		if(Get == null)
			return null;

		PointerEventData eventData;
		Get.GetPointerData(pointerID, out eventData, true);

		return eventData;
	}

	protected override void Awake()
	{
		base.Awake();
		Get = this;
	}
}