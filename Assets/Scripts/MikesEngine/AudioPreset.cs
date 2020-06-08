using UnityEngine;

[CreateAssetMenu(fileName = "AudioPreset", menuName = "Tools/AudioPreset")]
public class AudioPreset : ScriptableObject
{
	public enum LifeMode { ATTACH_TO, ON_CAMERA, WORLD_POINT }
	public enum Spacialisation { TwoD, ThreeD }

	public Spacialisation dMode;
	public LifeMode lifeMode;
}