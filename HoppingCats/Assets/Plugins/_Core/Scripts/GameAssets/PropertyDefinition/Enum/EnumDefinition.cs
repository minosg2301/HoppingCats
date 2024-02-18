using System;
using System.Collections.Generic;

namespace moonNest
{
    [Serializable]
    public class EnumDefinition : BaseDefinition
    {
        public List<string> stringList = new List<string>();

        // for item detail editor
        public int colWidth = 80;

        public EnumDefinition(string name) : base(name) { }
    }
}