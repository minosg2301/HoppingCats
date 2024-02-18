using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using moonNest;

public abstract class BasePropertyDrawer : PropertyDrawer
{
    public const int kLineH = 18;
    public const int kPadding = 2;
    public const int kIndentPerLevel = 15;

    public Rect beginRect;
    public Rect rect;
    public SerializedProperty property;
    public GUIContent label;

    public virtual int TotalLine => 1;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        this.property = property;
        int totalLine = TotalLine;
        return totalLine * kLineH + (totalLine - 1) * kPadding;
    }

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(rect, label, property);

        beginRect = rect;
        this.rect = rect;
        this.rect.height = kLineH;
        this.property = property;
        this.label = label;

        DoDrawProperty();

        EditorGUI.EndProperty();
    }

    public abstract void DoDrawProperty();

    protected Rect PrefixLabel(Rect rect, string label)
        => EditorGUI.PrefixLabel(rect, new GUIContent(label));

    protected int DrawIntPopup<T>(Rect rect, string propertyName, List<T> list, string labelField, string valueField)
    {
        var p = property.FindPropertyRelative(propertyName);
        var intValue = p.intValue;
        intValue = Draw.IntPopup(rect, intValue, list, labelField, valueField);
        if (p.serializedObject.targetObjects.Count() == 1) p.intValue = intValue;
        return intValue;
    }

    protected string DrawStringPopup<T>(Rect rect, string propertyName, List<T> list, string valueField)
    {
        var p = property.FindPropertyRelative(propertyName);
        var value = p.stringValue;
        value = Draw.StringPopup(rect, value, list, valueField);
        if(p.serializedObject.targetObjects.Count() == 1) p.stringValue = value;
        return value;
    }

    protected string DrawStringPopup(Rect rect, string propertyName, List<string> list)
    {
        var p = property.FindPropertyRelative(propertyName);
        var value = p.stringValue;
        value = Draw.StringPopup(rect, value, list);
        if (p.serializedObject.targetObjects.Count() == 1) p.stringValue = value;
        return value;
    }

    protected Rect UpdateIndent(Rect rect)
    {
        rect.x = beginRect.x + EditorGUI.indentLevel * kIndentPerLevel;
        return rect;
    }

    protected Rect NextLine(Rect rect)
    {
        rect.width = beginRect.width;
        rect.x = beginRect.x;
        rect.y += kLineH + kPadding;
        return rect;
    }

    protected void NewLine()
    {
        rect.width = beginRect.width;
        rect.x = beginRect.x;
        rect.y += kLineH + kPadding;
    }

    protected void PropertyField(string propertyName, string label = null)
    {
        rect = EditorGUI.PrefixLabel(rect, new GUIContent(label ?? propertyName.ToTitleCase()));
        EditorGUI.PropertyField(rect, property.FindPropertyRelative(propertyName));
    }

    protected int GetProperty(string propertyName) => property.FindPropertyRelative(propertyName).intValue;
}

