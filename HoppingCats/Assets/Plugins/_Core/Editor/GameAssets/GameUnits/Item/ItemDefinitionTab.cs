using UnityEditor;
using UnityEngine;

namespace moonNest.editor
{
    public partial class ItemDefinitionTab : TabContent
    {
        private readonly ItemDefinitionDrawer itemTab = new ItemDefinitionDrawer();

        public override void DoDraw()
        {
            itemTab.DoDraw(ItemAsset.Ins.definitions);
            if(Draw.Button("Generate class", Color.magenta, Color.white, 150))
            {
                GenerateClass(ItemAsset.Ins.definitions);
                AssetDatabase.Refresh();
            }
        }
    }
}