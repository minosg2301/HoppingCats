using moonNest;

public class OnlineRewardDrawer : ListCardDrawer<RewardDetail>
{
    private RewardDrawer rewardDrawer;

    public OnlineRewardDrawer()
    {
        rewardDrawer = new RewardDrawer { DrawOnce = true, DrawHeader = false, CreateFirstReward = false };
        elementCreator = () => new RewardDetail("");
    }

    protected override void DoDrawElement(RewardDetail reward)
    {
        rewardDrawer.DoDraw(reward);
    }
}