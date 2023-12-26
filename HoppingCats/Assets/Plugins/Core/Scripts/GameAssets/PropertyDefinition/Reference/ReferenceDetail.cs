using System;

namespace moonNest
{
    [Serializable]
    public class ReferenceDetail
    {
        public int id;
        public string name;
        public int itemDefinitionId;
        public int referenceId = -1;

        public ReferenceDetail(ReferenceDefinition @ref)
        {
            id = @ref.id;
            name = @ref.name;
            itemDefinitionId = @ref.itemDefinitionId;
            referenceId = -1;
        }
    }
}