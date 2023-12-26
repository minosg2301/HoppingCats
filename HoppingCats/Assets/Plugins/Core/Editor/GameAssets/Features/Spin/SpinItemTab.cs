using System.Collections.Generic;
using System.Linq;
using moonNest;

public class SpinItemTab : TabContent
{
    private readonly TableDrawer<SpinItemConfig> spinItemTable;
    private readonly SpinItemTabDrawer spinItemTab;

    public SpinDetail spinDetail;

    public SpinItemTab()
	{
        spinItemTable = new TableDrawer<SpinItemConfig>();
        spinItemTable.AddCol("Name", 100, ele => ele.name = Draw.Text(ele.name, 100));
        spinItemTable.AddCol("Icon", 100, ele => ele.icon = Draw.Sprite(ele.icon, 100));
        spinItemTable.AddCol("Weight", 400, ele => ele.weight = Draw.IntSlider(ele.weight, 0, 9999, 400));
        spinItemTable.AddCol("Probability", 80, ele => Draw.Label(ele.probability.ToString("00.0000%"), 80));
        spinItemTable.AddCol("Big Reward", 80, ele => ele.isBigReward = Draw.Toggle(ele.isBigReward, 80));
        spinItemTable.AddCol("Reward", 80, ele =>
        {
            if(ele.reward.rewards.Count == 0) Draw.LabelBold(TextPack.NoReward, 80);
            else Draw.LabelBold(TextPack.HaveReward, 80);
        });
        spinItemTable.elementCreator = () => new SpinItemConfig("Spin Item " + (spinItemTable.FullList.Count + 1));

        spinItemTab = new SpinItemTabDrawer();
    }

    public override void DoDraw()
    {
        Draw.BeginChangeCheck();
        if(GatchaDrawMethod.Table == spinDetail.drawMethod) spinItemTable.DoDraw(spinDetail.spinItems);
        else spinItemTab.DoDraw(spinDetail.spinItems);
        if(Draw.EndChangeCheck()) CalculateRatioPercent(spinDetail.spinItems);
    }

    private void CalculateRatioPercent(List<SpinItemConfig> spinItems)
    {
        float totalPoint = spinItems.Sum(_ => _.weight);
        spinItems.ForEach(_ => _.probability = _.weight / totalPoint);
    }


    public class SpinItemTabDrawer : ListTabDrawer<SpinItemConfig>
    {
        readonly RewardDrawer rewardDrawer = new RewardDrawer();

        public SpinItemTabDrawer() { }

        protected override SpinItemConfig CreateNewElement() => new SpinItemConfig("New Item");

        protected override string GetTabLabel(SpinItemConfig element) => element.name;

        protected override void DoDrawContent(SpinItemConfig element)
        {
            Draw.BeginHorizontal();
            {
                Draw.BeginVertical();
                element.icon = Draw.Sprite(element.icon, true, 150, 150);
                Draw.EndVertical();

                Draw.Space(10);
                Draw.BeginVertical();
                {
                    element.name = Draw.TextField(TextPack.Name, element.name, 120);
                    element.isBigReward = Draw.ToggleField(TextPack.BigReward, element.isBigReward, 80);
                    element.weight = Draw.IntSliderField(TextPack.Weight, element.weight, 0, 9999, 400);
                    Draw.BeginHorizontal();
                    Draw.FitLabelBold(TextPack.Probability);
                    Draw.Space(50);
                    Draw.Label(element.probability.ToString("00.0000%"), 80);
                    Draw.EndHorizontal();
                }
                Draw.EndVertical();
                Draw.FlexibleSpace();
            }
            Draw.EndHorizontal();

            rewardDrawer.DoDraw(element.reward);
        }
    }
}