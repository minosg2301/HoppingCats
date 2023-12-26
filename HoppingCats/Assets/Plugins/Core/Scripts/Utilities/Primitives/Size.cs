using System;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace moonNest
{
    [Serializable]
    public struct Size
    {
        public float width;
        public float height;

        public Size(float width, float height)
        {
            this.width = width;
            this.height = height;
        }

        public static Size operator *(Size size, float value)
        {
            return new Size(size.width * value, size.height * value);
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(Size))]
    public class SizeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            Rect minRect = new Rect(position.x, position.y, 40, position.height);
            Rect maxRect = new Rect(position.x + 90, position.y, 40, position.height);

            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            EditorGUI.LabelField(minRect, "Width");
            minRect.x += 40;
            EditorGUI.PropertyField(minRect, property.FindPropertyRelative("width"), GUIContent.none);

            EditorGUI.LabelField(maxRect, "Height");
            maxRect.x += 45;
            EditorGUI.PropertyField(maxRect, property.FindPropertyRelative("height"), GUIContent.none);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
#endif
}