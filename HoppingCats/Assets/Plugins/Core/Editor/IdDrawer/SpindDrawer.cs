using UnityEditor;
using UnityEngine;

namespace moonNest.editor
{
    [CustomPropertyDrawer(typeof(SpinId))]
    public class SpinIdDrawer : BasePropertyDrawer
    {
        public override void DoDrawProperty()
        {
            rect = PrefixLabel(rect, property.displayName);
            DrawIntPopup(rect, "id", GatchaAsset.Ins.spins, "name", "id");
        }
    }

    [CustomPropertyDrawer(typeof(SpinItemId))]
    public class SpinItemIdDrawer : BasePropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 40;

        public override void DoDrawProperty()
        {
            rect = PrefixLabel(rect, "Spin");
            int spinId = DrawIntPopup(rect, "spinId", GatchaAsset.Ins.spins, "name", "id");
            if(spinId != -1)
            {
                rect = NextLine(rect);
                rect = PrefixLabel(rect, "Spin Item");
                DrawIntPopup(rect, "id", GatchaAsset.Ins.FindSpin(spinId).spinItems, "name", "id");
            }
        }
    }
}