using System.Collections.Generic;
using moonNest;

public class BattlePassLevelCardDrawer : ListTabDrawer<BattlePassLevel>
{
    public List<BattlePassLevel> Levels { get; internal set; }

    readonly RewardDrawer rewardDrawer = new RewardDrawer("Reward");
    readonly RewardDrawer premiumRewardDrawer = new RewardDrawer("Premium Reward");

    public BattlePassLevelCardDrawer()
    {
        rewardDrawer.DrawOnce = true;
        premiumRewardDrawer.DrawOnce = true;
    }

    protected override void DoDrawContent(BattlePassLevel element)
    {
        element.level = Levels.IndexOf(element) + 1;
        Draw.LabelBold("Level " + element.level);
        element.requireValue = Draw.IntField("Require", element.requireValue, 150);

        Draw.Space();
        Draw.BeginHorizontal();
        {
            Draw.BeginVertical(Draw.BoxStyle);
            rewardDrawer.DoDraw(element.reward);
            Draw.EndVertical();

            Draw.BeginVertical(Draw.BoxStyle);
            premiumRewardDrawer.DoDraw(element.premiumReward);
            Draw.EndVertical();

            Draw.FlexibleSpace();
        }
        Draw.EndHorizontal();
    }

    protected override BattlePassLevel CreateNewElement() => new BattlePassLevel(0);

    protected override string GetTabLabel(BattlePassLevel element) => "" + element.level;
}