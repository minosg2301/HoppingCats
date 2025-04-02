using UnityEngine;

public class UIItem : MonoBehaviour
{
    public ItemConfig config;
    public virtual void SetData(ItemConfig config)
    {
        this.config = config;
    }

    public virtual void ItemTrigger()
    {
        gameObject.SetActive(false);
    }
}
