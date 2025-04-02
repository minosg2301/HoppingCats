public class UIStarItem : UIItem
{
    public override void ItemTrigger()
    {
        base.ItemTrigger();
        GameEventManager.Ins.OnAddStar(config.amount);
    }
}
