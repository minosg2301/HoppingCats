using UnityEditor;
using UnityEngine;
using moonNest;

internal class ItemUpgradeTab : TabContent
{
    private TabContainer tabContainer;

    public override void OnFocused()
    {
        tabContainer = new TabContainer();
        //foreach (var itemDefinition in GameDefinitionAsset.Ins.items)
        {
            //tabContainer.AddTab(itemDefinition.name, new ItemUpgradeConfigTab(itemDefinition));
        }
    }

    public override bool DoDrawWindow() => tabContainer.DoDrawWindow();

    public override void DoDraw()
    {
        Undo.RecordObject(ItemAsset.Ins, "Items");

        tabContainer.DoDraw();

        if (GUI.changed)
            EditorUtility.SetDirty(ItemAsset.Ins);
    }

    public class ItemUpgradeConfigTab : TabContent
    {
        private ItemDefinition itemDefinition;
        private AnimationCurve curve;

        public ItemUpgradeConfigTab(ItemDefinition itemDefinition)
        {
            this.itemDefinition = itemDefinition;
            curve = new AnimationCurve();
        }

        public override void DoDraw()
        {
            EditorGUILayout.CurveField("Curve", curve, GUILayout.Height(300));
        }
    }
}