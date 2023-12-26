using System;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class AssetReferenceDefinition : BaseData
    {
        public string displayName = "";
        public Sprite displayIcon;
        public AssetType type;

        // for item detail editor
        public int colWidth = 80;

        public AssetReferenceDefinition() { }
        public AssetReferenceDefinition(string name) : base(name) { }
    }

    public enum AssetType { Sprite, GameObject, AudioClip }

}