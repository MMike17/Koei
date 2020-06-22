using TMPro;
using UnityEngine;

public class ConclusionCard : MonoBehaviour, IInitializable, IDebugable
{
	[Header("Assign in Inspector")]
	public TextMeshProUGUI comment;
	public Animator animator;

	[Header("Debug")]
	public Conclusion conclusion;

	public bool initialized => initializableInterface.initializedInternal;

	IInitializable initializableInterface => (IInitializable) this;
	IDebugable debugableInterface => (IDebugable) this;

	string IDebugable.debugLabel => "<b>[DesignedCard] : </b>";
	bool IInitializable.initializedInternal { get; set; }

	bool unlocked;

	public void Init(Conclusion data)
	{
		conclusion = data;

		comment.text = data.comment;
		comment.color = GameData.GetColorFromCategory(data.category);

		initializableInterface.InitInternal();
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;

		Debug.Log(debugableInterface.debugLabel + "Initializing done");
	}

	public void HideCard()
	{
		if(!initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "Not initialized");
			return;
		}

		animator.Play("Idle");
		unlocked = false;
	}

	public void ShowCard(bool playSound = true)
	{
		if(!initialized)
		{
			Debug.LogError(debugableInterface.debugLabel + "Not initialized");
			return;
		}

		animator.Play("Unlock");

		if(playSound)
			AudioManager.PlaySound("Swish");

		unlocked = true;
	}

	public bool IsUnlocked(bool playAnim = false)
	{
		if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Unlock") && playAnim)
			animator.Play("Locked");

		return unlocked;
	}
}