using System;
using UnityEngine;

public class UIPlatformStatus
{
    public bool isSafe;
}

public class UIPlatform : MonoBehaviour
{
    public Platform data;

    public Item item;

    public Action<UIPlatformStatus> onUpdateStatus = delegate {};

    private UIPlatformStatus status;

    public UIPlatformStatus Status => status;

    public UIPlatform(Platform data)
    {
        this.data = data;
    }

    public void SetData(Platform data)
    {
        this.data = data;
        status = new();
        status.isSafe = data.config.isSafe;
    }

    protected virtual void Active()
    {
        onUpdateStatus(status);
    }

    protected virtual void Deactive()
    {
        onUpdateStatus(status);
    }
}
