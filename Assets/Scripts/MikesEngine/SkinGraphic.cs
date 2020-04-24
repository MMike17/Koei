using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class SkinGraphic : MonoBehaviour
{
    public SkinTag skin_tag;

    Graphic graphic { get { return GetComponent<Graphic>(); } }

    bool is_skinned;

    void Awake()
    {
        is_skinned = false;
    }

    void Update()
    {
        if(!is_skinned && Skinning.IsReady())
        {
            Skin();
            Skinning.Register(this);
        }
    }

    public void Skin()
    {
        is_skinned = true;

        graphic.color = Skinning.GetSkin(skin_tag);
    }

    void OnDestroy()
    {
        Skinning.Resign(this);
    }
}