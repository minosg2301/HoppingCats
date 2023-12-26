using System;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class EnumDetail
    {
        [SerializeField] private int id;
        public string name;
        public int definitionId;
        public bool distinct;

        public int Id => id;

        public EnumDetail(EnumPropertyDefinition enumProp)
        {
            id = enumProp.id;
            definitionId = enumProp.definitionId;
            name = enumProp.Definition.stringList[0];
        }

        [NonSerialized] private EnumDefinition _definition;
        public EnumDefinition Definition { get { if(!_definition) _definition = GameDefinitionAsset.Ins.enums.Find(e => e.id == definitionId); return _definition; } }

        public override string ToString() => name;
    }
}