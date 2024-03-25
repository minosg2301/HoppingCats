using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using moonNest;
using System;

[Serializable]
public class CatAsset : SingletonScriptObject<CatAsset>
{
    public List<CatData> cats = new List<CatData>();

    public CatData Find(string name) => cats.Find(c => c.name == name);
    public CatData Find(int id) => cats.Find(c => c.id == id);
}

[Serializable]
public class CatData : BaseData
{
    public string theme;
    public List<CatPart> catParts = new List<CatPart>();

    public CatData(string name) : base(name) { }
}

[Serializable]
public class CatPart : BaseData
{
    public AssetReferenceDetail sprite;
    public string type;
    public Vector2 offset;
    public Vector2 scale;

    public BodyPart Type { get { return BodyAsset.Ins.Find(type); } }
}
