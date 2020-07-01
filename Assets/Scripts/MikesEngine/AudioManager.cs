using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Component for sound spawning and pooling</summary>
public class AudioManager : MonoBehaviour
{
	/// <value>Static instance of the AudioManager</value>
	static AudioManager instance;

	[Header("Settings")]
	public int maxSoundEmitters;
	[Space]
	[SerializeField] SoundClip[] clips;

	[Header("Assign in the Inspetor")]
	public GameObject soundEffectModel;

	List<SoundClip> pool;
	bool initialized = false;

	public void Init()
	{
		initialized = true;
		SceneManager.activeSceneChanged += (previous, next) => ChangeScene();

		if(instance == null)
		{
			instance = this;

			pool = new List<SoundClip>();
		}
		else if(instance != this)
			Destroy(gameObject);

		Debug.Log("<b>[" + GetType() + "] : </b>Initializing done");
	}

	void Update()
	{
		if(initialized)
			RefreshPool();
	}

	void RefreshPool()
	{
		List<SoundClip> toDelete = new List<SoundClip>();

		int forceDelete = 0;

		foreach (SoundClip entry in pool)
		{
			entry.CheckDone();
			entry.Follow();

			if(entry.donePlaying)
			{
				if(pool.Count - forceDelete > maxSoundEmitters || entry.destroyOnDone)
				{
					forceDelete++;
					toDelete.Add(entry);
				}
			}
		}

		foreach (SoundClip entry in toDelete)
		{
			pool.Remove(entry);

			if(entry.attachedSource != null)
				Destroy(entry.attachedSource.gameObject);
		}
	}

	/// <summary>Command to spawn a prefab that will play a given sound.static Doesn't require object ref</summary>
	/// <param name="clip_name">name of the SoundClip you want to play</param>
	/// <param name="point">position you want to spawn the prefab to (Vector3.zero by default)</param>
	/// <param name="destroy_on_done">sets wether the prefab should be destroyed when done playing</param>
	public static void PlaySound(string clip_name, Action callback = null, bool destroy_on_done = false, Vector3 point = default(Vector3))
	{
		SoundClip selected_sound = AudioManager.instance.FindClipByName(clip_name);

		if(selected_sound == null)
		{
			Debug.LogError("<b>[" + AudioManager.instance.GetType() + "] : </b>Couldn't find SoundClip with name \"" + clip_name + "\"");

			if(callback != null)
				callback.Invoke();
			return;
		}

		AudioSource selected_from_pool = null;

		List<SoundClip> sort = new List<SoundClip>();

		foreach (SoundClip entry in AudioManager.instance.pool)
		{
			if(entry.donePlaying)
				sort.Add(entry);
		}

		if(sort.Count > 0)
		{
			selected_from_pool = sort[0].attachedSource;
			AudioManager.instance.pool.Remove(sort[0]);
		}
		else
		{
			GameObject temp = Instantiate(AudioManager.instance.soundEffectModel);

			if(temp.GetComponent<AudioSource>() != null)
				selected_from_pool = temp.GetComponent<AudioSource>();
			else
				selected_from_pool = temp.AddComponent<AudioSource>();
		}

		selected_sound.attachedSource = selected_from_pool;
		selected_sound.SetupSource(selected_from_pool, point, callback);

		AudioManager.instance.pool.Add(new SoundClip(selected_sound));
		selected_sound.attachedSource.Play();
	}

	/// <summary>Calls ShootSound with default parameters (for UI buttons). Doesn't require object ref</summary>
	/// <param name="clip_name">name of the SoundClip you want to play</param>
	public static void PlaySound(string clip_name)
	{
		AudioManager.PlaySound(clip_name, null, false, default(Vector3));
	}

	/// <summary>Calls ShootSound with default parameters (for UI buttons). Doesn't require object ref</summary>
	/// <param name="clip_name">name of the SoundClip you want to play</param>
	public static void StopSound(string clip_name)
	{
		int index = AudioManager.instance.pool.FindIndex((item) => { return item.GetName() == clip_name; });

		if(index >= 0)
		{
			AudioManager.instance.pool[index].Finish();
			AudioManager.instance.pool[index].Stop();
		}
		else
			Debug.LogError("<b>[" + AudioManager.instance.GetType() + "] : </b>Couldn't find active source with name \"" + clip_name + "\"");
	}

	SoundClip FindClipByName(string name)
	{
		foreach (SoundClip clip in clips)
		{
			if(clip.GetName() == name)
				return clip;
		}

		return null;
	}

	void ChangeScene()
	{
		foreach (SoundClip clip in pool)
			clip.Clear();

		pool.Clear();
	}

	[Serializable]
	internal class SoundClip
	{
		[SerializeField] string name;
		[SerializeField] AudioClip clip;
		[Range(0, 1)]
		[SerializeField] float volume;
		[SerializeField] Transform attachTo;
		[SerializeField] AudioPreset preset;

		public bool donePlaying { get; private set; }
		public bool destroyOnDone { get; private set; }
		public AudioSource attachedSource { get; set; }

		Action doneCallback;

		public SoundClip(SoundClip model)
		{
			name = model.name;
			clip = model.clip;
			volume = model.volume;
			attachTo = model.attachTo;
			preset = model.preset;

			destroyOnDone = model.destroyOnDone;
			attachedSource = model.attachedSource;
			model.donePlaying = false;

			doneCallback = model.doneCallback;
		}

		public void SetupSource(AudioSource source, Vector3 point, Action callback)
		{
			AudioSource sound_source = source;
			attachedSource = source;

			sound_source.clip = clip;
			sound_source.volume = volume;
			sound_source.spatialBlend = preset.dMode == AudioPreset.Spacialisation.TwoD?0 : 1;
			sound_source.spatialize = preset.dMode == AudioPreset.Spacialisation.ThreeD;

			donePlaying = false;

			doneCallback = callback;

			switch(preset.lifeMode)
			{
				case AudioPreset.LifeMode.ATTACH_TO:
					if(attachTo != null)
						attachedSource.transform.position = attachTo.position;
					break;
				case AudioPreset.LifeMode.ON_CAMERA:
					attachedSource.transform.SetParent(Camera.main.transform);
					break;
				case AudioPreset.LifeMode.WORLD_POINT:
					attachedSource.transform.position = point;
					attachedSource.transform.parent = null;
					break;
			}
		}

		public string GetName()
		{
			return name;
		}

		public float GetClipLength()
		{
			if(clip != null)
				return clip.length;
			else
				return 0;
		}

		public void SetTarget(Transform target)
		{
			attachTo = target;
		}

		public void Follow()
		{
			if(preset.lifeMode == AudioPreset.LifeMode.ATTACH_TO)
				attachedSource.transform.position = attachTo.position;
		}

		public void CheckDone()
		{
			if(attachedSource == null || (attachedSource != null && !attachedSource.loop && attachedSource.time >= GetClipLength()))
			{
				Finish();
				donePlaying = true;
			}
		}

		public void Clear()
		{
			if(attachedSource != null)
				Destroy(attachedSource.gameObject);
		}

		public void Stop()
		{
			if(attachedSource != null)
				attachedSource.Stop();
		}

		public void Finish()
		{
			if(doneCallback != null)
				doneCallback.Invoke();
		}
	}
}