using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Combat Dialogue", menuName = "Koei/Combat Dialogue")]
public class CombatDialogue : ScriptableObject
{
	public GameManager.Enemy enemy;
	[Space]
	public List<SubCategory> weaknesses;
	[Space]
	public int tries;

	[Space]
	public Sprite[] sceneBackgrounds = new Sprite[3];
	[Space]
	// 0 = damage / 1-2-3 = suicide / 4+ = HP (0 to full)
	public Sprite[] enemySprites;
	[Space]
	public List<Replica> preCombatReplicas;
	[Space]
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
	public string playerWinConsequence;

	public bool alreadyLost;

	public void Init()
	{
		alreadyLost = false;
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