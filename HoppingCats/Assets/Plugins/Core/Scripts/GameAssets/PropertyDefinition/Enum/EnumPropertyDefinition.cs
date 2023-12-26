using System;

namespace moonNest
{
    [Serializable]
    public class EnumPropertyDefinition : BaseData
    {
        public int definitionId;
        public bool savable = false;
        public bool distinct = false;

        // for item detail editor
        public int colWidth = 80;

        public EnumPropertyDefinition() { }
        public EnumPropertyDefinition(string name) : base(name) { }

        [NonSerialized] private EnumDefinition _definition;
        public EnumDefinition Definition { get { if(!_definition) _definition = GameDefinitionAsset.Ins.enums.Find(e => e.id == definitionId); return _definition; } }
    }
}