using UnityEngine;

namespace moonNest.editor
{
    public class ItemDefinitionDrawer : ListTabDrawer<ItemDefinition>
    {
        private readonly ItemPropertyDefinitionDrawer itemPropertyDefinitionDrawer;

        public ItemDefinitionDrawer()
        {
            HeaderType = HeaderType.Vertical;

            onElementRemoved = OnItemDefinitionDeleted;
            askBeforeDelete = ele => "Consider delete Item";

            itemPropertyDefinitionDrawer = new ItemPropertyDefinitionDrawer();
        }

        private void OnItemDefinitionDeleted(ItemDefinition itemDefinition)
        {
            ItemAsset.Ins.RemoveItemByDefinition(itemDefinition);
            ShopDefinition shopDefinition = ShopAsset.Ins.shopDefinitions.Find(shop => shop.itemDefinitionId == itemDefinition.id);
            if(shopDefinition) ShopAsset.Ins.RemoveShop(shopDefinition);
        }

        protected override ItemDefinition CreateNewElement() => new ItemDefinition("New Item");

        protected override void DoDrawContent(ItemDefinition element)
        {
            element.name = Draw.TextField("Name", element.name, 200);

            Draw.BeginHorizontal();
            {
                Draw.BeginVertical();
                {
                    Draw.SpaceAndLabelBold("Behaviour");
                    DrawSellInShop(element);
                    DrawLockedToggle(element);
                    DrawStorageType(element);
                }
                Draw.EndVertical();

                Draw.Space(30);
                Draw.BeginVertical();
                {
                    Draw.SpaceAndLabelBold("Preset");
                    element.init = Draw.IntField("Init", element.init, 120);
                    element.capacity = Draw.IntField("Capacity", element.capacity, 120);
                    element.uiPrefab = Draw.ObjectField("UI Template", element.uiPrefab, 120);
                }
                Draw.EndVertical();

                Draw.Space(30);
                Draw.BeginVertical();
                {
                    Draw.SpaceAndLabelBold("Setting");
                    DrawShowInitAmount(element);
                    DrawShowCapacity(element);
                    DrawShowDisplayName(element);
                }
                Draw.EndVertical();

                Draw.Space(30);
                Draw.BeginVertical();
                {
                    Draw.SpaceAndLabelBold("Extended ScriptableObject");
                    element.scriptableCastName = Draw.TextField("Cast Name", element.scriptableCastName, 120);
                    element.showExtended = Draw.ToggleField("Show Extended", element.showExtended, 60);
                }
                Draw.EndVertical();

                Draw.FlexibleSpace();
            }
            Draw.EndHorizontal();

            Draw.Space();
            Draw.LabelBoldBox("Properties", Color.blue);
            itemPropertyDefinitionDrawer.DoDraw(element);
        }

        private void DrawStorageType(ItemDefinition element)
        {
            Draw.BeginChangeCheck();
            element.storageType = Draw.EnumField("Storage Type", element.storageType, 120);
            if(Draw.EndChangeCheck())
            {

            }
        }

        private static void DrawShowInitAmount(ItemDefinition element)
        {
            Draw.BeginChangeCheck();
            element.showInitAmount = Draw.ToggleField("Show Init", element.showInitAmount, 60);
            if(Draw.EndChangeCheck())
            {

            }
        }

        private static void DrawShowCapacity(ItemDefinition element)
        {
            Draw.BeginChangeCheck();
            element.showCapacity = Draw.ToggleField("Show Capacity", element.showCapacity, 60);
            if(Draw.EndChangeCheck())
            {

            }
        }

        private static void DrawShowDisplayName(ItemDefinition element)
        {
            Draw.BeginChangeCheck();
            element.showDisplayName = Draw.ToggleField("Show Display Name", element.showDisplayName, 60);
            if(Draw.EndChangeCheck())
            {

            }
        }

        private static void DrawSellInShop(ItemDefinition element)
        {
            Draw.BeginChangeCheck();
            element.sellInShop = Draw.ToggleField("Sell In Shop", element.sellInShop, 120);

            if(Draw.EndChangeCheck())
            {
                ShopDefinition shopDefinition;
                if(element.sellInShop)
                {
                    shopDefinition = new ShopDefinition(ShopItemType.Item, element.name) { itemDefinitionId = element.id };
                    ShopDetail shop = new ShopDetail(shopDefinition);
                    ShopAsset.Ins.shopDefinitions.Add(shopDefinition);
                    ShopAsset.Ins.AddShop(shop);
                    foreach(ItemDetail itemDetail in ItemAsset.Ins.FindByDefinition(element))
                    {
                        ShopItemDetail shopItem = new ShopItemDetail(itemDetail.name);
                        shopItem.price.currencyId = GameDefinitionAsset.Ins.currencies[0].id;
                        shopItem.contentId = itemDetail.id;
                        shopItem.shopId = shop.id;
                        shopItem.type = ShopItemType.Item;
                        ShopAsset.Ins.AddItem(shopItem);
                    }
                }
                else
                {
                    if(Draw.DisplayDialog("Sell In Shop", $"Uncheck this will lose all {element.name}'s Shop Items", "Understand", "Undo"))
                    {
                        shopDefinition = ShopAsset.Ins.shopDefinitions.Find(shop => shop.itemDefinitionId == element.id);
                        if(shopDefinition) ShopAsset.Ins.RemoveShop(shopDefinition);
                    }
                    else
                    {
                        element.sellInShop = true;
                    }
                }
            }
        }

        private static void DrawLockedToggle(ItemDefinition itemDefinition)
        {
            Draw.BeginChangeCheck();
            itemDefinition.unlockedByProgress = Draw.ToggleField("Locked", itemDefinition.unlockedByProgress);
            if(Draw.EndChangeCheck())
            {
                if(itemDefinition.unlockedByProgress)
                {
                    foreach(ItemDetail itemDetail in ItemAsset.Ins.FindByDefinition(itemDefinition))
                    {
                        if(!itemDetail.UnlockContent)
                        {
                            UnlockContentDetail unlockContent = new UnlockContentDetail("Unlock " + itemDetail.name)
                            {
                                type = UnlockContentType.Item,
                                conditionId = -1,
                                itemDefinitionId = itemDefinition.id,
                                contentId = itemDetail.id
                            };
                            itemDetail.UnlockContent = unlockContent;
                            UnlockContentAsset.Ins.AddContent(unlockContent);
                        }
                    }
                }
                else
                {
                    if(Draw.DisplayDialog("Unlock By Progress", $"Uncheck this will lose all Unlock Content of {itemDefinition.name}", "Understand", "Undo"))
                    {
                        foreach(ItemDetail itemDetail in ItemAsset.Ins.FindByDefinition(itemDefinition))
                        {
                            UnlockContentAsset.Ins.RemoveContent(itemDetail.unlockContentId);
                            itemDetail.UnlockContent = null;
                        }
                    }
                }
            }
        }

        protected override string GetTabLabel(ItemDefinition element) => element.name;
    }
}