using System;
using System.Collections.Generic;
using System.Linq;
using moonNest;

[Serializable]
public class StatDetail
{
    public int id;
    public string name;
    public StatValue value = 0;
    public bool distinct = false;

    public StatDetail(string name)
    {
        this.name = name;
    }

    public StatDetail(StatDefinition stat)
    {
        id = stat.id;
        name = stat.name;
        value = stat.initValue;
        distinct = stat.distinct;
    }

    public override string ToString() => name;
}

[Serializable]
public class StatListDetail : ICloneable
{
    public string name;
    public List<StatValue> values = new List<StatValue>();

    public StatListDetail(string name)
    {
        this.name = name;
    }

    public object Clone() => new StatListDetail(name) { values = values.ToList() };
}