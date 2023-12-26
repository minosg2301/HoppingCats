using UnityEditor;
using UnityEngine;

namespace moonNest.editor
{
    public class ShopAssetTab : TabContent
    {
        private readonly TabContainer tabContainer;

        public ShopAssetTab()
        {
            tabContainer = new TabContainer();
            tabContainer.AddTab("Definition", new ShopDefinitionTab());
            tabContainer.AddTab("Shop", new ShopTab());
            tabContainer.FirstIndexFocused = 1;
        }

        public override void DoDraw()
        {
            Undo.RecordObject(ShopAsset.Ins, "Shop");
            tabContainer.DoDraw();
            if(GUI.changed) Draw.SetDirty(ShopAsset.Ins);
        }
    }
}