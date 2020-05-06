using UnityEngine;
using UnityEngine.UI;

/// <summary>Component to animate UI</summary>
[RequireComponent(typeof(Image))]
public class AnimationSystemUI : MonoBehaviour
{
    /// <value>SpriteSheet containing all the animations</value>
    [Header("Assing in Inspector")]
    public SpriteSheet sprite_sheet;

    /// <value>Value to check if animation is finished</value>
    [HideInInspector]
    public bool has_finished;

    Image sprite;
    AnimationClip currently_playing;
    int current_index;
    float current_frame_rate;
    float timer;

    void OnDrawGizmos ()
    {
        if(!Application.isPlaying && GetComponent<Image>()!=null && sprite_sheet!=null && sprite_sheet.animation_clips.Length>0)
            GetComponent<Image>().sprite=sprite_sheet.bank[sprite_sheet.animation_clips[0].start_index];
    }

    void Awake ()
    {
        sprite=GetComponent<Image>();
        timer=0;
        current_frame_rate=0;
        current_index=0;
        currently_playing=null;
    }

    void Update ()
    {
        if(currently_playing!=null)
        {
            if(timer<=1/current_frame_rate)
                timer+=Time.deltaTime;
            else if(current_index<=currently_playing.end_index)
            {
                timer=0;
                current_index++;
            }

            if(current_index<currently_playing.start_index)
                current_index=currently_playing.start_index;

            if(currently_playing.loop && current_index>currently_playing.end_index)
                current_index=currently_playing.start_index;

            if(current_index<=currently_playing.end_index)
                sprite.sprite=sprite_sheet.bank[current_index];
        }
        else
            timer=0;
    }

    AnimationClip GetAnimationClip (string name)
    {
        foreach (AnimationClip clip in sprite_sheet.animation_clips)
        {
            if(clip.name==name)
                return clip;
        }

        Debug.LogError("<b>[AnimationSystem]</b> : no animation found with this name");
        return new AnimationClip();
    }

    /// <summary>Plays the AnimationClip with provided name if it exists</summary>
    /// <param name="name">the name of the AnimationClip to play (defined in SpriteSheet object)</param>
    public void PlayClip (string name)
    {
        AnimationClip initial=currently_playing;
        currently_playing=GetAnimationClip(name);

        if(currently_playing!=initial)
            current_frame_rate=currently_playing.frame_rate;
    }

    /// <summary>Sets the frame rate of the AnimationClip with the provided name if it exists</summary>
    /// <param name="name">the name of the AnimationClip to play (defined in SpriteSheet object)</param>
    /// <param name="rate">the new rate at which the AnimationClip should play (defined in SpriteSheet object)</param>
    public void SetAnimationRate (float rate)
    {
        current_frame_rate=rate;
    }

    /// <summary>Gets informations about state of the animation and component</summary>
    public AnimationInfo GetAnimationState ()
    {
        return new AnimationInfo(currently_playing,current_index,current_frame_rate);
    }

    /// <summary>Class that describe the state of the AnimationSystemUI</summary>
    public struct AnimationInfo
    {
        /// <value>Name of the currently played AnimationClip</value>
        public string animation_name {get{if(current_clip!=null) return current_clip.name; else return null;}}
        /// <value>First index of the currently played AnimationClip</value>
        public int start_index {get{if(current_clip!=null) return current_clip.start_index; else return 0;}}
        /// <value>Current index of the currently played AnimationClip</value>
        public int current_index {get{return internal_current_index;}}
        /// <value>Last index of the currently played AnimationClip</value>
        public int end_index {get{if(current_clip!=null) return current_clip.end_index; else return 0;}}
        /// <value>Initial frame rate of the currently played AnimationClip</value>
        public float animation_frame_rate {get{if(current_clip!=null) return current_clip.frame_rate; else return 0;}}
        /// <value>Currently used frame rate of the currently played AnimationClip</value>
        public float current_frame_rate {get{if(current_clip!=null) return internal_current_frame_rate; else return 0;}}
        /// <value>Wether the currently played AnimationClip will loop or not</value>
        public bool loop {get{if(current_clip!=null) return current_clip.loop; else return false;}}

        AnimationClip current_clip;
        float internal_current_frame_rate;
        int internal_current_index;

        public AnimationInfo (AnimationClip clip, int current_index, float current_frame_rate)
        {
            current_clip=clip;

            this.internal_current_index=current_index;
            this.internal_current_frame_rate=current_frame_rate;
        }
    }
}