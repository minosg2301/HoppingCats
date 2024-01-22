using moonNest;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class JumpConfig : BaseScriptableObject
{
#if UNITY_EDITOR
    [MenuItem("Moons/Create Jump Step")]
    static void Create()
    {
        CreateAsset<JumpConfig>("Create Jump Step", "JumpConfig");
    }
#endif

    public JumpType jumpType;
    public JumpStep jumpStep;
    public bool safeJumpType;
    public Sprite jumpImage;
}

public enum JumpType
{
    Normal,
    None,
    Cloud,
    Temp,
    Spike,
    Appear,
    First
}

