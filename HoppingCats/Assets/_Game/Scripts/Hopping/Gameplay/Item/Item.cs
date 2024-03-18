using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemConfig config;

    public Item(ItemConfig config)
    {
        this.config = config;
    }

    public virtual void ApplyItem()
    {

    }
}
