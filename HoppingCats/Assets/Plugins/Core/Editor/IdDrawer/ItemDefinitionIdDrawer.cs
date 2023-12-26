using UnityEditor;

namespace moonNest.editor
{
    [CustomPropertyDrawer(typeof(ItemDefinitionId))]
    public class ItemDefinitionIdDrawer : BasePropertyDrawer
    {
        public override void DoDrawProperty()
        {
            rect = PrefixLabel(rect, property.displayName);
            DrawIntPopup(rect, "id", ItemAsset.Ins.definitions, "name", "id");
        }
    }
}