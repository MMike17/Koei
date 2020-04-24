using System.Collections.Generic;
using UnityEngine;

public enum SkinTag
{
	BACKGROUND,

	PRIMARY_WINDOW,
	PRIMARY_ELEMENT,

	SECONDARY_WINDOW,
	SECONDARY_ELEMENT,

	CONTRAST,

	VALIDATE,
	DELETE,

	PICTO
}

public static class Skinning
{
	static SkinData actual_skin;

	static List<SkinGraphic> components;

	public static void Init(SkinData data)
	{
		actual_skin = data;
	}

	public static Color GetSkin(SkinTag tag)
	{
		if(!IsReady())
		{
			Debug.LogError("<b>[Skinning]</b> : skinning is not ready (skin data has not been assigned)");
			return Color.black;
		}

		return actual_skin.GetSkin(tag);
	}

	public static bool IsReady()
	{
		return actual_skin != null;
	}

	public static void Register(SkinGraphic graphic)
	{
		if(components == null)
			components = new List<SkinGraphic>();

		components.Add(graphic);
	}

	public static void Resign(SkinGraphic graphic)
	{
		if(components == null)
		{
			components = new List<SkinGraphic>();
			return;
		}

		components.Remove(graphic);
	}

	public static void ResetSkin(SkinData data)
	{
		actual_skin = data;
		components.ForEach((item) => { item.Skin(); });
	}
}