using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using moonNest;
using System;

[Serializable]
public class TileAsset : SingletonScriptObject<TileAsset>
{
    public List<TileData> tiles = new List<TileData>();

    public TileData Find(string name) => tiles.Find(t => t.name == name);
    public TileData Find(int id) => tiles.Find(t => t.id == id);
}

[Serializable]
public class TileData : BaseData
{
    public TileData(string name) : base(name) { }
}
