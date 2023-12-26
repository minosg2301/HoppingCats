using System;

namespace moonNest
{
    [Serializable]
    public class AttributeDetail
    {
        public int id;
        public string name;
        public AttributeValue value;

        public AttributeDetail(AttributeDefinition definition)
        {
            id = definition.id;
            name = definition.name;
            value = definition.initValue;
        }
    }
}
