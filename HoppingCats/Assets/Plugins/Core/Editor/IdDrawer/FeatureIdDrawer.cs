using UnityEditor;

namespace moonNest.editor
{
    [CustomPropertyDrawer(typeof(FeatureId))]
    public class FeatureIdDrawer : BasePropertyDrawer
    {
        public override void DoDrawProperty()
        {
            rect = PrefixLabel(rect, property.displayName);
            DrawIntPopup(rect, "id", GameDefinitionAsset.Ins.features, "name", "id");
        }
    }
}