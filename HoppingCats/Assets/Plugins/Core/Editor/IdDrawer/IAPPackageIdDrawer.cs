using UnityEditor;
using UnityEngine;

namespace moonNest.editor
{
    [CustomPropertyDrawer(typeof(IAPPackageId))]
    public class IAPPackageIdDrawer : BasePropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 40;
        }

        public override void DoDrawProperty()
        {
            rect = PrefixLabel(rect, "IAP Group");
            int groupId = DrawIntPopup(rect, "groupId", IAPPackageAsset.Ins.Groups, "name", "id");
            rect = NextLine(rect);
            rect = PrefixLabel(rect, property.displayName);
            DrawIntPopup(rect, "id", IAPPackageAsset.Ins.FindByGroup(groupId), "productId", "id");
        }
    }

    [CustomPropertyDrawer(typeof(IAPGroupId))]
    public class IAPGroupIdDrawer : BasePropertyDrawer
    {
        public override void DoDrawProperty()
        {
            rect = PrefixLabel(rect, property.displayName);
            DrawIntPopup(rect, "id", IAPPackageAsset.Ins.Groups, "name", "id");
        }
    }
}