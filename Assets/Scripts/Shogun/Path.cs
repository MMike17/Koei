using UnityEngine;

public class Path : MonoBehaviour, IInitializable, IDebugable
{
	[Header("Settings")]
	public float pathThickness;

	[Header("Debug")]
	public Transform start;
	public Transform end;

	RectTransform rectTransform { get { return GetComponent<RectTransform>(); } }

	IDebugable debuguableInterface => (IDebugable) this;
	IInitializable initializableInterface => (IInitializable) this;

	public bool initialized => initializableInterface.initializedInternal;

	bool IInitializable.initializedInternal { get; set; }
	string IDebugable.debugLabel => "<b>[Path] : </b>";

	void OnDrawGizmos()
	{
		if(start != null && end != null)
			SetPath();
	}

	public void Init(Transform start, Transform end)
	{
		this.start = start;
		this.end = end;

		initializableInterface.InitInternal();
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;
		Debug.Log(debuguableInterface.debugLabel + "Initializing done");
	}

	void SetPath()
	{
		Vector3 offset = (end.localPosition - start.localPosition);

		// moves path center
		transform.localPosition = start.localPosition + offset / 2;

		// rotates path
		float angle = Vector3.Angle(start.right, offset);
		transform.rotation = Quaternion.Euler(0, 0, angle);

		// set height
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, pathThickness);

		// set width
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Vector3.Distance(start.localPosition, end.localPosition));

		// puts the path on top of hierarchy to render behind knobs
		transform.SetAsFirstSibling();

		Debug.Log(debuguableInterface.debugLabel + "Path set");
	}
}