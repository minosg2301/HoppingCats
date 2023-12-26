using System;

namespace moonNest
{
    [Serializable]
    public class ShopDefinition : BaseDefinition
    {
        public ShopItemType type = ShopItemType.All;
        public int itemDefinitionId;

        public ShopDefinition(string name) : base(name) { }

        public ShopDefinition(ShopItemType type, string name) : base(name)
        {
            this.type = type;
        }
    }
}