using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsequencesTest : MonoBehaviour
{
	[Header("Settings")]
	public bool isTesting = false;
	public GameData.GameState testState;
	public int combatIndex;

	[Header("Assign in Inspector")]
	public ConsequencesManager consequencesManager;

	void Awake()
	{
		if(isTesting)
		{
			consequencesManager.Init(testState, combatIndex, () => { Debug.Log("Can't go to shogun scene while in testing mode"); }, () => { Debug.Log("Can't advance enemy phase while in testing mode"); });
		}
	}
}