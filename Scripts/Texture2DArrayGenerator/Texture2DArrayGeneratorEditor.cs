#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Texture2DArrayGeneratorEditor : EditorWindow {
    #region Filed

    public string fileName = "tex2darray.asset";

    public TextureFormat format = TextureFormat.ARGB32;

    public Texture2D[] textures;

    public DefaultAsset directory { get; set; }

    #endregion Field

    #region Method

    [MenuItem("Custom/Texture2DArrayGenerator")]
    static void Init() {
        EditorWindow.GetWindow<Texture2DArrayGeneratorEditor>(typeof(Texture2DArrayGenerator).Name);
    }

    protected void OnGUI() {
        GUIStyle marginStyle = GUI.skin.label;
        marginStyle.wordWrap = true;
        marginStyle.margin = new RectOffset(5, 5, 5, 5);

        using (new GUILayout.HorizontalScope()) {
            EditorGUILayout.LabelField("Texture Directory", GUILayout.Width(180));
            directory = (DefaultAsset)EditorGUILayout.ObjectField(directory, typeof(DefaultAsset), false, GUILayout.Width(200));
            GUILayout.ExpandWidth(true);
        }

        this.fileName = EditorGUILayout.TextField("File Name", this.fileName);

        this.format = (TextureFormat)EditorGUILayout.EnumPopup("Format", this.format);

        SerializedObject serializedObject = new SerializedObject(this);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("textures"), true);
        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Load")) {
            if (directory != null) {
                var directories = new List<string>();
                string selectionPath = AssetDatabase.GetAssetPath(directory);
                directories.Add(selectionPath);
                var guids = AssetDatabase.FindAssets("t:Texture", directories.ToArray());
                textures = guids.Select(g => (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(g), typeof(Texture2D))).ToArray();
            }
        }
        if (GUILayout.Button("Generate")) {
            string path = AssetCreationHelper.CreateAssetInCurrentDirectory
                          (Texture2DArrayGenerator.Generate(this.textures, this.format), this.fileName);

            ShowNotification(new GUIContent("SUCCESS : " + path));
        }

    }

    #endregion Method
}

#endif // UNITY_EDITOR