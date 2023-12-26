using System;
using UnityEngine;
using moonNest;

[Serializable]
public class StatDefinition : BaseDefinition
{
    public string displayName = "";
    public Sprite displayIcon;
    public StatValueType type;
    public StatValue initValue;
    public bool safe = false;
    public bool deletable = true;
    public bool sync = true;
    public bool layer = false;
    public bool progress = false;
    public StatProgressType progressType;
    public bool lockContent = false;

    public bool savable = false;
    public bool distinct = false;

    // for item detail editor
    public int colWidth = 80;

    public StatDefinition() : base() { }
    public StatDefinition(string name) : base(name) { }

    public override string ToString() => name;
}

[Serializable]
public class StatListDefinition : BaseDefinition
{
    public string displayName = "";
    public Sprite displayIcon;
    public StatValueType type;

    public StatListDefinition(string name) : base(name) { }
}

/// <summary>
/// Enum to specify the type of value stored in this StatDefinition.
/// </summary>
public enum StatValueType : int
{
    /// <summary>
    /// Stat value is of type int.
    /// </summary>
    Int,

    /// <summary>
    /// Stat value is of type float.
    /// </summary>
    Float
}

public enum StatProgressType { Passive, Active }