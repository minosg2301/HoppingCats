using UnityEditor;

namespace moonNest.editor
{
    [CustomPropertyDrawer(typeof(NavigationId))]
    public class NavigationIdDrawer : BasePropertyDrawer
    {
        public override void DoDrawProperty()
        {
            rect = PrefixLabel(rect, property.displayName);
            DrawIntPopup(rect, "id", NavigationAsset.Ins.navigationDatas, "name", "id");
        }
    }

    [CustomPropertyDrawer(typeof(NavigationPathId))]
    public class NavigationDefinitionIdDrawer : BasePropertyDrawer
    {
        public override void DoDrawProperty()
        {
            rect = PrefixLabel(rect, property.displayName);
            DrawIntPopup(rect, "id", GameDefinitionAsset.Ins.events, "name", "id");
        }
    }

}
