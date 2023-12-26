using System;

namespace moonNest
{
    [Serializable]
    public class ItemDefinition : BaseObjectDefinition
    {
        public bool sellInShop;
        public bool unlockedByProgress;
        public int init = 0;
        public int capacity = -1;
        public StorageType storageType = StorageType.Single;
        public UIItemDetail uiPrefab;

        public ItemDrawMethod drawMethod = ItemDrawMethod.Table;
        public ItemFilter itemFilter = new ItemFilter();
        
        public bool showDisplayName = false;
        public bool showInitAmount = false;
        public bool showCapacity = false;

        public string scriptableCastName = "";
        public bool showExtended = false;

        public ItemDefinition(string name) : base(name) { }
    }

    public enum StorageType { Single, Several }
    public enum ItemDrawMethod { Table, Card }
}