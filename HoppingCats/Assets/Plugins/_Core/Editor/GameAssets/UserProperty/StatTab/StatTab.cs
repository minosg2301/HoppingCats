using UnityEditor;
using UnityEngine;
using moonNest;

public class StatTab : TabContent
{
    readonly TabItem passiveTab, activeTab;
    readonly TabContainer tabContainer;
    readonly StatProgressGroupDetail progressGroup;
    private TabItem showTabItem;

    public StatProgressGroupDetail ProgressGroup => progressGroup;

    public StatTab(StatProgressGroupDetail group)
    {
        progressGroup = group;

        passiveTab = new TabItem("Passive", new PassiveProgressGroupTab(progressGroup));
        activeTab = new TabItem("Active", new ActiveProgressGroupTab(progressGroup));

        //tabContainer = new TabContainer();
        //tabContainer.AddTab(progressTab);
        //tabContainer.AddTab(segmentTab);
    }

    public override void OnFocused()
    {
        base.OnFocused();

        SetShow(passiveTab, progressGroup.type == StatProgressType.Passive);
        SetShow(activeTab, progressGroup.type == StatProgressType.Active);
    }

    private void SetShow(TabItem tabItem, bool show)
    {
        tabItem.show = show;
        if(show)
        {
            tabItem.Content.OnFocused();
            showTabItem = tabItem;
        }
    }

    public override void DoDraw()
    {
        Undo.RecordObject(StatProgressAsset.Ins, "UserProgress");
        showTabItem.DoDrawContent();
        if(GUI.changed) EditorUtility.SetDirty(StatProgressAsset.Ins);
    }
}