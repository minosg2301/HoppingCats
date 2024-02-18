using UnityEditor;
using UnityEngine;

namespace moonNest.editor
{
    [CustomPropertyDrawer(typeof(AchievementId))]
    public class AchievementIdDrawer : BasePropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 40;
        }

        public override void DoDrawProperty()
        {
            rect = PrefixLabel(rect, "Achievement Group");
            int groupId = DrawIntPopup(rect, "groupId", AchievementAsset.Ins.Groups, "name", "id");

            rect = NextLine(rect);

            rect = PrefixLabel(rect, "Achievement Id");
            DrawIntPopup(rect, "id", AchievementAsset.Ins.FindByGroup(groupId), "name", "id");
        }
    }
}