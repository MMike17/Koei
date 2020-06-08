using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class SkinGraphic : MonoBehaviour
{
	public SkinTag skin_tag;

	Graphic graphic { get { return GetComponent<Graphic>(); } }
	TextMeshProUGUI graphicText { get { return GetComponent<TextMeshProUGUI>(); } }

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

		if(graphicText == null && graphic == null)
		{
			Debug.LogError("<b>[" + GetType() + "] : </b>" + "Doesn't have attached graphic component", gameObject);
			Destroy(this);
			return;
		}

		if(graphicText != null)
			graphicText.color = Skinning.GetSkin(skin_tag);
		else
			graphic.color = Skinning.GetSkin(skin_tag);
	}

	void OnDestroy()
	{
		Skinning.Resign(this);
	}
}