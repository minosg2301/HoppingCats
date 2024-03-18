using moonNest;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemConfig : BaseScriptableObject
{
#if UNITY_EDITOR
    [MenuItem("Moons/Create Item")]
    static void Create()
    {
        CreateAsset<ItemConfig>("Create Item", "Item");
    }
#endif

    public Sprite itemImage;
}
