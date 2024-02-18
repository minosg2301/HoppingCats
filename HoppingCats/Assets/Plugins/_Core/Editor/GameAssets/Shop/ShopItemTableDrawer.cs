using System.Collections.Generic;
using UnityEditor;

namespace moonNest.editor
{
    public class ShopItemTableDrawer : TableDrawer<ShopItemDetail>
    {
        const float ContentWidth = 200;

        public ShopDetail Shop { get; set; }
        private List<int> slots = new List<int>();

        public List<CurrencyDefinition> Currencies { get; private set; }
        public List<ItemDetail> Items { get; private set; }
        public Dictionary<int, List<ItemDetail>> itemMap = new Dictionary<int, List<ItemDetail>>();

        public ShopItemTableDrawer(string name) : base(name)
        {
            AddCol("Name", 150, ele => ele.name = Draw.Text(ele.name, 150));
            AddCol("Icon", 120, ele => ele.icon = Draw.Sprite(ele.icon, 120));
            AddCol("Quantity", 80, ele => ele.quantity = Draw.Int(ele.quantity, 80));
            AddCol("Content", ContentWidth, ele => DrawItemContent(ele, ContentWidth));
            AddCol("Value", 80, ele => ele.contentValue = Draw.Int(ele.contentValue, 80));
            AddCol("Price", 130, ele => Draw.Price(ele.price, 130));
            AddCol("Prefab", 120, ele => ele.prefab = Draw.Object(ele.prefab, 120));
            AddCol("Tracking Id", 120, ele => ele.trackingId = Draw.Text(ele.trackingId, 120));
            AddCol("Kind", 60, ele => ele.kind = Draw.Enum(ele.kind, 60), ele => Shop.refreshConfig.enabled && Shop.type == ShopType.List);
            AddCol("Slot", 40, ele => ele.slot = Draw.IntPopup(ele.slot, slots, 40), ele => Shop.refreshConfig.enabled && Shop.type == ShopType.Slot);
            onElementAdded = shopItem => ShopAsset.Ins.AddItem(shopItem);
            onElementDeleted = shopItem => ShopAsset.Ins.RemoveItem(shopItem);
            elementCreator = () => new ShopItemDetail($"New {Shop.name}") { shopId = Shop.id, type = Shop.ItemType };
            onOrderChanged = OnOrderChanged;
        }

        private void OnOrderChanged(ShopItemDetail a, ShopItemDetail b)
        {
            ShopAsset.Ins.UpdateList(Shop.id, FullList);
        }

        private void DrawItemContent(ShopItemDetail ele, float contentWidth)
        {
            if(drawingInlineAdd)
            {
                ele.type = Draw.Enum(ele.type, contentWidth);
            }
            else
            {
                switch(ele.type)
                {
                    case ShopItemType.Currency: ele.contentId = Draw.IntPopup(ele.contentId, Currencies, "name", "id", contentWidth); break;
                    case ShopItemType.Chest: ele.contentId = Draw.IntPopup(ele.contentId, ChestAsset.Ins.chests, "name", "id", contentWidth); break;
                    case ShopItemType.Item:
                    {
                        if(Shop.ItemType == ShopItemType.All)
                        {
                            var itemDefinitionId = Draw.IntPopup(ele.filteredId, ItemAsset.Ins.definitions, "name", "id", contentWidth * 0.5f);
                            var itemDefinition = ItemAsset.Ins.FindDefinition(itemDefinitionId);
                            if(itemDefinition != null)
                            {
                                var items = ItemAsset.Ins.FindByDefinition(itemDefinition.id);
                                ele.contentId = Draw.IntPopup(ele.contentId, items, "name", "id", contentWidth * 0.5f);
                            }
                            else
                            {
                                ele.contentId = Draw.IntPopup(ele.contentId, ItemAsset.Ins.items, "name", "id", contentWidth * 0.5f);
                            }

                            ele.filteredId = itemDefinitionId;
                        }
                        else
                        {
                            ele.contentId = Draw.IntPopup(ele.contentId, Items, "name", "id", contentWidth);
                        }
                        break;
                    }
                    case ShopItemType.Random:
                        ele.contentId = Draw.IntPopup(ele.contentId, ItemAsset.Ins.itemRandoms, "name", "id", contentWidth);
                        break;
                }
            }
        }

        public override void DoDraw(List<ShopItemDetail> list)
        {
            inlineAdd = Shop.ItemType == ShopItemType.All;
            if(Shop.ItemType == ShopItemType.All)
            {
                Currencies = GameDefinitionAsset.Ins.currencies;
                Items = ItemAsset.Ins.items;
            }
            else if(Shop.ItemType == ShopItemType.Currency)
            {
                Currencies = GameDefinitionAsset.Ins.FindCurrenciesByType(CurrencyType.Soft);
            }
            else if(Shop.ItemType == ShopItemType.Item)
            {
                var itemDefinition = ItemAsset.Ins.FindDefinition(Shop.Definition.itemDefinitionId);
                Items = itemDefinition == null
                    ? ItemAsset.Ins.items
                    : ItemAsset.Ins.FindByDefinition(itemDefinition);
            }

            if(slots.Count != Shop.refreshConfig.maxSlot)
            {
                slots = new List<int>();
                for(int i = 0; i < Shop.refreshConfig.maxSlot; i++) { slots.Add(i + 1); }
            }

            base.DoDraw(list);

            if(inlineAdd)
            {
                disableInlineAddButton = WillAddedElement.type == ShopItemType.All;
                if(disableInlineAddButton)
                {
                    Draw.HelpBox("Select Shop Item type to add new Shop Item", MessageType.Error);
                }
            }
        }
    }
}
