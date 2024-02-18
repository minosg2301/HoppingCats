using UnityEditor;
using UnityEngine;

namespace moonNest.editor
{
    [CustomPropertyDrawer(typeof(QuestId))]
    public class QuestIdDrawer : BasePropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 40;
        }

        public override void DoDrawProperty()
        {
            rect = PrefixLabel(rect, "Quest Group");
            int groupId = DrawIntPopup(rect, "groupId", QuestAsset.Ins.Groups, "name", "id");

            rect = NextLine(rect);

            rect = PrefixLabel(rect, "Quest Id");
            DrawIntPopup(rect, "id", QuestAsset.Ins.FindByGroup(groupId), "name", "id");
        }
    }
}