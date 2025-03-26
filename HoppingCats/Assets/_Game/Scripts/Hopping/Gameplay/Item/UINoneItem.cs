using UnityEngine;

public class UINoneItem : UIItem
{
    public override void ItemTrigger()
    {
        base.ItemTrigger();
        Debug.Log("T--- None !!!!!");
    }
}
