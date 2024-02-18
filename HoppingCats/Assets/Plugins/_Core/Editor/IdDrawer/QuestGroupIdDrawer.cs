using UnityEditor;
using UnityEngine;

namespace moonNest.editor
{
    [CustomPropertyDrawer(typeof(QuestGroupId))]
    public class QuestGroupIdDrawer : BasePropertyDrawer
    {
        public override void DoDrawProperty()
        {
            rect = PrefixLabel(rect, property.displayName);
            DrawIntPopup(rect, "groupId", QuestAsset.Ins.Groups, "name", "id");
        }
    }
}