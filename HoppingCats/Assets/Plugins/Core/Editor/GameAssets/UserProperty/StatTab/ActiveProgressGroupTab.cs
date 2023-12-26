using System.Collections.Generic;
using moonNest;

public partial class ActiveProgressGroupTab : TabContent
{
    private List<StatDefinition> filterStats;
    readonly ProgressDetailTab progressTab;
    readonly StatProgressGroupDetail group;
    readonly string statName;
    private string linkStatName;
    private string linkMsg;

    public ActiveProgressGroupTab(StatProgressGroupDetail statGroup)
    {
        group = statGroup;
        statName = statGroup.name;

        progressTab = new ProgressDetailTab { HeaderType = HeaderType.Vertical };
        progressTab.onElementAdded = OnProgressAdded;
        progressTab.groupId = statGroup.id;
        progressTab.group = group;
    }

    public override void OnFocused()
    {
        base.OnFocused();

        filterStats = UserPropertyAsset.Ins.properties.stats.FindAll(stat => stat.type == StatValueType.Int && stat.id != group.statId);
        StatDefinition linkedStat = filterStats.Find(stat => stat.id == group.linkedStatId);
        linkStatName = linkedStat ? linkedStat.name : "";

        if(group.linkedStatId != -1)
        {
            linkMsg = $"This progress is linked with '" + linkStatName.ToUpper() + "'";
        }
    }

    public override void DoDraw()
    {
        if(group.linkedStatId != -1)
        {
            Draw.LabelBoldBox(linkMsg);
            return;
        }

        group.stepValue = Draw.IntField(TextPack.StepValue, group.stepValue, 60);
        DrawPremiumReward();
        progressTab.DoDraw(group.progresses);
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

    private void OnProgressAdded(ProgressDetail progressDetail)
    {
        List<ProgressDetail> progresses = group.progresses;
        progressDetail.requireValue = progresses.Count > 1 ? progresses[progresses.Count - 2].requireValue + group.stepValue : 0;
        progressDetail.name = statName + " " + progressDetail.requireValue;
    }
}