public class UIHeartItem : UIItem
{
    public override void ItemTrigger()
    {
        base.ItemTrigger();
        GameEventManager.Ins.OnAddHealth(config.amount);
    }
}
