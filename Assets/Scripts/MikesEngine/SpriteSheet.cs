using System;
using UnityEngine;

/// <summary>Object containing references to sprites and animations</summary>
[CreateAssetMenu(fileName = "SpriteSheet", menuName = "Tools/Sprite Sheet")]
public class SpriteSheet : ScriptableObject
{
    /// <value>Texture for automated sprite importing</value>
    [Header("Place sprite sheet here to extract sprites")]
    [HideInInspector]
    public Texture2D imported_texture;

    /// <value>Array of the animation clips in the SpriteSheet</value>
    [Header("Settings")]
    public AnimationClip[] animation_clips;

    /// <value>Array of sprites</value>
    [Header("Assign in Inspector")]
    public Sprite[] bank;
}

/// <summary>Class that contains animation infos</summary>
[Serializable]
public class AnimationClip
{
    /// <value>Name of the AnimationClip</value>
    public string name;

    /// <value>Indexes for beginning and end frame of animation</value>
    public int start_index,end_index;

    /// <value>Animation speed (frames per second)</value>
    [Tooltip("frames per second")]
    public float frame_rate;

    /// <value>Wether the animation will loop or not</value>
    public bool loop;
}