using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpriteSheet))]
public class SpriteSheetEditor : Editor
{
    SpriteSheet sheet;

    public override void OnInspectorGUI()
    {
        sheet=target as SpriteSheet;

        GUILayout.Space(10);

        sheet.imported_texture=EditorGUILayout.ObjectField("Sprite sheet texture",sheet.imported_texture,typeof(Texture2D),false) as Texture2D;
        GUILayout.Space(10);

        if(GUILayout.Button("Extract sprites"))
        {
            string texture_path=AssetDatabase.GetAssetPath(sheet.imported_texture);
            Object[] extracted_objects=AssetDatabase.LoadAllAssetsAtPath(texture_path);
            Sprite[] extracted_sprites=new Sprite[extracted_objects.Length-1];
            
            for(int i=1;i<extracted_objects.Length;i++)
            {
                extracted_sprites[i-1]=extracted_objects[i] as Sprite;
            }

            sheet.bank=extracted_sprites;
        }

        GUILayout.Space(10);

        base.OnInspectorGUI();
    }
}