#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public static class ShapeAssetGenerator
{
    private const string FACE_DIR = "AnimalFaces";   // Resources/AnimalFaces
    private const string OUT_DIR = "Assets/Resources/Shapes";

    [MenuItem("Tools/Match3/Generate All ShapeData %#g")] // Ctrl+Shift+G
    public static void Run()
    {
        Directory.CreateDirectory(OUT_DIR);

        foreach (AnimalType ani in System.Enum.GetValues(typeof(AnimalType)))
        {
            var face = Resources.Load<Sprite>($"{FACE_DIR}/{ani}");
            if (!face)
            {
                Debug.LogError($"Не найден спрайт {ani}.png в Resources/{FACE_DIR}");
                continue;
            }

            foreach (ShapeForm form in System.Enum.GetValues(typeof(ShapeForm)))
                foreach (BorderColor col in System.Enum.GetValues(typeof(BorderColor)))
                {
                    string path = $"{OUT_DIR}/{form}_{col}_{ani}.asset";
                    ShapeData data = AssetDatabase.LoadAssetAtPath<ShapeData>(path);
                    bool isNew = false;
                    if (!data)
                    {
                        data = ScriptableObject.CreateInstance<ShapeData>();
                        data.form = form;
                        data.color = col;
                        data.animal = ani;
                        isNew = true;
                    }

                    data.faceSprite = face;
                    if (isNew) AssetDatabase.CreateAsset(data, path);
                    EditorUtility.SetDirty(data);
                }
        }
        AssetDatabase.SaveAssets();
        Debug.Log("ShapeData generation complete.");
    }
}
#endif
