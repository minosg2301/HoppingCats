using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using moonNest;

public class QuestDetailTab : ListTabDrawer<QuestDetail>
{
    const string EmptyAction = "To add require for quest, create ACTION first!";
    public QuestGroupDetail groupDetail;

    private List<int> slots = new List<int>();
    //private readonly ListCardDrawer<ActionRequire> requireDrawer;
    private readonly RewardDrawer rewardDrawer = new RewardDrawer();

    private readonly TableDrawer<int> unlockQuestTable;

    private readonly ListCardDrawer<ActionDetail> requiresTable;


    private List<QuestDetail> nonActiveGroups;

    public QuestDetailTab()
    {
        onElementAdded = OnQuestAdded;
        onElementRemoved = OnQuestRemoved;
        onSwapPerformed = OnQuestSwapped;
        //onElementCloned = OnQuestClone;


        unlockQuestTable = new TableDrawer<int>();
        unlockQuestTable.AddCol("Quest Group", 120, ele => Draw.IntPopup(ele, nonActiveGroups, "name", "id", 120));
        unlockQuestTable.drawIndex = unlockQuestTable.drawControl = unlockQuestTable.drawOrder = false;
        unlockQuestTable.drawHeader = false;
        unlockQuestTable.elementCreator = () => -1;

        requiresTable = new ListCardDrawer<ActionDetail>();
        requiresTable.onDrawElement = ele => DoDrawActionDetail(ele);
        requiresTable.elementCreator = () => new ActionDetail();
    }

    private void OnQuestSwapped(QuestDetail q1, QuestDetail q2)
    {
        QuestAsset.Ins.Editor_DoSwap(q1, q2);
    }

    private void OnQuestRemoved(QuestDetail quest)
    {
        QuestAsset instance = QuestAsset.Ins;
        instance.Remove(quest);

        if (instance.layerEnabled && groupDetail.layerEnabled)
        {
            List<LayerDetail> layers = LayerAsset.Ins.FindByGroup(instance.layerGroupId);
            layers.ForEach(layer =>
            {
                QuestGroupLayer groupLayer = layer.questGroups.Find(g => g.groupId == groupDetail.id);
                groupLayer.quests.Remove(q => q.questId == quest.id);
            });
        }
    }

    private void OnQuestAdded(QuestDetail quest)
    {
        ActionRequire actionRequire = new ActionRequire() { action = new ActionDetail() };
        quest.require = actionRequire;
        ActionRequires actionRequires = new ActionRequires() { actions = new List<ActionDetail>() };
        quest.requires = actionRequires;

        QuestAsset instance = QuestAsset.Ins;
        instance.Add(quest);

        if (instance.layerEnabled && groupDetail.layerEnabled)
        {
            List<LayerDetail> layers = LayerAsset.Ins.FindByGroup(instance.layerGroupId);
            layers.ForEach(layer =>
            {
                QuestGroupLayer groupLayer = layer.questGroups.Find(g => g.groupId == groupDetail.id);
                groupLayer.quests.Add(new QuestLayer(quest));
            });
        }
    }

    private void DoDrawActionDetail(ActionDetail action)
    {
        if (action.id == -1)
        {
            DoDrawEditDetail(action);
            return;
        }

        Draw.BeginHorizontal();
        Draw.LabelBold($"{action.Definition.name}");
        if (Draw.FitButton("-", Color.green))
        {
            action.id = -1;
            action.Definition = null;
        }
        Draw.FlexibleSpace();
        Draw.EndHorizontal();

        for (int i = 0; i < action.actionParams.Length; i++)
        {
            var actioDetail = action;
            if (actioDetail.Definition.paramTypes[i] == ActionParamType.Enum)
                DrawActionParam(actioDetail.actionParams[i], actioDetail.Definition.enumTypes[i], 120);
            else DrawActionParam(actioDetail.actionParams[i], 120);
        }
    }

    private bool DoDrawEditDetail(ActionDetail action)
    {
        var actionDefinitionId = Draw.IntPopupField(TextPack.Action, action.id, GameDefinitionAsset.Ins.actions, "name", "id", 250);
        if (action.id != actionDefinitionId)
        {
            action.id = actionDefinitionId;
            action.actionParams = new ActionParam[action.Definition.paramTypes.Length];
            for (int i = 0; i < action.actionParams.Length; i++)
            {
                var param = new ActionParam { type = action.Definition.paramTypes[i], content = -1 };
                action.actionParams[i] = param;
            }
            return true;
        }
        else
        {
            action.id = actionDefinitionId;
        }
        return false;
    }

    private void DoDrawRequire(ActionRequire require)
    {
        if (require.action.id == -1)
        {
            DoDrawEditRequire(require);
            return;
        }

        Draw.BeginHorizontal();
        Draw.LabelBold($"{require.action.Definition.name}");
        if (Draw.FitButton("-", Color.green))
        {
            require.action.id = -1;
            require.action.Definition = null;
        }
        Draw.FlexibleSpace();
        Draw.EndHorizontal();

        for (int i = 0; i < require.action.actionParams.Length; i++)
        {
            var action = require.action;
            if (action.Definition != null && action.Definition.paramTypes[i] == ActionParamType.Enum)
                DrawActionParam(action.actionParams[i], action.Definition.enumTypes[i], 120);
            else DrawActionParam(action.actionParams[i], 120);
        }

        require.count = Draw.IntField(TextPack.Count, require.count, 120);
    }

    private bool DoDrawEditRequire(ActionRequire require)
    {
        ActionDetail action = require.action;
        var actionDefinitionId = Draw.IntPopupField(TextPack.Action, action.id, GameDefinitionAsset.Ins.actions, "name", "id", 250);
        if (action.id != actionDefinitionId)
        {
            action.id = actionDefinitionId;
            action.actionParams = new ActionParam[action.Definition.paramTypes.Length];
            for (int i = 0; i < action.actionParams.Length; i++)
            {
                var param = new ActionParam { type = action.Definition.paramTypes[i], content = -1 };
                action.actionParams[i] = param;
            }
            return true;
        }
        else
        {
            action.id = actionDefinitionId;
        }
        return false;
    }

    private void DoDrawRequire(StatRequireDetail require)
    {
        if (require.statId == -1)
        {
            require.statId = Draw.IntPopupField("Stat", require.statId, UserPropertyAsset.Ins.properties.stats, "name", "id", 120);
            return;
        }

        Draw.BeginHorizontal();
        Draw.LabelBold($"{require.Definition.name}");
        if (Draw.FitButton("-", Color.green))
        {
            require.statId = -1;
            require.Definition = null;
        }
        Draw.FlexibleSpace();
        Draw.EndHorizontal();
        require.value = Draw.IntField("Value", require.value, 120);
    }

    void DrawActionParam(ActionParam param, int enumType, float width)
    {
        var enumTypes = GameDefinitionAsset.Ins.FindEnum(enumType);
        param.enumType = Draw.StringPopupField("Type", param.enumType, GameDefinitionAsset.Ins.FindEnum(enumType).stringList, width);
        param.content = enumTypes.stringList.IndexOf(param.enumType);
    }

    void DrawActionParam(ActionParam param, float width)
    {
        switch (param.type)
        {
            case ActionParamType.Item: param.content = Draw.IntPopupField(TextPack.Item, param.content, ItemAsset.Ins.items, "name", "id", width); break;
            case ActionParamType.Chest: param.content = Draw.IntPopupField(TextPack.Chest, param.content, ChestAsset.Ins.chests, "name", "id", width); break;
            case ActionParamType.Quest: param.content = Draw.IntPopupField(TextPack.Quest, param.content, QuestAsset.Ins.Quests, "name", "id", width); break;
            case ActionParamType.Currency: param.content = Draw.IntPopupField(TextPack.Currency, param.content, GameDefinitionAsset.Ins.currencies, "name", "id", width); break;
            case ActionParamType.IntValue: param.content = Draw.IntField("Value", param.content, width); break;
            case ActionParamType.QuestGroup: param.content = Draw.IntPopupField("Group Id", param.content, QuestAsset.Ins.Groups, "name", "id", width); break;
        }
    }


    protected override QuestDetail CreateNewElement() => new QuestDetail($"New {groupDetail.name}", groupDetail.id) { silent = groupDetail.silent };

    protected override string GetTabLabel(QuestDetail element) => element.name;

    protected override void DoDrawContent(QuestDetail quest)
    {
        if (slots.Count != groupDetail.refreshConfig.maxQuest)
        {
            slots = new List<int>();
            for (int i = 0; i < groupDetail.refreshConfig.maxQuest; i++) { slots.Add(i + 1); }
        }


        nonActiveGroups = QuestAsset.Ins.Quests.FindAll(q => q.id != quest.id && q.groupId == groupDetail.id && !q.activeOnLoad);

        Draw.BeginHorizontal();
        {
            Draw.BeginVertical();
            quest.icon = Draw.Sprite(quest.icon, false, 90, 90);
            Draw.EndVertical();
            Draw.Space(10);
            Draw.BeginVertical();
            {
                quest.name = Draw.TextField(TextPack.Name, quest.name, 200);
                quest.displayName = Draw.TextField(TextPack.DisplayName, quest.displayName, 200);
                quest.description = Draw.TextField(TextPack.Description, quest.description, 400);
                //quest.serverId = Draw.IntField("Server Id", quest.serverId, 80);
                quest.activeOnLoad = Draw.ToggleField(TextPack.ActiveOnLoad, quest.activeOnLoad, 80);
                quest.removeOnClaimed = Draw.ToggleField(TextPack.RemoveOnClaimed, quest.removeOnClaimed, 80);
                quest.useMultiActions = Draw.ToggleField("Use Multi Actions", quest.useMultiActions, 60);
                if (groupDetail.pointEnabled)
                {
                    quest.point = Draw.IntField(TextPack.PointReward, quest.point, 100);
                }
                if (groupDetail.refreshConfig.Enabled)
                {
                    if (groupDetail.type == QuestGroupType.List) quest.showAlways = Draw.ToggleField(TextPack.ShowAlways, quest.showAlways, 80);
                    else quest.slot = Draw.IntPopupField(TextPack.Slot, quest.slot, slots, 80);
                }
            }
            Draw.EndVertical();
            Draw.FlexibleSpace();

            Draw.BeginVertical();
            {
                Draw.LabelBold("Active On Completed");
                unlockQuestTable.DoDraw(quest.activeOnCompleteds);
            }
            Draw.EndVertical();

            Draw.BeginVertical();
            {
                Draw.LabelBold("Navigation");
                quest.isNavigation = Draw.Toggle(quest.isNavigation, 60);
                if (quest.isNavigation)
                {
                    quest.navigationId = Draw.IntPopup(quest.navigationId, NavigationAsset.Ins.navigationDatas, 160);
                }
            }
            Draw.EndVertical();
        }
        Draw.EndHorizontal();

        Draw.Space();
        Draw.LabelBoldBox("Require", Color.yellow, 254);
        if (GameDefinitionAsset.Ins.actions.Count > 0)
        {
            if (quest.useMultiActions)
            {
                if (quest.requires == null) quest.requires = new ActionRequires();

                quest.requires.count = Draw.IntField(TextPack.Count, quest.requires.count, 120);
                requiresTable.DoDraw(quest.requires.actions);
            }
            else
            {
                if (quest.statRequire.statId == -1)
                {
                    DoDrawRequire(quest.require);
                }

                if (quest.require.action.id == -1)
                {
                    DoDrawRequire(quest.statRequire);
                }
            }
        }
        else
        {
            Draw.HelpBox(EmptyAction, MessageType.Error);
        }

        if (!groupDetail.pointEnabled || !groupDetail.pointRewardOnly)
        {
            Draw.Space();
            rewardDrawer.DoDraw(quest.reward);
        }
    }
}