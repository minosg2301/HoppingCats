using System;
using UnityEditor;
using UnityEngine;

public class BaseEnumDrawer<T> : PropertyDrawer where T : struct
{
    static readonly string[] enumNames = GetEnumNames();

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(rect, label, property);
        rect = EditorGUI.PrefixLabel(rect, label);
        property.enumValueIndex = EditorGUI.Popup(rect, property.enumValueIndex, enumNames);
        EditorGUI.EndProperty();
    }

    static string[] GetEnumNames()
    {
        string[] result = Enum.GetNames(typeof(T));
        for(int i = 0; i < result.Length; i++)
            result[i] = $"{result[i][0]}/{result[i]}";
        return result;
    }
}
