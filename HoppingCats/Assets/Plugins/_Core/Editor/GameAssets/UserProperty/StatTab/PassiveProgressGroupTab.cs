using System;
using System.Collections.Generic;
using UnityEngine;
using moonNest;

public class PassiveProgressGroupTab : TabContent
{
    readonly ColDrawer<ProgressDetail> downgradeCol;
    readonly ColDrawer<ProgressDetail> valueCol;
    readonly ColDrawer<ProgressDetail> requireCol;
    readonly ExpandableColDrawer<ProgressDetail> progressesCol;
    readonly TableDrawer<ProgressDetail> progressTable;
    readonly ProgressDetailTab progressTab;
    readonly StatProgressGroupDetail group;
    readonly int groupId;
    readonly string statName;

    string progressStatName;
    List<StatDefinition> filterStats;
    StatProgressGroupDetail linkedGroup;
    List<ProgressDetail> linkedProgresses;
    ProgressDetail selectedProgress;

    public PassiveProgressGroupTab(StatProgressGroupDetail statGroup)
    {
        group = statGroup;
        statName = statGroup.name;
        groupId = statGroup.id;

        valueCol = new ColDrawer<ProgressDetail>(statName, 60, ele => ele.statValue = Draw.Int(ele.statValue, 60), false);
        requireCol = new ColDrawer<ProgressDetail>("Require", 80, ele => ele.requireValue = Draw.Int(ele.requireValue, 80));
        downgradeCol = new ColDrawer<ProgressDetail>("Backable", 70, ele => ele.downgradable = Draw.Toggle(ele.downgradable, 70));
        progressesCol = new ExpandableColDrawer<ProgressDetail>("Progresses", "Edit", 80, ele => DoDrawProgresses(ele));

        progressTable = new TableDrawer<ProgressDetail>("User Progress");
        progressTable.AddCol("Name", 100, ele => ele.name = Draw.Text(ele.name, 100));
        progressTable.AddCol("Display Name", 180, ele => DrawDisplayName(ele));
        progressTable.AddCol("Icon", 150, ele => ele.icon = Draw.Sprite(ele.icon, 150));
        progressTable.AddCol(valueCol);
        progressTable.AddCol(requireCol);
        progressTable.AddCol(downgradeCol);
        progressTable.AddExpandCol(progressesCol);
        progressTable.elementCreator = () =>
        {
            int count = progressTable.FullList.Count + 1;
            return new ProgressDetail(statName + " " + count) { groupId = groupId };
        };
        progressTable.onElementAdded = OnProgressTableAdded;
        progressTable.onElementDeleted = OnProgressTableDeleted;
        progressTable.drawIndex = false;

        progressTab = new ProgressDetailTab { HeaderType = HeaderType.Vertical };
        progressTab.onElementAdded = OnProgressTabAdded;
        progressTab.onElementRemoved = OnProgressTabRemoved;
        progressTab.groupId = statGroup.id;
        progressTab.group = group;
    }

    public override void OnFocused()
    {
        base.OnFocused();

        filterStats = UserPropertyAsset.Ins.properties.stats.FindAll(stat => stat.type == StatValueType.Int && stat.id != group.statId);
        StatDefinition stat = filterStats.Find(stat => group.progressId == stat.id);
        progressStatName = stat ? stat.name : "-";
        linkedGroup = StatProgressAsset.Ins.FindGroupByStat(group.progressId);
        progressTab.DrawAddButton = linkedGroup;
        progressTab.MaxUnlockContent = Screen.width - 600;
    }

    public override void DoDraw()
    {
        Draw.BeginHorizontal();
        {
            Draw.BeginVertical();
            DrawBuffered();
            DrawProgressId();
            DrawBufferedProgress();
            DrawLinked();
            DrawAccumulate();
            DrawPremiumReward();
            Draw.EndVertical();

            Draw.Space(60);
            Draw.BeginVertical();
            Draw.LabelBoldBox("Editor");
            group.stepValue = Draw.IntField(TextPack.StepValue, group.stepValue, 60);
            if(linkedGroup) linkedGroup.stepValue = Draw.IntField("Step Progress Value", linkedGroup.stepValue, 60);
            Draw.EndVertical();

            Draw.FlexibleSpace();
        }
        Draw.EndHorizontal();

        Draw.Space();
        if(group.progressId != -1) progressTable.DoDraw(group.progresses);
        else Draw.LabelBoldBox("UpdatedBy must be selected!");
    }

    private void DrawDisplayName(ProgressDetail ele)
    {
        Draw.BeginChangeCheck();
        ele.displayName = Draw.Text(ele.displayName, 180);
        if(Draw.EndChangeCheck())
        {
            if(linkedGroup)
            {
                var linkedProgresses = group.FindLinkedProgresses(ele.statValue);
                if(linkedProgresses.Count > 0)
                {
                    linkedProgresses[0].displayName = ele.displayName;
                }
            }
        }
    }

    private void DrawPremiumReward()
    {
        Draw.BeginChangeCheck();
        group.premiumReward = Draw.ToggleField(TextPack.PremiumReward, group.premiumReward, 80);
        if(Draw.EndChangeCheck())
        {
            if(!group.premiumReward) group.premiumStatId = 1;
        }
        if(group.premiumReward)
            group.premiumStatId = Draw.IntPopupField(TextPack.PremiumStat, group.premiumStatId, filterStats, "name", "id", 120);
    }

    private void DrawLinked()
    {
        if(group.progressId != -1)
        {
            Draw.BeginChangeCheck();
            group.linkedProgress = Draw.ToggleField(TextPack.LinkProgress, group.linkedProgress, 60);
            if(Draw.EndChangeCheck())
            {
                StatDefinition progressStatDef = UserPropertyAsset.Ins.FindStat(group.progressId);
                linkedGroup = StatProgressAsset.Ins.FindGroupByStat(group.progressId);
                if(group.linkedProgress)
                {
                    if(linkedGroup)
                    {
                        linkedGroup.linkedStatId = group.statId;
                        linkedGroup.bufferedStatId = group.bufferedProgressId;
                    }
                    else
                    {
                        progressStatDef.progress = true;
                        progressStatDef.progressType = StatProgressType.Active;

                        linkedGroup = StatProgressAsset.Ins.CreateGroup(progressStatDef);
                        linkedGroup.linkedStatId = group.statId;
                        linkedGroup.bufferedStatId = group.bufferedProgressId;
                        linkedGroup.progresses = group.progresses
                            .Map(progress => new ProgressDetail($"{linkedGroup.name} " + progress.requireValue)
                            {
                                groupId = linkedGroup.id,
                                requireValue = progress.requireValue
                            });
                    }
                }
                else if(linkedGroup)
                {
                    linkedGroup.linkedStatId = -1;
                    linkedGroup.bufferedStatId = -1;
                    linkedGroup = null;
                }

                progressTab.DrawAddButton = group.linkedProgress;
            }
        }
    }

    private void DrawAccumulate()
    {
        if(group.progressId != -1)
        {
            group.accumulatable = Draw.ToggleField(TextPack.Accumulatable, group.accumulatable, 60);
            Draw.BeginVertical(Draw.BoxStyle, GUILayout.MaxWidth(400));
            Draw.BeginHorizontal();
            if(!group.accumulatable)
            {
                Draw.FitLabelBold($"[{progressStatName}]");
                Draw.FitLabel(" will be reset to 0 when ");
                Draw.FitLabelBold($"[{statName}]");
                Draw.FitLabel(" updated");
            }
            else
            {
                Draw.FitLabelBold($"[{progressStatName}]");
                Draw.FitLabel(" will keep value when ");
                Draw.FitLabelBold($"[{statName}]");
                Draw.FitLabel(" updated");
            }
            Draw.EndHorizontal();
            Draw.EndVertical();
        }
    }

    private void DrawBuffered()
    {
        if(group.type == StatProgressType.Active)
        {
            group.bufferedStatId = Draw.IntPopupField(TextPack.Buffered, group.bufferedStatId, filterStats, "name", "id", 120);
        }
    }

    private void DrawProgressId()
    {
        Draw.BeginChangeCheck();
        group.progressId = Draw.IntPopupField(TextPack.ProgressBy, group.progressId, filterStats, "name", "id", 120);
        if(Draw.EndChangeCheck())
        {
            StatDefinition stat = filterStats.Find(stat => group.progressId == stat.id);
            progressStatName = stat ? stat.name : "-";
            requireCol.name = group.progressId != -1 ? progressStatName : TextPack.Require;
            if(group.progressId == -1 && linkedGroup)
            {
                linkedGroup.linkedStatId = -1;
                linkedGroup.bufferedStatId = -1;
                linkedGroup = null;
            }
        }
    }

    private void DrawBufferedProgress()
    {
        Draw.BeginChangeCheck();
        group.bufferedProgressId = Draw.IntPopupField(TextPack.ProgressBuffered, group.bufferedProgressId, filterStats, "name", "id", 120);
        if(Draw.EndChangeCheck())
        {
            if(linkedGroup) linkedGroup.bufferedStatId = group.bufferedProgressId;
        }
    }

    private void OnProgressTableAdded(ProgressDetail progress)
    {
        List<ProgressDetail> progresses = group.progresses;
        progress.statValue = progresses.Count;
        progress.requireValue = progresses.Count > 1 ? progresses[progresses.Count - 2].requireValue + group.stepValue : 0;

        if(linkedGroup)
        {
            linkedGroup.progresses.Add(new ProgressDetail($"{linkedGroup.name} " + progress.requireValue)
            {
                groupId = linkedGroup.id,
                requireValue = progress.requireValue
            });
        }
    }

    private void OnProgressTableDeleted(ProgressDetail progress)
    {
        if(linkedGroup)
        {
            linkedGroup.progresses.RemoveAll(linkProgress => linkProgress.requireValue == progress.requireValue);
        }
    }

    private void OnProgressTabAdded(ProgressDetail progress)
    {
        if(linkedGroup)
        {
            ProgressDetail last = linkedProgresses[linkedProgresses.Count - 2];
            progress.requireValue = last.requireValue + linkedGroup.stepValue;
            progress.name = $"{linkedGroup.name} " + progress.requireValue;
            progress.groupId = linkedGroup.id;
            linkedGroup.progresses.Add(progress);
            linkedGroup.progresses.SortAsc(progress => progress.requireValue);
            linkedProgresses = group.FindLinkedProgresses(selectedProgress.statValue);
        }
    }

    private void OnProgressTabRemoved(ProgressDetail progress)
    {
        if(linkedGroup)
        {
            linkedGroup.progresses.Remove(progress);
        }
    }

    private void DoDrawProgresses(ProgressDetail progress)
    {
        if(group.linkedProgress && linkedGroup)
        {
            if(selectedProgress != progress)
            {
                selectedProgress = progress;
                linkedProgresses = group.FindLinkedProgresses(progress.statValue);
            }

            progressTab.passiveProgress = progress;
            progressTab.DoDraw(linkedProgresses);
        }
        else
        {
            progressTab.DoDraw(group.progresses);
        }
    }
}