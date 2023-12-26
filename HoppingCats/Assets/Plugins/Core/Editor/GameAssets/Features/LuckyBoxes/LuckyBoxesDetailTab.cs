using System.Collections.Generic;
using System.Linq;
using moonNest;

public class LuckyBoxesDetailTab : ListTabDrawer<LuckyBoxesDetail>
{
    readonly TableDrawer<LuckyBoxConfig> boxTable;
    readonly BoxDetailTabDrawer boxTabDrawer;

    public LuckyBoxesDetailTab()
    {
        boxTable = new TableDrawer<LuckyBoxConfig>();
        boxTable.AddCol("Name", 100, ele => ele.name = Draw.Text(ele.name, 100));
        boxTable.AddCol("Icon", 100, ele => ele.icon = Draw.Sprite(ele.icon, 100));
        boxTable.AddCol("Weight", 400, ele => ele.weight = Draw.IntSlider(ele.weight, 0, 9999, 400));
        boxTable.AddCol("Probability", 80, ele => Draw.Label(ele.probability.ToString("00.0000%"), 80));
        boxTable.AddCol("Big Reward", 80, ele => ele.isBigReward = Draw.Toggle(ele.isBigReward, 80));
        boxTable.AddCol("Reward", 80, ele =>
        {
            if(ele.reward.rewards.Count == 0) Draw.LabelBold(TextPack.NoReward, 80);
            else Draw.LabelBold(TextPack.HaveReward, 80);
        });
        boxTable.drawControl = false;
        boxTable.drawDeleteButton = false;
        boxTable.drawOrder = false;

        boxTabDrawer = new BoxDetailTabDrawer();
    }

    protected override LuckyBoxesDetail CreateNewElement() => new LuckyBoxesDetail("Lucky Box");

    protected override string GetTabLabel(LuckyBoxesDetail element) => element.name;

    protected override void DoDrawContent(LuckyBoxesDetail element)
    {
        element.name = Draw.TextField("Name", element.name, 200);
        Draw.PriceField("Cost", element.cost, 200);
        element.drawMethod = Draw.EnumField("Draw Method", element.drawMethod, 120);
        Draw.BeginChangeCheck();
        if(GatchaDrawMethod.Table == element.drawMethod) boxTable.DoDraw(element.boxes.ToList());
        else boxTabDrawer.DoDraw(element.boxes);
        if(Draw.EndChangeCheck()) CalculateRatioPercent(element.boxes.ToList());
    }

    private void CalculateRatioPercent(List<LuckyBoxConfig> spinItems)
    {
        float totalPoint = spinItems.Sum(_ => _.weight);
        spinItems.ForEach(_ => _.probability = _.weight / totalPoint);
    }

    public class BoxDetailTabDrawer : ListTabDrawer<LuckyBoxConfig>
    {
        readonly RewardDrawer rewardDrawer = new RewardDrawer();

        public BoxDetailTabDrawer() { }

        protected override LuckyBoxConfig CreateNewElement() => new LuckyBoxConfig("New Item");

        protected override string GetTabLabel(LuckyBoxConfig element) => element.name;

        protected override void DoDrawContent(LuckyBoxConfig element)
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