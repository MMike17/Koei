using System;
using TMPro;
using UnityEngine;

public class ConsequencesManager : MonoBehaviour, IDebugable, IInitializable
{
	[Header("Assign in Inspector")]
	public DialogueWriter writer;
	public Animator canvas;

	IDebugable debuguableInterface => (IDebugable) this;
	IInitializable initializableInterface => (IInitializable) this;

	public bool initialized => initializableInterface.initializedInternal;

	bool IInitializable.initializedInternal { get; set; }
	string IDebugable.debugLabel => "<b>[TitleManager] : </b>";

	Action toShogun;
	GameData.GameState actualState;
	int combatIndex;
	bool isStarted, mapShow;

	public void Init(GameData.GameState state, int combatIndex, Action toShogunCallback, Action advanceEnemy, string textToShow = null)
	{
		if(state == GameData.GameState.NORMAL && !string.IsNullOrEmpty(textToShow))
		{
			writer.line = textToShow;
			advanceEnemy.Invoke();
		}

		writer.GetComponent<TextMeshProUGUI>().color = Skinning.GetSkin(SkinTag.VALIDATE);
		writer.highlightColor = Skinning.GetSkin(SkinTag.DELETE);
		writer.SetAudio(() => AudioManager.PlaySound("Writting"), () => AudioManager.StopSound("Writting"));
		writer.Play();

		canvas.Play("Deity");

		isStarted = false;
		mapShow = false;
		actualState = state;
		this.combatIndex = combatIndex;
		toShogun = toShogunCallback;

		initializableInterface.InitInternal();
	}

	void Update()
	{
		if(writer.isDone)
		{
			if(!isStarted)
			{
				isStarted = true;

				if(actualState == GameData.GameState.NORMAL)
				{
					canvas.Play("Fade");
					Invoke("AdvanceMap", 3);
				}
			}
			else if(Input.GetMouseButtonDown(0))
				toShogun.Invoke();
		}
	}

	void AdvanceMap()
	{
		canvas.Play("Show" + (combatIndex + 1));
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;
		Debug.Log(debuguableInterface.debugLabel + "Initializing done");
	}
}