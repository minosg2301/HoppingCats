using moonNest;
using System;

[Serializable]
public class PlatformConfig : BaseData
{
    public PlatformType platformType;
    public UIPlatform platformPrefab;
    public bool isSafe;
}

public enum PlatformType
{
    Normal,
    None,
    Cloud,
    Temp,
    Spike,
    Appear,
    First
}
