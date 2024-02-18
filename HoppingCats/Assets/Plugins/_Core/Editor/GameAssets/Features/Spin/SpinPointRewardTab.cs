using moonNest;

internal class SpinPointRewardTab : TabContent
{
    public SpinDetail spinDetail;

    private ListCardDrawer<PointReward> poinRewardDrawer;
    private readonly RewardDrawer rewardDrawer = new RewardDrawer("Reward");

    public SpinPointRewardTab()
    {
        poinRewardDrawer = new ListCardDrawer<PointReward>();
        poinRewardDrawer.onDrawElement = ele => DrawQuestReward(ele);
        poinRewardDrawer.elementCreator = () => new PointReward();

        rewardDrawer.DrawOnce = true;
    }

    public override void DoDraw()
    {
        poinRewardDrawer.DoDraw(spinDetail.pointRewards);
    }

    private void DrawQuestReward(PointReward ele)
    {
        ele.point = Draw.IntField(TextPack.Point, ele.point, 100);
        Draw.Space();
        rewardDrawer.DoDraw(ele.reward);
    }

}