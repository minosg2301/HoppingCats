using System;

namespace moonNest
{
    [Serializable]
    public class ShopDetail : BaseDetail<ShopDefinition>
    {
        public string displayName = "";
        public ShopRotateConfig refreshConfig = new ShopRotateConfig();
        public ShopType type;
        public UIShopItem prefab;
        public bool activeOnLoad = true;
        public bool replacePrefab = false;
        public bool layerEnabled = false;
        public bool sync = false;
        public bool removeItemOutStock = false;

        public ShopItemType ItemType => Definition.type;

        public ShopDetail(ShopDefinition definition) : base(definition)
        {
            displayName = definition.name;
        }

        protected override ShopDefinition GetDefinition(int definitionId) => ShopAsset.Ins.FindShopDefinition(definitionId);

        public override string ToString() => Definition.name;
    }

    [Serializable]
    public class ShopRotateConfig
    {
        public bool enabled = false;
        public int maxSlot = 6;
        public int period = 1;
    }

    public enum ShopType { List, Slot }
}