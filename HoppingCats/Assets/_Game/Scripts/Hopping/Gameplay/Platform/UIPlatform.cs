using System;
using UnityEngine;

public class UIPlatform : MonoBehaviour
{
    [Header("Components")]
    public Item item;
    public SpriteRenderer platformSprite;

    protected Platform data;
    protected int rowIndex = -1;
    protected bool isSafe = false;

    public int RowIndex => rowIndex;
    public bool IsSafe => isSafe;
    public bool IsEmpty => data == null;

    public Action<UIPlatform> onUpdateStatus = delegate { };

    public virtual void SetData(Platform data, int rowIndex)
    {
        this.data = data;
        this.rowIndex = rowIndex;
        isSafe = data.config.isSafe;
        gameObject.name = $"---- row {rowIndex} " + gameObject.name + " ----";
    }

    public virtual void ResetData()
    {
        data = null;
        rowIndex = -1;
        isSafe = false;
    }

}
