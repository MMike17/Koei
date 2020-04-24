using System;
using UnityEngine;

/// <summary>Class to fix the paralax so it has bounds</summary>
public class Paralax : MonoBehaviour
{
    [Header("Assign in Inspector")]
    /// <value>Object that moves, considered as the player's Avatar</value>
    public Transform player;
    [SerializeField] Threshold player_bounds_x;
    [SerializeField] Threshold player_bounds_y;
    /// <value>Array of sprites to move around</value>
    public Transform[] layers;
    [SerializeField] Color top,down;
    [SerializeField] Camera main_camera;

    Color side;
    Threshold[] layers_target_x,layers_target_y;

    void OnDrawGizmos ()
    {
        float top_h,top_s,top_v;
        float down_h,down_s,down_v;

        Color.RGBToHSV(top,out top_h,out top_s,out top_v);
        Color.RGBToHSV(down,out down_h,out down_s,out down_v);

        side=Color.HSVToRGB(Mathf.Lerp(top_h,down_h,0.5f),Mathf.Lerp(top_s,down_s,0.5f),Mathf.Lerp(top_v,down_v,0.5f));

        Debug.DrawLine(new Vector2(player_bounds_x.min,player_bounds_y.min),new Vector2(player_bounds_x.max,player_bounds_y.min),down);
        Debug.DrawLine(new Vector2(player_bounds_x.min,player_bounds_y.max),new Vector2(player_bounds_x.max,player_bounds_y.max),top);
        Debug.DrawLine(new Vector2(player_bounds_x.min,player_bounds_y.min),new Vector2(player_bounds_x.min,player_bounds_y.max),side);
        Debug.DrawLine(new Vector2(player_bounds_x.max,player_bounds_y.min),new Vector2(player_bounds_x.max,player_bounds_y.max),side);
    }

    void Update ()
    {
        layers_target_x=new Threshold[layers.Length];
        layers_target_y=new Threshold[layers.Length];

        float percentile_x=(player.position.x-player_bounds_x.min)/player_bounds_x.extent;
        float percentile_y=(player.position.y-player_bounds_y.min)/player_bounds_y.extent;
        
        // need to calculate sprite bounds at every frame
        for(int i=0;i<layers.Length;i++)
        {
            CalculateTargets(i);

            layers[i].position=new Vector3(
                layers_target_x[i].max-layers_target_x[i].extent*percentile_x,
                layers_target_y[i].max-layers_target_y[i].extent*percentile_y,
                layers[i].position.z
                );
        }
    }

    void CalculateTargets (int index)
    {
        layers_target_x[index].max=main_camera.ViewportToWorldPoint(new Vector2(0,0)).x+layers[index].GetComponent<SpriteRenderer>().sprite.bounds.extents.x*layers[index].localScale.x;

        layers_target_x[index].min=main_camera.ViewportToWorldPoint(new Vector2(1,0)).x-layers[index].GetComponent<SpriteRenderer>().sprite.bounds.extents.x*layers[index].localScale.x;

        layers_target_y[index].max=main_camera.ViewportToWorldPoint(new Vector2(0,0)).y+layers[index].GetComponent<SpriteRenderer>().sprite.bounds.extents.y*layers[index].localScale.y;
        
        layers_target_y[index].min=main_camera.ViewportToWorldPoint(new Vector2(0,1)).y-layers[index].GetComponent<SpriteRenderer>().sprite.bounds.extents.y*layers[index].localScale.y;
    }

    /// <summary>Sets the player's minimum and maximum positions for calculating paralax</summary>
    /// <param name="min_x">The minimum position of the player on the x axis</param>
    /// <param name="max_x">The maximum position of the player on the x axis</param>
    /// <param name="min_y">The minimum position of the player on the y axis</param>
    /// <param name="max_y">The maximum position of the player on the y axis</param>
    public void SetPlayerBounds (float min_x, float max_x, float min_y, float max_y)
    {
        player_bounds_x=new Threshold(min_x,max_x);
        player_bounds_y=new Threshold(min_y,max_y);
    }

    /// <summary>Sets the player's minimum and maximum positions for calculating paralax</summary>
    /// <param name="min">The minimum position of the player</param>
    /// <param name="max">The maximum position of the player</param>
    public void SetPlayerBounds (Vector2 min, Vector2 max)
    {
        player_bounds_x=new Threshold(min.x,max.x);
        player_bounds_y=new Threshold(min.y,max.y);
    }

    [Serializable]
    struct Threshold
    {
        public float min,max;
        public float extent {get{return max-min;}}

        public Threshold (float min, float max)
        {
            this.min=min;
            this.max=max;
        }
    }
}