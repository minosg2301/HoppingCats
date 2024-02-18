using System;

namespace moonNest
{
    [Serializable]
    public class ReferenceDefinition : BaseDefinition
    {
        public int itemDefinitionId;

        // for item detail editor
        public int colWidth = 80;

        public ReferenceDefinition() { }
        public ReferenceDefinition(string name) : base(name) { }
    }
}