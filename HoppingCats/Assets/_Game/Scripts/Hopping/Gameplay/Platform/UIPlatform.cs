using System;
using UnityEngine;

public class UIPlatform : MonoBehaviour
{
    [Header("Components")]
    public SpriteRenderer platformSprite;

    protected UIItem item;
    protected Platform data;
    protected int rowIndex = -1;
    protected bool isSafe = false;

    public int RowIndex => rowIndex;
    public bool IsSafe => isSafe;

    public Action<UIPlatform> onUpdateStatus = delegate { };

    public virtual void SetData(Platform data, int rowIndex)
    {
        gameObject.name = $"---- row {rowIndex} " + gameObject.name + " ----";
        this.data = data;
        this.rowIndex = rowIndex;
        isSafe = data.config.isSafe;
        RandomItem();
    }

    public virtual void Trigger()
    {
        if (item) item.ItemTrigger();
    }

    public virtual void Active()
    {
        
    }

    public virtual void Deactive()
    {

    }

    protected virtual void RandomItem()
    {
        if (!isSafe || rowIndex == 0) return;
        var itemConfig = ItemGenerator.GenerateRandomItem();
        if (itemConfig != null && itemConfig.amount > 0)
        {
            item = Instantiate(itemConfig.prefab, transform);
            item.transform.localScale = Vector3.one;
        }
    } 
}
