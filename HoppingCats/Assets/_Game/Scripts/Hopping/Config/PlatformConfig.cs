using moonNest;
using UnityEditor;
using UnityEngine;

public class PlatformConfig : BaseScriptableObject
{
#if UNITY_EDITOR
    [MenuItem("Moons/Create Jump Step")]
    static void Create()
    {
        CreateAsset<PlatformConfig>("Create Platform", "PlatformConfig");
    }
#endif

    public PlatformType platformType;
    public UIPlatform platformPrefab;
    public bool isSafe;
    public Sprite platformImage;
}

public enum PlatformType
{
    Normal,
    None,
    Cloud,
    Temp,
    Spike,
    Appear,
    First
}

