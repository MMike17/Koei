using UnityEngine;
using UnityEngine.UI;

public class Path : MonoBehaviour, IInitializable, IDebugable
{
	[Header("Settings")]
	public float pathThickness;

	[Header("Debug")]
	public Transform start;
	public Transform end;
	public SubCategory startClue;
	public SubCategory endClue;

	public enum State
	{
		NORMAL,
		VALIDATED,
		WRONG,
		OLD
	}

	public bool initialized => initializableInterface.initializedInternal;

	RectTransform rectTransform => GetComponent<RectTransform>();

	IDebugable debuguableInterface => (IDebugable) this;
	IInitializable initializableInterface => (IInitializable) this;

	bool IInitializable.initializedInternal { get; set; }
	string IDebugable.debugLabel => "<b>[Path] : </b>";

	State state;
	Color normalColor, validatedColor, wrongColor, oldColor;

	public void Init(Transform start, SkinTag normal, SkinTag validated, SkinTag wrong, SkinTag old, SubCategory startClue = SubCategory.EMPTY)
	{
		this.start = start;

		this.startClue = startClue;
		endClue = SubCategory.EMPTY;

		normalColor = Skinning.GetSkin(normal);
		validatedColor = Skinning.GetSkin(validated);
		wrongColor = Skinning.GetSkin(wrong);
		oldColor = Skinning.GetSkin(old);

		initializableInterface.InitInternal();
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;
		Debug.Log(debuguableInterface.debugLabel + "Initializing done");
	}

	public void UpdatePath(bool hasPos = false, Vector3 newEnd = default(Vector3), State newState = State.NORMAL)
	{
		if(!initialized)
		{
			Debug.LogError(debuguableInterface.debugLabel + "Not initialized");
			return;
		}

		Vector3 endPosition = hasPos ? newEnd : end.localPosition;
		Vector3 offset = (endPosition - start.localPosition);

		// moves path center
		transform.localPosition = start.localPosition + offset / 2;

		// rotates path
		float angle = Vector3.SignedAngle(Vector3.right, offset, Vector3.forward);
		transform.rotation = Quaternion.Euler(0, 0, angle + transform.parent.eulerAngles.z);

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

		if(end != null)
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
				SetColor(normalColor);
				break;
			case State.VALIDATED:
				SetColor(validatedColor);
				break;
			case State.WRONG:
				SetColor(wrongColor);
				break;
			case State.OLD:
				SetColor(oldColor);
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

	public void SetColor(Color color)
	{
		Image pathColor = GetComponent<Image>();

		pathColor.color = color;
	}

	public bool Compare(Path other)
	{
		return ContainsKnob(other.start) && ContainsKnob(other.end);
	}

	public void SetOld()
	{
		state = State.OLD;
		SetColorFromState();
	}

	public State GetState()
	{
		return state;
	}
}