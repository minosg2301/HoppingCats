using UnityEditor;
using UnityEngine;
using moonNest;

public class LuckyBoxTab : TabContent
{
    readonly LuckyBoxesDetailTab luckyBoxDetailTab;

    const string gatchaLocked = "Gatcha is locked by '{0}'";
    const string gatchaLockedByNoCondition = "Gatcha is locked by no condition!!";

    private FeatureConfig _featureConfig;
    public FeatureConfig FeatureConfig
    {
        get
        {
            if(!_featureConfig) _featureConfig = GameDefinitionAsset.Ins.FindFeature("Gatcha");
            return _featureConfig;
        }
    }

    public LuckyBoxTab()
    {
        luckyBoxDetailTab = new LuckyBoxesDetailTab();
    }

    public override void DoDraw()
    {
        if(FeatureConfig.locked)
        {
            UnlockConditionDetail unlockCondition = UnlockContentAsset.Ins.FindCondition(FeatureConfig.unlockConditionId);
            if(unlockCondition) Draw.LabelBoldBox(string.Format(gatchaLocked, unlockCondition.name), Color.blue);
            else Draw.LabelBoldBox(string.Format(gatchaLockedByNoCondition, unlockCondition.name), Color.red);
        }

        Undo.RecordObject(GatchaAsset.Ins, "LuckyBox");
        GatchaAsset.Ins.luckyBoxDuration = Draw.IntField("Duration (Day)", GatchaAsset.Ins.luckyBoxDuration, 120);
        luckyBoxDetailTab.DoDraw(GatchaAsset.Ins.luckyBoxes);
        if(GUI.changed) EditorUtility.SetDirty(GatchaAsset.Ins);
    }
}