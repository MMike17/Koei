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

	Action toShogun, toFight, toEnd;
	GameData.GameState actualState;
	int combatIndex;
	bool isStarted, mapShow;

	public void Init(GameData.GameState state, int combatIndex, Action toEndCallback, Action toShogunCallback, Action toFightCallback, Action advanceEnemy, string textToShow)
	{
		if(state != GameData.GameState.GAME_OVER_GENERAL && state != GameData.GameState.GAME_OVER_FINISHER)
			advanceEnemy.Invoke();

		writer.GetComponent<TextMeshProUGUI>().color = Skinning.GetSkin(SkinTag.VALIDATE);
		writer.SetAudio(() => AudioManager.PlaySound("Writting"), () => AudioManager.StopSound("Writting"));
		writer.Play(textToShow, Skinning.GetSkin(SkinTag.DELETE));

		canvas.Play("Deity");

		isStarted = false;
		mapShow = false;
		actualState = state;
		this.combatIndex = combatIndex;

		toEnd = toEndCallback;
		toFight = toFightCallback;
		toShogun = toShogunCallback;

		initializableInterface.InitInternal();
	}

	void IInitializable.InitInternal()
	{
		initializableInterface.initializedInternal = true;
		Debug.Log(debuguableInterface.debugLabel + "Initializing done");
	}

	void Update()
	{
		if(writer.isDone)
		{
			if(!isStarted)
			{
				isStarted = true;

				if(actualState != GameData.GameState.GAME_OVER_GENERAL && actualState != GameData.GameState.GAME_OVER_FINISHER)
				{
					canvas.Play("Fade");
					Invoke("AdvanceMap", 3);
				}
			}
			else if(Input.GetMouseButtonDown(0))
			{
				switch(actualState)
				{
					case GameData.GameState.GAME_OVER_GENERAL:
						toFight.Invoke();
						break;
					case GameData.GameState.GAME_OVER_FINISHER:
						toShogun.Invoke();
						break;
					default:
						toEnd.Invoke();
						break;
				}
			}
		}
	}

	void AdvanceMap()
	{
		canvas.Play("Show" + (combatIndex + 1));
	}
}