using UnityEditor;
using UnityEngine;

namespace moonNest.editor
{
    [CustomPropertyDrawer(typeof(ItemId))]
    public class ItemIdDrawer : BasePropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 40;

        public override void DoDrawProperty()
        {
            rect = PrefixLabel(rect, "Item Type");
            int definitionId = DrawIntPopup(rect, "definitionId", ItemAsset.Ins.definitions, "name", "id");
            rect = NextLine(rect);
            rect = PrefixLabel(rect, "Item Id");
            DrawIntPopup(rect, "id", ItemAsset.Ins.FindByDefinition(definitionId), "name", "id");
        }
    }
}