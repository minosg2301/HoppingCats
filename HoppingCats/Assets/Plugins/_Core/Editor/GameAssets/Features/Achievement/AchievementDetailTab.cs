using UnityEngine;
using moonNest;

public class AchievementDetailTab : ListTabDrawer<AchievementDetail>
{
    public AchievementGroupDetail groupDetail;
    readonly ListCardDrawer<StatRequireDetail> requireDrawer = new ListCardDrawer<StatRequireDetail>();
    readonly RewardDrawer rewardDrawer = new RewardDrawer();

    public AchievementDetailTab(string name) : base(name)
    {
        onElementAdded = achievement => AchievementAsset.Ins.Add(achievement);
        onElementRemoved = achievement => AchievementAsset.Ins.Remove(achievement);

        requireDrawer.onDrawElement = ele => DoDrawRequire(ele);
        requireDrawer.onDrawEditElement = ele => DoDrawEditRequire(ele);
        requireDrawer.elementCreator = () => new StatRequireDetail();
        requireDrawer.EditBeforeAdd = true;
        requireDrawer.CardWidth = 150;
    }

    private void DoDrawRequire(StatRequireDetail require)
    {
        var stat = UserPropertyAsset.Ins.properties.FindStat(require.statId);
        if(stat != null) Draw.LabelBoldBox($"{stat.name}", Color.yellow);
        else Draw.LabelBoldBox($"Stat Unknown");
        Draw.Space();
        require.value = Draw.LongField("Value", require.value, 80);
    }

    private bool DoDrawEditRequire(StatRequireDetail require)
    {
        require.statId = Draw.IntPopupField("Stat", require.statId, UserPropertyAsset.Ins.properties.stats, "name", "id", 120);
        require.value = Draw.LongField("Value", require.value, 80);
        return true;
    }

    protected override AchievementDetail CreateNewElement() => new AchievementDetail($"New {groupDetail.name}", groupDetail.id);

    protected override string GetTabLabel(AchievementDetail element) => element.name;

    protected override void DoDrawContent(AchievementDetail achievement)
    {
        Draw.BeginHorizontal();
        {
            Draw.BeginVertical();
            achievement.icon = Draw.Sprite(achievement.icon, true, 100, 100);
            Draw.EndVertical();

            Draw.Space(10);
            Draw.BeginVertical();
            {
                achievement.name = Draw.TextField("Name", achievement.name, 200);
                achievement.description = Draw.TextField("Description", achievement.description, 400);
                achievement.removeOnClaimed = Draw.ToggleField("Remove On Claimed", achievement.removeOnClaimed, 80);
            }
            Draw.EndVertical();

            Draw.FlexibleSpace();
        }
        Draw.EndHorizontal();

        if (GameDefinitionAsset.Ins.actions.Count > 0)
        {
            Draw.Space();
            DoDrawEditRequire(achievement.require);
        }

        Draw.Space();
        rewardDrawer.DoDraw(achievement.reward);
    }
}