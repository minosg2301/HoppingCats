using UnityEditor;

namespace moonNest
{
    [CustomPropertyDrawer(typeof(ChestId))]
    public class ChestIdDrawer : BasePropertyDrawer
    {
        public override void DoDrawProperty()
        {
            rect = PrefixLabel(rect, "Chest Id");
            DrawIntPopup(rect, "id", ChestAsset.Ins.chests, "name", "id");
        }
    }
}