using UnityEditor;
using UnityEngine;

namespace moonNest.editor
{
    [CustomPropertyDrawer(typeof(LinkUrl))]
    public class LinkUrlDrawer : BasePropertyDrawer
    {
        public override void DoDrawProperty()
        {
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("url"), new GUIContent(property.displayName));
        }
    }
}