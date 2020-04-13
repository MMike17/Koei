using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsequencesTest : MonoBehaviour
{
	[Header("Settings")]
	public bool isTesting = false;
	public bool isGameOver;
	public int combatIndex;

	[Header("Assign in Inspector")]
	public ConsequencesManager consequencesManager;

	void Awake()
	{
		if(isTesting)
		{
			consequencesManager.Init(isGameOver, combatIndex, () => { Debug.Log("Can't go to shogun scene while in testing mode"); });
		}
	}
}