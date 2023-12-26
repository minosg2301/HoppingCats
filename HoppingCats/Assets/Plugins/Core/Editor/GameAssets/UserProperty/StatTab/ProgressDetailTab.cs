using System.Collections.Generic;
using UnityEngine;
using moonNest;

public class ProgressDetailTab : ListTabDrawer<ProgressDetail>
{
    public int groupId;
    public StatProgressGroupDetail group;

    readonly RewardDrawer rewardDrawer = new RewardDrawer("Reward");
    readonly RewardDrawer premiumRewardDrawer = new RewardDrawer("Premium Reward");

    private ProgressDetail progress;
    private ListCardDrawer<UnlockContentDetail> unlockContentDrawer;
    internal ProgressDetail passiveProgress;

    public int MaxUnlockContent
    {
        set
        {
            unlockContentDrawer.MaxWidth = value;
        }
    }

    public ProgressDetailTab()
    {
        HeaderType = HeaderType.Vertical;
        TabWidth = 140;

        rewardDrawer.DrawOnce = true;
        rewardDrawer.CreateFirstReward = false;
        premiumRewardDrawer.DrawOnce = true;
        premiumRewardDrawer.CreateFirstReward = false;

        unlockContentDrawer = new ListCardDrawer<UnlockContentDetail>();
        unlockContentDrawer.onDrawElement = DoDrawUnlockContent;
        unlockContentDrawer.onDrawEditElement = DoDrawEditUnlockContent;
        unlockContentDrawer.elementCreator = () => new UnlockContentDetail("") { id = -1 };
        unlockContentDrawer.EditBeforeAdd = true;
        unlockContentDrawer.onElementAdded = OnUnlockContentAdded;
        unlockContentDrawer.onElementRemoved = OnUnlockContentRemoved;
    }

    protected override ProgressDetail CreateNewElement() => new ProgressDetail("") { groupId = groupId };

    protected override string GetTabLabel(ProgressDetail progressDetail) => progressDetail.name;

    protected override void DoDrawContent(ProgressDetail progressDetail)
    {
        if(progress != progressDetail)
        {
            progress = progressDetail;
        }

        progressDetail.name = Draw.TextField("Name", progressDetail.name, 180);
        progressDetail.customPrefab = Draw.ObjectField("Custom Prefab", progressDetail.customPrefab, 180);
        progressDetail.requireValue = Draw.IntField("Require", progressDetail.requireValue, 80);

        DrawUnlockContents();

        DrawRewards();
    }

    private void DrawRewards()
    {
        Draw.SpaceAndLabelBoldBox(TextPack.Rewards);
        Draw.BeginHorizontal();
        {
            Draw.BeginVertical(Draw.BoxStyle);
            rewardDrawer.DoDraw(progress.reward);
            Draw.EndVertical();

            Draw.BeginVertical(Draw.BoxStyle);
            premiumRewardDrawer.DoDraw(progress.premiumReward);
            Draw.EndVertical();
            Draw.FlexibleSpace();
        }
        Draw.EndHorizontal();
    }

    private void DrawUnlockContents()
    {
        Draw.SpaceAndLabelBoldBox(TextPack.UnlockContent);
        unlockContentDrawer.DoDraw(progress.UnlockContents);
    }

    private bool DoDrawEditUnlockContent(UnlockContentDetail unlockContentDetail)
    {
        unlockContentDetail.id = Draw.IntPopup(unlockContentDetail.id, UnlockContentAsset.Ins.unlockContents, "name", "id");
        return unlockContentDetail.id != -1;
    }

    private void DoDrawUnlockContent(UnlockContentDetail unlockContentDetail)
    {
        if(unlockContentDetail)
        {
            Draw.LabelBoldBox(unlockContentDetail.name, Color.yellow);
            Draw.FitLabelBold("Unlock by " + unlockContentDetail.UnlockConditionName);
        }
        else
        {
            Draw.LabelBoldBox("Unlock Content is null!", Color.yellow);
        }
    }

    private void OnUnlockContentRemoved(UnlockContentDetail ucd)
    {
        ucd.conditionId = -1;
        progress.unlockContentIds.Remove(ucd.id);

        // reset cached for lazy load
        progress.UnlockContents = null;
    }

    private void OnUnlockContentAdded(UnlockContentDetail ucd)
    {
        // update unlock condition for unlock content
        UnlockConditionDetail condition = GetCondition(progress);
        List<UnlockConditionDetail> conditions = UnlockContentAsset.Ins.FindConditions(group.statId);
        UnlockContentDetail unlockContent = UnlockContentAsset.Ins.FindContent(ucd.id);
        if(condition && unlockContent.conditionId != -1 && unlockContent.conditionId != condition.id)
        {
            UnlockConditionDetail oldCondition = conditions.Find(c => c.id == unlockContent.conditionId);
            if(Draw.DisplayDialog(
                "Modified Condition",
                $"{unlockContent.name} is already set unlock condition: {oldCondition.name}\n" +
                "Do you want to modified?",
                "Modified", "Cancel"))
            {
                unlockContent.conditionId = condition.id;
                unlockContent.UnlockCondition = null;
            }
        }
        else
        {
            if(!condition) condition = CreateCondition(progress);
            unlockContent.conditionId = condition.id;
            unlockContent.UnlockCondition = null;
        }

        // reset cached for lazy load
        progress.UnlockContents = null;
        progress.unlockContentIds.Add(unlockContent.id);
    }

    private UnlockConditionDetail GetCondition(ProgressDetail progress)
    {
        List<UnlockConditionDetail> conditions = UnlockContentAsset.Ins.FindConditions(group.statId);
        if(group.type == StatProgressType.Passive)
            return conditions.Find(c => c.requireValue == passiveProgress.statValue);
        else
            return conditions.Find(c => c.requireValue == progress.statValue);
    }

    private UnlockConditionDetail CreateCondition(ProgressDetail progress)
    {
        int statId = group.statId;
        UnlockConditionGroup conditionGroup = UnlockContentAsset.Ins.FindGroup(statId);
        if(!conditionGroup)
        {
            StatDefinition statDefinition = UserPropertyAsset.Ins.FindStat(statId);
            conditionGroup = new UnlockConditionGroup(statDefinition);
            UnlockContentAsset.Ins.AddGroup(conditionGroup);
        }

        string name = group.type == StatProgressType.Passive ? passiveProgress.name : progress.name;
        UnlockConditionDetail unlockCondition = new UnlockConditionDetail(name)
        {
            groupId = conditionGroup.id,
            statId = statId,
            requireValue = group.type == StatProgressType.Passive ? passiveProgress.statValue : progress.requireValue
        };
        UnlockContentAsset.Ins.Add(unlockCondition);
        return unlockCondition;
    }
}