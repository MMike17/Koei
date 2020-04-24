using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkinData))]
public class SkinDataEditor : Editor
{
	const float sample_square_size = 50;
	const float inset_square_size = 20;
	const float border_size = 5;

	const float square_difference = (sample_square_size - inset_square_size) / 2;

	const float spacing = 5;
	const float edge_offset = 10;

	SkinData data;

	public override void OnInspectorGUI()
	{
		data = target as SkinData;

		if(!data.are_slots_valid)
			data.FixSlots();

		// container
		Vector2 container_size = new Vector2(sample_square_size * 3 + spacing * 4, sample_square_size * 2 + spacing * 3);
		Vector2 container_offset = Vector2.one * edge_offset;

		EditorGUI.DrawRect(new Rect(container_offset, container_size), data.GetSkin(SkinTag.BACKGROUND));

		// samples
		float first = edge_offset + spacing;
		float second = edge_offset + spacing * 2 + sample_square_size;
		float third = edge_offset + spacing * 3 + sample_square_size * 2;

		EditorGUI.DrawRect(new Rect(Vector2.one * first, Vector2.one * sample_square_size), data.GetSkin(SkinTag.PICTO));
		EditorGUI.DrawRect(new Rect(Vector2.one * (first + (sample_square_size - (sample_square_size - border_size)) / 2), Vector2.one * (sample_square_size - border_size)), data.GetSkin(SkinTag.BACKGROUND));

		DrawSample(new Vector2(second, first), SkinTag.PRIMARY_WINDOW);
		DrawInset(new Vector2(second, first), SkinTag.PRIMARY_ELEMENT);

		DrawSample(new Vector2(third, first), SkinTag.SECONDARY_WINDOW);
		DrawInset(new Vector2(third, first), SkinTag.SECONDARY_ELEMENT);

		DrawSample(new Vector2(first, second), SkinTag.CONTRAST);

		DrawSample(Vector2.one * second, SkinTag.VALIDATE);
		DrawSample(new Vector2(third, second), SkinTag.DELETE);

		GUILayout.Space(container_size.y + edge_offset * 2);

		if(GUILayout.Button("Test Skin"))
		{
			TestSkin();
		}

		base.OnInspectorGUI();
	}

	void DrawSample(Vector2 offset, SkinTag tag)
	{
		EditorGUI.DrawRect(new Rect(offset.x, offset.y, sample_square_size, sample_square_size), data.GetSkin(tag));
	}

	void DrawInset(Vector2 offset, SkinTag tag)
	{
		EditorGUI.DrawRect(new Rect(offset.x + square_difference, offset.y + square_difference, inset_square_size, inset_square_size), data.GetSkin(tag));
	}

	void TestSkin()
	{
		Skinning.Init(data);

		foreach (SkinGraphic graphic in Resources.FindObjectsOfTypeAll<SkinGraphic>())
		{
			graphic.Skin();
		}
	}
}