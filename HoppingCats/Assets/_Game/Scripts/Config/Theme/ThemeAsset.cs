using Doozy.Engine.Themes;
using moonNest;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ThemeAsset : SingletonScriptObject<ThemeAsset>
{
    public List<ThemeData> themes = new List<ThemeData>();

    public ThemeData Find(string name) => themes.Find(t => t.name == name);
}

[Serializable]
public class ThemeData : BaseData
{
    public List<ThemeTile> tiles = new List<ThemeTile>();

    public ThemeData(string name) : base(name) { }
    public ThemeTile Find(string type) => tiles.Find(t => t.type == type);
}

[Serializable]
public class ThemeTile : BaseData
{
    public string type;
    public AssetReferenceDetail sprite;

    public ThemeTile(string name) : base(name)
    {
        type = name;
    }

    public TileData Type { get { return TileAsset.Ins.Find(type); } }
}
