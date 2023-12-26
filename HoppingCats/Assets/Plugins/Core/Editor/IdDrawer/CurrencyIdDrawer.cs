using UnityEditor;

namespace moonNest.editor
{
    [CustomPropertyDrawer(typeof(CurrencyId))]
    public class CurrencyIdDrawer : BasePropertyDrawer
    {
        public override void DoDrawProperty()
        {
            rect = PrefixLabel(rect, "Currency");
            DrawIntPopup(rect, "id", GameDefinitionAsset.Ins.currencies, "name", "id");
        }
    }
}