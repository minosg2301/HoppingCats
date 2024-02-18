using UnityEditor;
using UnityEngine;
using moonNest;

public class UnlockContentTab : TabContent
{
    private TabContainer tabContainer;

    public UnlockContentTab()
    {
        tabContainer = new TabContainer();
        tabContainer.AddTab("Unlock Content", new UnlockContentDetailTab());
        tabContainer.AddTab("Unlock Condition", new UnlockConditionTab());
    }

    public override void DoDraw()
    {
        Undo.RecordObject(UnlockContentAsset.Ins, "UnlockContent");
        tabContainer.DoDraw();
        if(GUI.changed) EditorUtility.SetDirty(UnlockContentAsset.Ins);
    }
}