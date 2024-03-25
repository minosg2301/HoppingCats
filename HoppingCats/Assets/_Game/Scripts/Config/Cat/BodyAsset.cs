using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using moonNest;
using System;

[Serializable]
public class BodyAsset : SingletonScriptObject<BodyAsset>
{
    public List<BodyPart> parts = new List<BodyPart>();

    public BodyPart Find(string name) => parts.Find(x => x.name == name);
}

[Serializable]
public class BodyPart : BaseData
{
    public int sortingOrder;

    public BodyPart(string name, int sortingOrder = 0)
    {
        this.name = name;
        this.sortingOrder = sortingOrder;
    }
}