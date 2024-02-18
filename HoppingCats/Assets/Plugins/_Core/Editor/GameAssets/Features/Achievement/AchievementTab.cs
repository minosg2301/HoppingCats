using UnityEditor;
using UnityEngine;
using moonNest;

public class AchievementTab : TabContent
{
    const string achievementLocked = "Achievement is locked by '{0}'";
    const string achievementLockedByNoCondition = "Achievement is locked by no condition!!";

    ListTabDrawer<AchievementGroupDetail> groupTab = new AchievementGroupTab();

    private FeatureConfig _featureConfig;
    public FeatureConfig FeatureConfig
    {
        get
        {
            if(!_featureConfig) _featureConfig = GameDefinitionAsset.Ins.FindFeature("Achievement");
            return _featureConfig;
        }
    }

    public override void DoDraw()
    {
        if(FeatureConfig.locked)
        {
            UnlockConditionDetail unlockCondition = UnlockContentAsset.Ins.FindCondition(FeatureConfig.unlockConditionId);
            if(unlockCondition) Draw.LabelBoldBox(string.Format(achievementLocked, unlockCondition.name), Color.blue);
            else Draw.LabelBoldBox(string.Format(achievementLockedByNoCondition, unlockCondition.name), Color.red);
        }

        Undo.RecordObject(AchievementAsset.Ins, "Achievements");
        groupTab.DoDraw(AchievementAsset.Ins.Groups);
        if(GUI.changed) Draw.SetDirty(AchievementAsset.Ins);
    }
}