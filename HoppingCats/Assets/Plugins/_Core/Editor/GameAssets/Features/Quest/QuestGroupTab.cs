using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using moonNest;

public class QuestGroupTab : ListTabDrawer<QuestGroupDetail>
{
    private readonly QuestDetailTab questDetailTab = new QuestDetailTab();
    private readonly QuestGroupRewardTab questGroupRewardTab;
    private readonly QuestTab questTab;
    private readonly TabContainer tabContainer = new TabContainer();
    private readonly TableDrawer<int> unlockQuestTable;
    private readonly TableDrawer<QuestDetail> questDetailTable;
    private readonly ColDrawer<QuestDetail> alwayShowCol;
    private readonly ColDrawer<QuestDetail> slotCol;
    private readonly ColDrawer<QuestDetail> pointCol;

    QuestGroupDetail currentQuestGroupDetail;

    private List<QuestGroupDetail> nonActiveGroups;
    private List<int> slots = new List<int>();

    public QuestGroupTab()
    {
        unlockQuestTable = new TableDrawer<int>();
        unlockQuestTable.AddCol("Quest Group", 120, ele => Draw.IntPopup(ele, nonActiveGroups, "name", "id", 120));
        unlockQuestTable.drawIndex = unlockQuestTable.drawControl = unlockQuestTable.drawOrder = false;
        unlockQuestTable.drawHeader = false;
        unlockQuestTable.elementCreator = () => -1;

        onElementAdded = group => QuestAsset.Ins.AddGroup(group);
        onElementRemoved = group => QuestAsset.Ins.RemoveGroup(group.id);
        onElementCloned = OnQuestGroupCloned;

        alwayShowCol = new ColDrawer<QuestDetail>("Always Show", 120, ele => ele.showAlways = Draw.Toggle(ele.showAlways, 120));
        slotCol = new ColDrawer<QuestDetail>("Slot", 60, ele => ele.slot = Draw.IntPopup(ele.slot, slots, 60));
        pointCol = new ColDrawer<QuestDetail>("Point", 60, ele => ele.point = Draw.Int(ele.point, 60));
        questDetailTable = new TableDrawer<QuestDetail>();
        questDetailTable.AddCol("Name", 120, ele => ele.name = Draw.Text(ele.name, 120));
        questDetailTable.AddCol("Icon", 120, ele => ele.icon = Draw.Sprite(ele.icon, 120));
        questDetailTable.AddCol("Display Name", 120, ele => ele.displayName = Draw.Text(ele.displayName, 120));
        questDetailTable.AddCol("Server Id", 120, ele => ele.serverId = Draw.Int(ele.serverId, 120));
        questDetailTable.AddCol("Description", 250, ele => ele.description = Draw.Text(ele.description, 250));
        questDetailTable.AddCol(pointCol);
        questDetailTable.AddCol(slotCol);
        questDetailTable.AddCol(alwayShowCol);
        questDetailTable.onOrderChanged += OnQuestOrderChanged;
        questDetailTable.onElementDeleted += OnQuestDeleted;

        questGroupRewardTab = new QuestGroupRewardTab();
        questTab = new QuestTab(questDetailTable);
        tabContainer = new TabContainer();
        tabContainer.AddTab("Quests", questTab);
        tabContainer.AddTab("Rewards", questGroupRewardTab);

        onSwapPerformed += OnGroupSwapped;
    }

    private void OnQuestDeleted(QuestDetail quest)
    {
        QuestAsset instance = QuestAsset.Ins;
        instance.Remove(quest);

        if (instance.layerEnabled && currentQuestGroupDetail.layerEnabled)
        {
            List<LayerDetail> layers = LayerAsset.Ins.FindByGroup(instance.layerGroupId);
            layers.ForEach(layer =>
            {
                QuestGroupLayer groupLayer = layer.questGroups.Find(g => g.groupId == currentQuestGroupDetail.id);
                groupLayer.quests.Remove(q => q.questId == quest.id);
            });
        }
    }

    private void OnQuestOrderChanged(QuestDetail arg1, QuestDetail arg2)
    {
        QuestAsset.Ins.Editor_DoSwap(arg1, arg2);
    }

    private void OnGroupSwapped(QuestGroupDetail group1, QuestGroupDetail group2)
    {
        QuestAsset.Ins.Editor_DoSwapGroup(group1, group2);
    }

    public override bool DoDrawWindow()
    {
        return base.DoDrawWindow() || tabContainer.DoDrawWindow() || questDetailTab.DoDrawWindow();
    }

    private void OnQuestGroupCloned(QuestGroupDetail newGroup, QuestGroupDetail originGroup)
    {
        var quests = QuestAsset.Ins.FindByGroup(originGroup.id);
        foreach (var quest in quests)
        {
            var newQuest = quest.Clone() as QuestDetail;
            newQuest.groupId = newGroup.id;
            QuestAsset.Ins.Add(newQuest);
        }
    }

    protected override QuestGroupDetail CreateNewElement() => new QuestGroupDetail("Quest Group");

    protected override string GetTabLabel(QuestGroupDetail element) => element.name;

    protected override void DoDrawContent(QuestGroupDetail group)
    {
        currentQuestGroupDetail = group;

        if (slots.Count != group.refreshConfig.maxQuest)
        {
            slots = new List<int>();
            for (int i = 0; i < group.refreshConfig.maxQuest; i++) { slots.Add(i + 1); }
        }

        alwayShowCol.enabled = group.refreshConfig.Enabled && group.type == QuestGroupType.List;
        slotCol.enabled = group.refreshConfig.Enabled && group.type == QuestGroupType.Slot;
        pointCol.enabled = group.pointEnabled;

        nonActiveGroups = QuestAsset.Ins.Groups.FindAll(g => g.id != group.id && !g.activeOnLoad);
        Draw.BeginHorizontal();
        group.name = Draw.TextField(TextPack.Name, group.name, 200);
        Draw.FlexibleSpace();
        Draw.FitLabelBold("ID: " + group.id);
        Draw.EndHorizontal();

        Draw.Space();
        Draw.BeginHorizontal();
        {
            Draw.BeginVertical();
            {
                group.serverId = Draw.IntField("Server Id", group.serverId, 120);
                group.activeOnLoad = Draw.ToggleField(TextPack.ActiveOnLoad, group.activeOnLoad, 120);
                group.removeOnCompleted = Draw.ToggleField(TextPack.RemoveOnCompleted, group.removeOnCompleted, 120);
                DrawSilent(group);
                DrawLayerEnabled(group);
                group.drawMethod = Draw.EnumField(TextPack.DrawMethod, group.drawMethod, 120);
            }
            Draw.EndVertical();

            Draw.Space(60);
            Draw.BeginVertical();
            {
                group.refreshConfig.type = Draw.EnumField(TextPack.Refresh, group.refreshConfig.type, 120);
                Draw.BeginDisabledGroup(!group.refreshConfig.Enabled);
                {
                    group.refreshConfig.maxQuest = Draw.IntField(TextPack.MaxQuest, group.refreshConfig.maxQuest, 120);
                    Draw.BeginDisabledGroup(group.refreshConfig.type != QuestRefeshType.OnTime);
                    {
                        var periodType = group.refreshConfig.period.type;
                        Draw.BeginChangeCheck();
                        Draw.PeriodField(TextPack.Period, group.refreshConfig.period, 120);
                        if (Draw.EndChangeCheck())
                        {
                            if (group.refreshConfig.period.type != PeriodType.Day
                                && group.refreshConfig.period.type != PeriodType.Week
                                && group.refreshConfig.period.type != PeriodType.Month)
                            {
                                Draw.DisplayDialog("Error", $"Period Type {group.refreshConfig.period.type} isn't supported!", "OK");
                                group.refreshConfig.period.type = periodType;
                            }
                        }
                    }
                    group.type = Draw.EnumField(TextPack.Type, group.type, 80);
                    Draw.EndDisabledGroup();
                }
                Draw.EndDisabledGroup();
            }
            Draw.EndVertical();

            Draw.Space(60);
            Draw.BeginVertical();
            {
                DrawPointEnabled(group);
            }
            Draw.EndVertical();

            Draw.Space(60);
            Draw.BeginVertical();
            {
                Draw.LabelBold("Active On Completed");
                unlockQuestTable.DoDraw(group.activeOnCompleteds);
            }
            Draw.EndVertical();

            Draw.FlexibleSpace();
        }
        Draw.EndHorizontal();

        if (group.pointEnabled)
        {
            questGroupRewardTab.groupDetail = group;
            questTab.groupDetail = group;
            tabContainer.DoDraw();
        }
        else
        {
            if (group.drawMethod == DrawMethod.Tab)
            {
                questDetailTab.groupDetail = group;
                questDetailTab.DoDraw(QuestAsset.Ins.FindByGroup(group.id).ToList());
            }
            else
            {
                questDetailTable.DoDraw(QuestAsset.Ins.FindByGroup(group.id).ToList());
            }
        }
    }

    private static void DrawSilent(QuestGroupDetail group)
    {
        Draw.BeginChangeCheck();
        group.silent = Draw.ToggleField("Silent", group.silent, 40f);
        if (Draw.EndChangeCheck())
        {
            QuestAsset.Ins.FindByGroup(group.id).ForEach(q => q.silent = group.silent);
        }
    }

    private static void DrawLayerEnabled(QuestGroupDetail group)
    {
        Draw.BeginChangeCheck();
        Draw.BeginDisabledGroup(!QuestAsset.Ins.layerEnabled);
        group.layerEnabled = Draw.ToggleField(TextPack.Layer, group.layerEnabled, 120);
        if (group.layerEnabled)
        {
            LayerGroup layerGroup = LayerAsset.Ins.FindGroup(QuestAsset.Ins.layerGroupId);
            if (layerGroup)
                Draw.LabelBoldBox($"Rewards is overrided by Layer '{layerGroup.name}'", Color.blue);
            else
                Draw.LabelBoldBox("Select which Layer Group in LAYER TAB", Color.red);
        }
        Draw.EndDisabledGroup();
        if (Draw.EndChangeCheck())
        {
            LayerGroup layerGroup = LayerAsset.Ins.FindGroup(QuestAsset.Ins.layerGroupId);
            if (layerGroup == null) return;

            List<LayerDetail> layers = LayerAsset.Ins.FindByGroup(QuestAsset.Ins.layerGroupId);
            if (group.layerEnabled)
            {
                List<QuestDetail> quests = QuestAsset.Ins.FindByGroup(group.id);

                layerGroup.questGroupIds.Add(group.id);
                layers.ForEach(layer => layer.questGroups.Add(new QuestGroupLayer(group, quests)));
            }
            else
            {
                layerGroup.questGroupIds.Remove(group.id);
                layers.ForEach(layer => layer.questGroups.Remove(c => c.groupId == group.id));
            }
        }
    }

    private static void DrawPointEnabled(QuestGroupDetail group)
    {
        Draw.BeginChangeCheck();
        Draw.LabelBold("Point");
        group.pointEnabled = Draw.ToggleField("Enabled", group.pointEnabled, 100);
        if (Draw.EndChangeCheck()) QuestAsset.Ins.FindByGroup(group.id).ForEach(questDetail => questDetail.pointEnabled = group.pointEnabled);
        if (group.pointEnabled)
        {
            group.pointAutoClaimed = Draw.ToggleField("Auto Claimed", group.pointAutoClaimed, 100);
            group.pointRewardOnly = Draw.ToggleField("Only Point Reward", group.pointRewardOnly, 100);
            group.pointIcon = Draw.SpriteField("Icon", group.pointIcon, true, 70, 70);
        }
    }

    public class QuestTab : TabContent
    {
        public QuestGroupDetail groupDetail;

        private readonly QuestDetailTab questDetailTab = new QuestDetailTab();
        private readonly TableDrawer<QuestDetail> questDetailTable;

        public QuestTab(TableDrawer<QuestDetail> questDetailTable)
        {
            this.questDetailTable = questDetailTable;
        }

        public override void DoDraw()
        {
            questDetailTab.groupDetail = groupDetail;
            if (groupDetail.drawMethod == DrawMethod.Tab) questDetailTab.DoDraw(QuestAsset.Ins.FindByGroup(groupDetail.id).ToList());
            else questDetailTable.DoDraw(QuestAsset.Ins.FindByGroup(groupDetail.id).ToList());
        }

        public override bool DoDrawWindow()
        {
            return questDetailTab.DoDrawWindow();
        }

    }

    public class QuestGroupRewardTab : TabContent
    {
        public QuestGroupDetail groupDetail;

        private ListCardDrawer<PointReward> questRewardCardDrawer;
        private RewardDrawer rewardDrawer = new RewardDrawer("Reward");

        public QuestGroupRewardTab()
        {
            questRewardCardDrawer = new ListCardDrawer<PointReward>();
            questRewardCardDrawer.onDrawElement = ele => DrawQuestReward(ele);
            questRewardCardDrawer.elementCreator = () => new PointReward();

            rewardDrawer.DrawOnce = true;
        }

        public override void DoDraw()
        {
            questRewardCardDrawer.DoDraw(groupDetail.pointRewards);
        }

        private void DrawQuestReward(PointReward ele)
        {
            ele.point = Draw.IntField(TextPack.Point, ele.point, 100);
            Draw.Space();
            rewardDrawer.DoDraw(ele.reward);
        }
    }
}