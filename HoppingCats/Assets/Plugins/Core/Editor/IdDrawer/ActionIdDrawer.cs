using UnityEditor;

namespace moonNest.editor
{
    [CustomPropertyDrawer(typeof(ActionId))]
    public class ActionIdDrawer : BasePropertyDrawer
    {
        public override void DoDrawProperty()
        {
            rect = PrefixLabel(rect, property.displayName);
            DrawIntPopup(rect, "id", GameDefinitionAsset.Ins.actions, "name", "id");
        }
    }
}