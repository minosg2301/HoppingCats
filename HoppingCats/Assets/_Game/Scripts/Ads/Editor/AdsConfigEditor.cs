using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AdsConfig))]
public class AdsConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open Editor")) AdsConfigEditorWindow.OpenWindow();
    }
}