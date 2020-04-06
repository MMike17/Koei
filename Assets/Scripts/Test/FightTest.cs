using UnityEngine;

public class FightTest : MonoBehaviour
{
	[Header("Settings")]
	public bool isTesting = false;

	[Header("Assign in Inspector")]
	public CombatDialogue testedDialogue;
	public FightManager fightManager;
	public GeneralPunchlines comonPunchlines;

	void Awake()
	{
		if(!isTesting)
			return;

		fightManager.PreInit(testedDialogue);
		fightManager.Init(comonPunchlines, () => Debug.Log("Can't go to consequences in scene test mode"), () => Debug.Log("Can't go to GameOver when in scene test mode"));
	}
}