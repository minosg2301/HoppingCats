using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using moonNest;

public class UserPropertyAssetTab : TabContent
{
    private TabContainer tabContainer;
    UserPropertyDefinitionTab userPropertyDefinitionTab;
    private List<StatTab> statsTabs = new List<StatTab>();

    public UserPropertyAssetTab()
    {
        userPropertyDefinitionTab = new UserPropertyDefinitionTab();

        tabContainer = new TabContainer();
        tabContainer.AddTab(new TabItem("Definition", userPropertyDefinitionTab));

        InitTab();

        StatProgressAsset.Ins.onGroupChanged = InitTab;
    }

    private void InitTab()
    {
        List<StatProgressGroupDetail> statGroups = StatProgressAsset.Ins.groups;
        statsTabs.ToList().ForEach(statTab =>
        {
            if(statGroups.Find(group => group == statTab.ProgressGroup) == null)
            {
                statsTabs.Remove(statTab);
                tabContainer.RemoveTab(statTab);
            }
        });

        List<StatDefinition> stats = UserPropertyAsset.Ins.properties.propertyDefinitions
            .FindAll(propDef => propDef.statDefinition && propDef.statDefinition.type == StatValueType.Int && propDef.statDefinition.progress)
            .Map(p => p.statDefinition);
        stats.ForEach(stat =>
        {
            StatProgressGroupDetail group = StatProgressAsset.Ins.groups.Find(group => group.statId == stat.id);
            if(group && statsTabs.Find(tab => tab.ProgressGroup == group) == null)
            {
                StatTab statTab = new StatTab(group);
                statsTabs.Add(statTab);
                tabContainer.AddTab(stat.name, statTab);
            }
        });
    }

    public override void DoDraw()
    {
        Undo.RecordObject(UserPropertyAsset.Ins, "UserProperty");
        tabContainer.DoDraw();
        if(GUI.changed) EditorUtility.SetDirty(UserPropertyAsset.Ins);
    }
}