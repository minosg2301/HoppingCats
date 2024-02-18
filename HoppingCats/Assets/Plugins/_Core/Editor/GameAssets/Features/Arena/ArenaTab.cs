using moonNest;

internal class ArenaTab : TabContent
{
    readonly LeagueTabDrawer leagueTabDrawer;

    public ArenaTab()
    {
        leagueTabDrawer = new LeagueTabDrawer();
    }

    public void SetStat(StatDefinition statDef)
    {
        leagueTabDrawer.statFieldName = statDef ? statDef.name + " Value" : "-";
    }

    public override void DoDraw()
    {
        leagueTabDrawer.DoDraw(ArenaAsset.Ins.leagues);
    }

    internal class LeagueTabDrawer : ListTabDrawer<LeagueDetail>
    {
        public string statFieldName = "";
        public RewardDrawer rewardDrawer;
        public LeagueTabDrawer()
        {
            DrawAddButton = false;
            rewardDrawer = new RewardDrawer();
        }

        protected override LeagueDetail CreateNewElement() => null;

        protected override void DoDrawContent(LeagueDetail leagueDetail)
        {
            Draw.BeginDisabledGroup(true);
            Draw.IntField("League", leagueDetail.league, 60);
            Draw.IntField(statFieldName, leagueDetail.statValue, 60);
            Draw.EndDisabledGroup();
            leagueDetail.displayName = Draw.TextField("Display Name", leagueDetail.displayName, 240);
            Draw.Space();
            rewardDrawer.DoDraw(leagueDetail.reward);
        }

        protected override string GetTabLabel(LeagueDetail element) => element.name;
    }
}