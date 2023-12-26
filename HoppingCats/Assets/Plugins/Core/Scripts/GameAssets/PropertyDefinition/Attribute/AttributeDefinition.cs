using System;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class AttributeDefinition : BaseData
    {
        public string displayName = "";
        public Sprite displayIcon;
        public bool deletable = true;
        public bool sync = true;
        public AttributeType type;
        public AttributeValue initValue = new AttributeValue();

        // for item detail editor
        public int colWidth = 80;

        public AttributeDefinition() { }

        public AttributeDefinition(string name, AttributeType type) : base(name)
        {
            this.type = type;
        }
    }

    public enum AttributeType { String, Sprite, GameObject }
}