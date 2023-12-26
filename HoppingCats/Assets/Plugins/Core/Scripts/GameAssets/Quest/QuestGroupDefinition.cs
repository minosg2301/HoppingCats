using System;

namespace moonNest
{
    [Serializable]
    public class QuestGroupDefinition : BaseDefinition
    {
        public int period = -1;

        public QuestGroupDefinition(string name) : base(name) { }
    }
}