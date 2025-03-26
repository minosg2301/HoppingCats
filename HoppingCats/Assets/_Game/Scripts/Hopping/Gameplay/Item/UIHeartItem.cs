using UnityEngine;

public class UIHeartItem : UIItem
{
    public override void ItemTrigger()
    {
        base.ItemTrigger();
        Debug.Log("T--- Heart !!!!!");
    }
}
