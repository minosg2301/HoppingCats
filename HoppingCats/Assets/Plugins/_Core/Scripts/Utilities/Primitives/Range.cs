using System;
using UnityEngine;
using ERandom = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace moonNest
{
    [Serializable]
    public struct Range
    {
        public float min;
        public float max;
        public Range(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        public override string ToString() => string.Format("({0}, {1})", min, max);

        public float Length => max - min;

        public float Random() => ERandom.Range(min, max);

        public int RandomInt() => Mathf.RoundToInt(min == max ? min : ERandom.Range(min, max));

        public bool InRange(float value) => min <= value && value <= max;

        public static Range By(float min, float max) => new Range(min, max);

        public void Validate()
        {
            if(min > max)
            {
                float v = min;
                min = max;
                max = v;
            }
        }

        public float IntersectValue(Range range)
        {
            float Aa = Math.Max(0, min - range.min);
            float Ba = max - range.min;
            float Bb = Math.Max(0, max - range.max);
            return Math.Max(0, Ba - Bb - Aa);
        }

        public float Lerp(float t) => Mathf.Lerp(min, max, t);

        public float InverLerp(float value)
        {
            return Mathf.InverseLerp(min, max, value);
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(Range))]
    public class RangeDrawer : PropertyDrawer
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
            Rect maxRect = new Rect(position.x + 80, position.y, 40, position.height);

            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            EditorGUI.LabelField(minRect, "Min");
            minRect.x += 30;
            EditorGUI.PropertyField(minRect, property.FindPropertyRelative("min"), GUIContent.none);
            EditorGUI.LabelField(maxRect, "Max");
            maxRect.x += 30;
            EditorGUI.PropertyField(maxRect, property.FindPropertyRelative("max"), GUIContent.none);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
#endif
}