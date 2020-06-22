using UnityEngine;

public class FightTest : MonoBehaviour
{
	[Header("Settings")]
	public bool isTesting = false;

	[Header("Assign in Inspector")]
	public CombatDialogue testedDialogue;
	public GeneralDialogue generalDialogue;
	public FightManager fightManager;
	public GeneralPunchlines comonPunchlines;
	public SkinData skin;

	void Awake()
	{
		if(!isTesting)
			return;

		Skinning.Init(skin);

		fightManager.PreInit(testedDialogue);
		fightManager.Init(
			true,
			comonPunchlines,
			generalDialogue,
			() => Debug.Log("Can't go to consequences in scene test mode"),
			() => Debug.Log("Can't go to GameOver when in scene test mode")
		);
	}
}