using System.Linq;
using moonNest;

public class AchievementGroupTab : ListTabDrawer<AchievementGroupDetail>
{
    AchievementDetailTab questDetailTab;

    public AchievementGroupTab()
    {
        questDetailTab = new AchievementDetailTab("Achievement Group");
        onElementAdded = group => AchievementAsset.Ins.AddGroup(group);
    }

    protected override AchievementGroupDetail CreateNewElement() => new AchievementGroupDetail("Achievement Group");

    protected override string GetTabLabel(AchievementGroupDetail element) => element.name;

    protected override void DoDrawContent(AchievementGroupDetail group)
    {
        group.name = Draw.TextField("Name", group.name, 200);
        group.removeOnClaimed = Draw.ToggleField("Remove On Claimed", group.removeOnClaimed, 80);
        questDetailTab.HeaderType = Draw.EnumField("Header Type", questDetailTab.HeaderType, 80);
        questDetailTab.groupDetail = group;
        questDetailTab.DoDraw(AchievementAsset.Ins.FindByGroup(group.id));
    }
}