using UnityEditor;
using UnityEngine;

namespace moonNest.editor
{
    public class ItemAssetTab : TabContent
    {
        readonly TabContainer tabContainer;

        public ItemAssetTab()
        {
            tabContainer = new TabContainer();
            tabContainer.AddTab("Definition", new ItemDefinitionTab());
            tabContainer.AddTab("Detail", new ItemDetailTab());
            tabContainer.AddTab("Random Setting", new ItemRandomTab());
            tabContainer.FirstIndexFocused = 1;
        }

        public override void OnFocused()
        {
            base.OnFocused();
            tabContainer.SelectedItem?.Content?.OnFocused();
        }

        public override void DoDraw()
        {
            Undo.RecordObject(ItemAsset.Ins, "Items");
            tabContainer.DoDraw();
            if(GUI.changed) EditorUtility.SetDirty(ItemAsset.Ins);
        }

        public override bool DoDrawWindow() => tabContainer.DoDrawWindow();
    }
}