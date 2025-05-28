using moonNest;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkinConfig : BaseData
{
    public string displayName;
    public Sprite bgSprite;
    public Color bgColor = Color.white;
    public CatController catPrefab;
    public List<PlatformConfig> platforms;
    public bool isFree;
    public int price;
}
