using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using moonNest;

public partial class OnlineRewardTab : TabContent
{
    const string OnlineRewards = "OnlineRewards";
    const string overrideByLayer = "Rewards is overrided by Layer '{0}'";

    const string onlineRewardLocked = "Online Reward is locked by '{0}'";
    const string onlineRewardLockedByNoCondition = "Online Reward is locked by no condition!!";

    private readonly OnlineRewardTabDrawer tabDrawer;
    private readonly OnlineRewardAsset instance;
    private readonly FeatureConfig featureConfig;

    public OnlineRewardTab()
    {
        featureConfig = GameDefinitionAsset.Ins.FindFeature("OnlineReward");

        instance = OnlineRewardAsset.Ins;

        tabDrawer = new OnlineRewardTabDrawer();
        tabDrawer.onElementAdded = OnOnlineRewardAdded;
        tabDrawer.onElementRemoved = OnOnlineRewardRemoved;
    }

    private void OnOnlineRewardRemoved(OnlineRewardDetail removedOnlineReward)
    {
        if(instance.layerEnabled)
        {
            List<LayerDetail> layers = LayerAsset.Ins.FindByGroup(instance.layerGroupId);
            layers.ForEach(layer => layer.onlineRewards.Remove(l => l.onlineRewardId == removedOnlineReward.id));
        }
    }

    private void OnOnlineRewardAdded(OnlineRewardDetail newOnlineReward)
    {
        if(instance.layerEnabled)
        {
            List<LayerDetail> layers = LayerAsset.Ins.FindByGroup(instance.layerGroupId);
            layers.ForEach(layer => layer.onlineRewards.Add(new OnlineRewardLayer(newOnlineReward)));
        }
    }

    public override void DoDraw()
    {
        if(featureConfig.locked)
        {
            UnlockConditionDetail unlockCondition = UnlockContentAsset.Ins.FindCondition(featureConfig.unlockConditionId);
            if(unlockCondition) Draw.LabelBoldBox(string.Format(onlineRewardLocked, unlockCondition.name), Color.blue);
            else Draw.LabelBoldBox(string.Format(onlineRewardLockedByNoCondition, unlockCondition.name), Color.red);
        }

        if(instance.layerEnabled)
            Draw.LabelBoldBox(string.Format(overrideByLayer, instance.LayerName.ToUpper()), Color.blue);

        Undo.RecordObject(instance, OnlineRewards);
        tabDrawer.DoDraw(instance.onlineRewards);
        if(GUI.changed) Draw.SetDirty(instance);
    }
}

