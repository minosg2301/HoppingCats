using moonNest;
using System;
using System.Collections.Generic;
using Range = moonNest.Range;

[Serializable]
public class LevelConfig : BaseData
{
    public int level;
    public int fromFloor;
    public int toFloor;
    public List<PlatformRandomData> platformRandomDatas;

    public bool IsInRange(int floor)
    {
        return floor >= fromFloor && floor < toFloor;
    }
}

[Serializable]
public class PlatformRandomData : BaseData
{
    public PlatformType type;
    public bool isSafe;
    public int weight = 100;
    public float probability;
}
