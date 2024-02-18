using System.Collections.Generic;
using UnityEngine;

namespace moonNest
{
    public partial class IAPTriggerTab : TabContent
    {
        static List<StatDefinition> intStats = new List<StatDefinition>();
        readonly ListTriggerDrawer listTriggerDrawer = new ListTriggerDrawer();

        public override void DoDraw()
        {
            if(intStats.Count != UserPropertyAsset.Ins.properties.stats.Count)
            {
                intStats = UserPropertyAsset.Ins.properties.stats.FindAll(s => s.type == StatValueType.Int);
            }

            listTriggerDrawer.DoDraw(IAPPackageAsset.Ins.triggers);
        }

        internal class ListTriggerDrawer : ListTabDrawer<IAPTrigger>
        {
            readonly IAPPackageIdTable contentTable = new IAPPackageIdTable();
            readonly TriggerDetailTabDrawer targetDetailTabDrawer;
            readonly TableDrawer<IAPTriggerCondition> conditionTable;
            readonly TableDrawer<int> actionTable;

            public ListTriggerDrawer()
            {
                targetDetailTabDrawer = new TriggerDetailTabDrawer();

                conditionTable = new TableDrawer<IAPTriggerCondition>();
                conditionTable.AddCol("Type", 120, ele => ele.scope = Draw.Enum(ele.scope, 120));
                conditionTable.AddCol("Content", 120, ele => DrawContent(ele, 120));
                conditionTable.AddCol("Compare", 100, ele => ele.compareType = Draw.Enum(ele.compareType, 100));
                conditionTable.AddCol("Value", 80, ele => ele.value = Draw.Int(ele.value, 80), ele => ele.compareType != CompareFunc.None);
                conditionTable.elementCreator = () => new IAPTriggerCondition();
                conditionTable.drawHeader = false;
                conditionTable.drawControl = conditionTable.drawIndex = conditionTable.drawOrder = false;

                actionTable = new TableDrawer<int>();
                actionTable.AddCol("Action", 200, actionId => Draw.IntPopup(actionId, GameDefinitionAsset.Ins.actions, "name", "id", 200));
                actionTable.elementCreator = () => -1;
                actionTable.drawHeader = false;
                actionTable.drawIndex = actionTable.drawControl = actionTable.drawOrder = false;
            }

            void DrawContent(IAPTriggerCondition ele, int maxWidth)
            {
                switch(ele.scope)
                {
                    case Scope.UserStat:
                    ele.contentId = Draw.IntPopup(ele.contentId, intStats, "name", "id", maxWidth);
                    break;
                    case Scope.UserCurrency:
                    ele.contentId = Draw.IntPopup(ele.contentId, GameDefinitionAsset.Ins.currencies, "name", "id", maxWidth);
                    break;
                }
            }

            protected override IAPTrigger CreateNewElement() => new IAPTrigger("New Trigger");

            protected override string GetTabLabel(IAPTrigger trigger) => trigger.name;

            protected override void DoDrawContent(IAPTrigger trigger)
            {
                trigger.name = Draw.TextField("Name", trigger.name, 200);
                trigger.locationId = Draw.IntPopupField("Location", trigger.locationId, GameDefinitionAsset.Ins.locations, "name", "id", 120);
                trigger.frequences = Draw.IntField("Frequences", trigger.frequences, 60);
                trigger.sessionEvent = Draw.IntField("Event/Session", trigger.sessionEvent, 60);
                trigger.triggerType = Draw.EnumField("Trigger When", trigger.triggerType, 160);

                Draw.BeginChangeCheck();
                trigger.userTargeted = Draw.ToggleField("User Targeted", trigger.userTargeted, 40);
                if(Draw.EndChangeCheck())
                {
                    if(trigger.userTargeted) trigger.nonTargetedPackageIds.Clear();
                    else trigger.targetedContents.Clear();
                }

                Draw.Space();
                Draw.BeginHorizontal();
                {
                    Draw.BeginVertical();
                    Draw.LabelBoldBox("ACTIONS TO TRIGGER", Color.red, 350);
                    actionTable.DoDraw(trigger.actions);
                    Draw.EndVertical();

                    Draw.Space(60);
                    Draw.BeginVertical();
                    Draw.LabelBoldBox("CONDITIONS BY USER DATA", Color.yellow, 500);
                    conditionTable.DoDraw(trigger.conditions);
                    Draw.EndVertical();

                    Draw.FlexibleSpace();
                }
                Draw.EndHorizontal();

                if(trigger.userTargeted)
                {
                    Draw.SpaceAndLabelBoldBox("TRIGGER CONTENTS - USER TARGETED", Color.blue, 600);
                    targetDetailTabDrawer.DoDraw(trigger.targetedContents);
                }
                else
                {
                    Draw.SpaceAndLabelBoldBox("TRIGGER CONTENTS - NON USER TARGETED", Color.blue, 600);
                    contentTable.DoDraw(trigger.nonTargetedPackageIds);
                }
            }
        }

        internal class TriggerDetailTabDrawer : ListTabDrawer<IAPTriggerTargetedContent>
        {
            readonly IAPPackageIdTable contentTable = new IAPPackageIdTable();
            readonly TableDrawer<ComparableValueInt> targetedTable;

            public TriggerDetailTabDrawer()
            {
                targetedTable = new TableDrawer<ComparableValueInt>();
                targetedTable.AddCol("User Stat", 120, ele => ele.contentId = Draw.IntPopup(ele.contentId, intStats, "name", "id", 120));
                targetedTable.AddCol("Compare", 100, ele => ele.compareType = Draw.Enum(ele.compareType, 100));
                targetedTable.AddCol("Value", 80, ele => ele.value = Draw.Int(ele.value, 80), ele => ele.compareType != CompareFunc.None);
                targetedTable.elementCreator = () => new ComparableValueInt();
                targetedTable.drawHeader = false;
                targetedTable.drawControl = targetedTable.drawIndex = targetedTable.drawOrder = false;
            }

            protected override IAPTriggerTargetedContent CreateNewElement() => new IAPTriggerTargetedContent("New Detail");

            protected override string GetTabLabel(IAPTriggerTargetedContent element) => element.name;

            protected override void DoDrawContent(IAPTriggerTargetedContent element)
            {
                element.name = Draw.TextField("Name", element.name, 240);

                Draw.SpaceAndLabelBold("User Targeted Stats");
                Draw.SeparateHLine(400);
                targetedTable.DoDraw(element.userStats);

                Draw.SpaceAndLabelBold("IAP Packages");
                Draw.SeparateHLine(400);
                contentTable.DoDraw(element.contents);
            }
        }

        internal class IAPPackageIdTable : TableDrawer<IAPPackageId>
        {
            public IAPPackageIdTable()
            {
                AddCol("Group", 150, ele => { ele.groupId = Draw.IntPopup(ele.groupId, IAPPackageAsset.Ins.Groups, "name", "id", 150); });
                AddCol("Package", 300,
                    ele => { ele.id = Draw.IntPopup(ele.id, IAPPackageAsset.Ins.FindByGroup(ele.groupId), "name", "id", 300); },
                    ele => IAPPackageAsset.Ins.FindGroup(ele.groupId) != null);
                elementCreator = () => new IAPPackageId(-1);
                drawControl = drawIndex = drawOrder = false;
            }
        }
    }
}