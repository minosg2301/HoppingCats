using UnityEditor;
using UnityEngine;
using moonNest;

public class ChestTab : TabContent
{
    const string Chest = "Chest";
    readonly ListTabDrawer<ChestDetail> tabDrawer = new ChestDetailTabDrawer();

    public override void DoDraw()
    {
        Undo.RecordObject(ChestAsset.Ins, Chest);
        tabDrawer.DoDraw(ChestAsset.Ins.chests);
        if(GUI.changed) Draw.SetDirty(ChestAsset.Ins);
    }

    public override bool DoDrawWindow()
    {
        return tabDrawer.DoDrawWindow();
    }
}