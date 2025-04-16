using moonNest;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkinConfig : BaseData
{
    public Sprite bgSprite;
    public CatController catPrefab;
    public List<PlatformConfig> platforms;
    public bool isFree;
    public int price;
}
