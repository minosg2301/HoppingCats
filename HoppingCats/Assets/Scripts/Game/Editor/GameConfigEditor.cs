using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameConfig))]
public class GameConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open Editor")) GameConfigEditorWindow.OpenWindow();
    }
}