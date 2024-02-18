using UnityEditor;

namespace moonNest.editor
{
    [CustomPropertyDrawer(typeof(AchievementGroupId))]
    public class AchievementGroupIdDrawer : BasePropertyDrawer
    {
        public override void DoDrawProperty()
        {
            rect = PrefixLabel(rect, "Achievement Group");
            DrawIntPopup(rect, "groupId", AchievementAsset.Ins.Groups, "name", "id");
        }
    }
}