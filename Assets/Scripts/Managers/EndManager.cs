using UnityEngine;

public class EndManager : MonoBehaviour, IInitializable, IDebugable
{
	[Header("Assign in Inspector")]
	public DialogueWriter writer;

	IInitializable initializableInterface => (IInitializable) this;
	IDebugable debugableInterface => (IDebugable) this;

	public bool initialized => initializableInterface.initializedInternal;

	bool IInitializable.initializedInternal { get; set; }

	string IDebugable.debugLabel => "<b>[" + GetType() + "] : </b>";

	public void Init()
	{
		writer.SetAudio(() => AudioManager.PlaySound("Writting"), () => AudioManager.StopSound("Writting"));
		writer.Play("Fin de la démo\n\n\n\n\n\n\nMerci d'avoir joué", Skinning.GetSkin(SkinTag.DELETE));

		initializableInterface.InitInternal();
	}

	void IInitializable.InitInternal()
	{
		Debug.Log(debugableInterface.debugLabel + "Initilizing done");

		initializableInterface.initializedInternal = true;
	}
}