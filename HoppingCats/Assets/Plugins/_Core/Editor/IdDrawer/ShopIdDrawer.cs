using UnityEditor;

namespace moonNest.editor
{
    [CustomPropertyDrawer(typeof(ShopId))]
    public class ShopIdDrawer : BasePropertyDrawer
    {
        public override void DoDrawProperty()
        {
            rect = PrefixLabel(rect, property.displayName);
            DrawIntPopup(rect, "id", ShopAsset.Ins.Shops, "name", "id");
        }
    }
}