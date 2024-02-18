using UnityEditor;

namespace moonNest.editor
{
    [CustomPropertyDrawer(typeof(LocationId))]
    public class LocationIdDrawer : BasePropertyDrawer
    {
        public override void DoDrawProperty()
        {
            rect = PrefixLabel(rect, property.displayName);
            DrawIntPopup(rect, "id", GameDefinitionAsset.Ins.locations, "name", "id");
        }
    }
}