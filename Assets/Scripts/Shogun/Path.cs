using UnityEngine;
using UnityEngine.UI;

public class Path : MonoBehaviour, IInitializable, IDebugable
{
	[Header("Settings")]
	public float pathThickness;
	public Color normalColor, validatedColor, wrongColor;

	[Header("Debug")]
	public Transform start;
	public Vector3 end;
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

	void OnDrawGizmos()
	{
		if(start != null && end != null)
			SetPath(State.NORMAL);
	}

	public void Init(Transform start, Vector3 end, SubCategory startClue = SubCategory.EMPTY)
	{
		this.start = start;
		this.end = end;

		this.startClue = startClue;
		endClue = SubCategory.EMPTY;

		initializableInterface.InitInternal();
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;
		Debug.Log(debuguableInterface.debugLabel + "Initializing done");
	}

	public void SetPath(State newState = State.NORMAL)
	{
		Vector3 offset = (end - start.localPosition);

		// moves path center
		transform.localPosition = start.localPosition + offset / 2;

		// rotates path
		float angle = Vector3.SignedAngle(start.right, offset, Vector3.forward);
		transform.rotation = Quaternion.Euler(0, 0, angle);

		// set height
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, pathThickness);

		// set width
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Vector3.Distance(start.localPosition, end));

		// puts the path on top of hierarchy to render behind knobs
		transform.SetAsFirstSibling();

		state = newState;

		SetColorFromState();
	}

	public void SetEnd(Vector3 end, SubCategory clue = SubCategory.EMPTY)
	{
		this.end = end;
		endClue = clue;
	}

	public Vector3 GetEnd()
	{
		return end;
	}

	// checks state of path depending on knob clues
	public SubCategory CheckState()
	{
		if(startClue == endClue)
		{
			state = State.VALIDATED;
			SetColorFromState();

			return startClue;
		}
		else
		{
			state = State.VALIDATED;
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
		return start == knob || end == knob.position;
	}
}