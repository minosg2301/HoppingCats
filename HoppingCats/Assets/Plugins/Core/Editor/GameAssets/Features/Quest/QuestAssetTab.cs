using UnityEditor;
using UnityEngine;
using moonNest;

public class QuestAssetTab : TabContent
{
    readonly ListTabDrawer<QuestGroupDetail> groupTab = new QuestGroupTab();

    public override void DoDraw()
    {
        Undo.RecordObject(QuestAsset.Ins, "Quest");
        groupTab.DoDraw(QuestAsset.Ins.Groups);
        if(GUI.changed) Draw.SetDirty(QuestAsset.Ins);

    }

    public override bool DoDrawWindow()
    {
        return groupTab.DoDrawWindow();
    }
}