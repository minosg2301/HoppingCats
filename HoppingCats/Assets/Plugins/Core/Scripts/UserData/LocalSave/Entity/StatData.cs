using System;
using UnityEngine;
using moonNest;

[Serializable]
public class StatData
{
    [NonSerialized] public int id;
    [NonSerialized] public string name;
    [NonSerialized] public bool sync;

    [SerializeField] internal StatValue value;

    public StatData(StatValue value)
    {
        this.value = value;
    }
}

[Serializable]
public class SafeStatData
{
    [NonSerialized] public int id;
    [NonSerialized] public string name;
    [NonSerialized] public bool sync;

    [SerializeField] internal SafeInt value;

    public SafeStatData(StatValue statValue)
    {
        value = new SafeInt(statValue);
    }
}
