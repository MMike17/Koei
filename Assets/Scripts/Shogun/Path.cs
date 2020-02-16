using UnityEngine;
using UnityEngine.UI;

public class Path : MonoBehaviour, IInitializable, IDebugable
{
	[Header("Settings")]
	public float pathThickness;
	public Color normalColor, validatedColor, wrongColor;

	[Header("Debug")]
	public Transform start;
	public Transform end;
	public SubCategory startClue;
	public SubCategory endClue;

	public enum State
	{
		NORMAL,
		VALIDATED,
		WRONG
	}

	public bool initialized => initializableInterface.initializedInternal;

	RectTransform rectTransform => GetComponent<RectTransform>();
	Image pathColor => GetComponent<Image>();

	IDebugable debuguableInterface => (IDebugable) this;
	IInitializable initializableInterface => (IInitializable) this;

	bool IInitializable.initializedInternal { get; set; }
	string IDebugable.debugLabel => "<b>[Path] : </b>";

	State state;

	public void Init(Transform start, SubCategory startClue = SubCategory.EMPTY)
	{
		this.start = start;

		this.startClue = startClue;
		endClue = SubCategory.EMPTY;

		initializableInterface.InitInternal();
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;
		Debug.Log(debuguableInterface.debugLabel + "Initializing done");
	}

	public void UpdatePath(Vector3 newEnd = default(Vector3), State newState = State.NORMAL)
	{
		if(!initialized)
		{
			Debug.LogError(debuguableInterface.debugLabel + "Not initialized");
			return;
		}

		Vector3 endPosition = newEnd != default(Vector3) ? newEnd : end.localPosition;
		Vector3 offset = (endPosition - start.localPosition);

		// moves path center
		transform.localPosition = start.localPosition + offset / 2;

		// rotates path
		float angle = Vector3.SignedAngle(Vector3.right, offset, Vector3.forward);
		transform.rotation = Quaternion.Euler(0, 0, angle);

		// set height
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, pathThickness);

		// set width
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Vector3.Distance(start.localPosition, endPosition));

		// puts the path on top of hierarchy to render behind knobs
		transform.SetAsFirstSibling();

		state = newState;

		SetColorFromState();
	}

	public void SetEnd(Transform end, SubCategory clue)
	{
		this.end = end;
		endClue = clue;

		UpdatePath();
	}

	public Transform GetEnd()
	{
		if(!initialized)
		{
			Debug.LogError(debuguableInterface.debugLabel + "Not initialized");
			return null;
		}

		return end;
	}

	// checks state of path depending on knob clues
	public SubCategory CheckState()
	{
		if(!initialized)
		{
			Debug.LogError(debuguableInterface.debugLabel + "Not initialized");
			return SubCategory.EMPTY;
		}

		if(startClue == endClue)
		{
			state = State.VALIDATED;
			SetColorFromState();

			return startClue;
		}
		else
		{
			state = State.WRONG;
			SetColorFromState();

			return SubCategory.EMPTY;
		}

	}

	void SetColorFromState()
	{
		switch(state)
		{
			case State.NORMAL:
				pathColor.color = normalColor;
				break;
			case State.VALIDATED:
				pathColor.color = validatedColor;
				break;
			case State.WRONG:
				pathColor.color = wrongColor;
				break;
		}
	}

	public bool ContainsKnob(Transform knob)
	{
		if(!initialized)
		{
			Debug.LogError(debuguableInterface.debugLabel + "Not initialized");
			return false;
		}

		return start == knob || end == knob;
	}

	public bool Compare(Path path)
	{
		return path.start == start && path.end == end;
	}
}