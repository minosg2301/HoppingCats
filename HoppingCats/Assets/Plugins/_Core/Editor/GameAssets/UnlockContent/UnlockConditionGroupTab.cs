using System;
using System.Collections.Generic;
using UnityEngine;
using moonNest;

public class UnlockConditionGroupTab : ListTabDrawer<UnlockConditionGroup>
{
    private ColDrawer<UnlockConditionDetail> requireCol;
    private TableDrawer<UnlockConditionDetail> table;
    private UnlockConditionGroup unlockConditionGroup;

    public UnlockConditionGroupTab()
    {
        DrawAddButton = false;

        requireCol = new ColDrawer<UnlockConditionDetail>("Require", 80, ele => ele.requireValue = Draw.Int(ele.requireValue, 80));
        table = new TableDrawer<UnlockConditionDetail>("Unlock Condition");
        table.AddCol("Name", 120, ele => ele.name = Draw.Text(ele.name, 120));
        table.AddCol("Display Name", 180, ele => ele.displayName = Draw.Text(ele.displayName, 180));
        table.AddCol(requireCol);
        table.elementCreator = () => new UnlockConditionDetail("New Condition") { groupId = unlockConditionGroup.id, statId = unlockConditionGroup.statId };
        table.onElementAdded = OnUnlockConditionAdded;
        table.onElementDeleted = OnUnlockConditionDeleted;
        table.onOrderChanged = OnOrderChanged;
    }

    private void OnOrderChanged(UnlockConditionDetail condition1, UnlockConditionDetail condition2)
    {
        UnlockContentAsset.Ins.Editor_DoSwap(condition1, condition2);
    }

    private void OnUnlockConditionDeleted(UnlockConditionDetail unlockCondition)
    {
        UnlockContentAsset.Ins.Remove(unlockCondition);
    }

    private void OnUnlockConditionAdded(UnlockConditionDetail unlockCondition)
    {
        UnlockContentAsset.Ins.Add(unlockCondition);
    }

    protected override UnlockConditionGroup CreateNewElement() => null;

    protected override string GetTabLabel(UnlockConditionGroup element) => element.name;

    protected override void DoDrawContent(UnlockConditionGroup group)
    {
        unlockConditionGroup = group;
        if(group.StatDefinition.progress)
        {
            if(Draw.FitButton($"Sync data with [{group.name}] Progresses", Color.cyan))
            {
                DoSyncDataWithProgress(group);
            }
            Draw.Space();
        }

        requireCol.editable = !group.StatDefinition.progress;
        table.DoDraw(UnlockContentAsset.Ins.FindByGroup(group.id));
    }

    private void DoSyncDataWithProgress(UnlockConditionGroup group)
    {
        bool haveChanged = false;
        var unlockConditions = UnlockContentAsset.Ins.FindByGroup(group.id);
        var progressGroup = StatProgressAsset.Ins.FindGroupByStat(group.statId);
        progressGroup.progresses.ForEach(progress =>
        {
            int value = progressGroup.type == StatProgressType.Passive ? progress.statValue : progress.requireValue;
            UnlockConditionDetail uc = unlockConditions.Find(unlockCondition => unlockCondition.requireValue == value);
            if(!uc)
            {
                uc = new UnlockConditionDetail(progress.name)
                {
                    groupId = group.id,
                    requireValue = value,
                    statId = progressGroup.statId,
                    displayName = progress.displayName
                };
                UnlockContentAsset.Ins.Add(uc);
                haveChanged = true;
            }
            else
            {
                uc.displayName = progress.displayName;
                uc.name = progress.name;
            }
        });

        if(haveChanged)
        {
            unlockConditions = UnlockContentAsset.Ins.FindByGroup(group.id);
            unlockConditions.SortAsc(c => c.requireValue);
            UnlockContentAsset.Ins.UpdateList(group.id, unlockConditions);
        }
    }
}