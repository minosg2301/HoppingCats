namespace moonNest.ads
{
    internal class PushNotificationTab : TabContent
    {
        private PushNotificationConfig pushNotiConfig;
        private TableDrawer<string> table;

        public PushNotificationTab()
        {
            pushNotiConfig = ThirdPartyConfig.Ins.pushNotiConfig;
            table = new TableDrawer<string>();
            table.AddCol(" ", 220, (ele) => Draw.Text(ele, 220));
            table.elementCreator = () => "New Content";
        }

        public override void DoDraw()
        {
            Draw.BeginVertical(Draw.SubContentStyle);
            {
                pushNotiConfig.hourToPush = Draw.IntField("Hour To Push", pushNotiConfig.hourToPush, 120);
                pushNotiConfig.days = Draw.IntField("Days", pushNotiConfig.days, 120);
                //pushNotiConfig.interval = Draw.IntField("Interval", pushNotiConfig.interval, 120);
                //pushNotiConfig.maxPush = Draw.IntField("Max Push", pushNotiConfig.maxPush, 120);
                //pushNotiConfig.ingameDelay = Draw.IntField("Ingame Delay", pushNotiConfig.ingameDelay, 120);

                Draw.Space();
                Draw.LabelBold("Contents");
                table.DoDraw(pushNotiConfig.contents);
            }
            Draw.EndVertical();
        }
    }
}