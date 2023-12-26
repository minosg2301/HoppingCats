using moonNest;

public class SpinDetailTab : ListTabDrawer<SpinDetail>
{
    private readonly TableDrawer<SpinItemConfig> spinItemTable;
    private readonly TabContainer tabContainer = new TabContainer();
    private readonly SpinPointRewardTab spinPointRewardTab;
    private readonly TabItem pointRewardTab;
    private readonly SpinItemTab spinItemTab;

    public SpinDetailTab()
    {
        spinPointRewardTab = new SpinPointRewardTab();
        pointRewardTab = new TabItem("Point Reward", spinPointRewardTab);
        spinItemTab = new SpinItemTab();
        tabContainer.AddTab("Spin Item", spinItemTab);
        tabContainer.AddTab(pointRewardTab);
    }

    protected override SpinDetail CreateNewElement() => new SpinDetail("New Spin");

    protected override string GetTabLabel(SpinDetail element) => element.name;

    protected override void DoDrawContent(SpinDetail spinDetail)
    {
        Draw.BeginHorizontal();
        {
            Draw.BeginVertical(400);
            {
                spinDetail.name = Draw.TextField(TextPack.Name, spinDetail.name, 200);
                Draw.PriceField("Cost", spinDetail.cost, 200);
                spinDetail.multiSpin = Draw.IntField(TextPack.MultiTime, spinDetail.multiSpin, 120);
                spinDetail.multiSpinDiscount = Draw.FloatSliderField(TextPack.Discount, spinDetail.multiSpinDiscount, 0.5f, 0.95f, 200);
                spinDetail.drawMethod = Draw.EnumField("Draw Method", spinDetail.drawMethod, 120);
            }
            Draw.EndVertical();

            Draw.BeginVertical();
            {
                spinDetail.freePerDay = Draw.ToggleField(TextPack.FreePerDay, spinDetail.freePerDay, 150);
                DrawPointEnabled(spinDetail);
            }
            Draw.EndVertical();

            Draw.FlexibleSpace();
        }
        Draw.EndHorizontal();
        pointRewardTab.show = spinDetail.pointEnabled;
        spinPointRewardTab.spinDetail = spinDetail;
        spinItemTab.spinDetail = spinDetail;
        tabContainer.DoDraw();
    }

    private static void DrawPointEnabled(SpinDetail spinDetail)
    {
        Draw.BeginChangeCheck();
        spinDetail.pointEnabled = Draw.ToggleField(TextPack.PointEnabled, spinDetail.pointEnabled, 100);
        Draw.EndChangeCheck();
        if(spinDetail.pointEnabled)
        {
            spinDetail.pointIcon = Draw.SpriteField(TextPack.PointIcon, spinDetail.pointIcon, true, 70, 70);
        }
    }
}