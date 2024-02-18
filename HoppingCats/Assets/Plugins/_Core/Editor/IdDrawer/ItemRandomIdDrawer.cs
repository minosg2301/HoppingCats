using UnityEditor;

namespace moonNest.editor
{
    [CustomPropertyDrawer(typeof(ItemRandomId))]
    public class ItemRandomIdDrawer : BasePropertyDrawer
    {
        public override void DoDrawProperty()
        {
            rect = PrefixLabel(rect, property.displayName);
            DrawIntPopup(rect, "id", ItemAsset.Ins.itemRandoms, "name", "id");
        }
    }
}