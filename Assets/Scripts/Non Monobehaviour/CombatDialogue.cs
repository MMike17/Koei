using System;
using System.Collections.Generic;
using UnityEngine;
using static GameData;

// class used for the whole Fight phase
[CreateAssetMenu(fileName = "Combat Dialogue", menuName = "Koei/Combat Dialogue")]
public class CombatDialogue : ScriptableObject
{
	public GameManager.Enemy enemy;
	[Space]
	public List<SubCategory> weaknesses;
	[Space]
	public int allowedErrors;

	[Space]
	public Sprite[] sceneBackgrounds = new Sprite[3];
	[Space]
	[Header("7 = HP 3")]
	[Header("6 = HP 2")]
	[Header("5 = HP 1")]
	[Header("4 = HP 0")]
	[Header("1/2/3 = suicide")]
	[Header("0 = damage")]
	public Sprite[] enemySprites;
	[Space]
	public List<Replica> preCombatReplicas;
	[Space]
	[TextArea]
	public string preCombatReturnReplica;
	[Space]
	public Finishers finisherPunchlines;
	[Space]
	[TextArea]
	public string playerWinResponse;
	[TextArea]
	public string playerLoseResponse;
	[TextArea]
	public string playerFinisherLoseResponse;
	[Space]
	[TextArea]
	public string playerWinConsequence, playerLoseGeneralConsequence, playerLoseFinalConsequence;

	public GameState actualState { get; set; }

	public void Init()
	{
		actualState = GameState.NORMAL;
	}

	[Serializable]
	public class Replica
	{
		[TextArea]
		public string playerLine, enemyLine;
	}

	[Serializable]
	public class Finishers
	{
		[TextArea]
		public string[] finishers;
		public int correctOne;
	}
}