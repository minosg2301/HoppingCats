using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace moonNest.editor
{
    public class ShopTab : TabContent
    {
        readonly ListTabDrawer<ShopDetail> tabDrawer = new ShopDetailTab();

        public ShopTab()
        {
            tabDrawer.onElementCloned = OnElementCloned;
            tabDrawer.onElementRemoved = OnShopRemoved;
        }

        private void OnShopRemoved(ShopDetail shop)
        {
            ShopAsset.Ins.RemoveShop(shop);
        }

        private void OnElementCloned(ShopDetail newShop, ShopDetail originShop)
        {
            List<ShopItemDetail> shopItems = ShopAsset.Ins.FindByGroup(originShop.id);
            foreach(var shopItem in shopItems)
            {
                var newShopItem = shopItem.Clone() as ShopItemDetail;
                newShopItem.shopId = newShop.id;
                ShopAsset.Ins.AddItem(newShopItem);
            }
        }

        public override void DoDraw()
        {
            tabDrawer.DoDraw(ShopAsset.Ins.Shops);
        }

        class ShopDetailTab : ListTabDrawer<ShopDetail>
        {
            private ShopItemTableDrawer shopItemTable;

            public ShopDetail Shop { get; private set; }

            public ShopDetailTab()
            {
                DrawAddButton = false;
                shopItemTable = new ShopItemTableDrawer("Shop");
            }

            protected override ShopDetail CreateNewElement() => null;

            protected override string GetTabLabel(ShopDetail shop) => shop.name;

            protected override void DoDrawContent(ShopDetail shop)
            {

                switch(shop.ItemType)
                {
                    case ShopItemType.All: Draw.LabelBold($"All Items Shop"); break;
                    case ShopItemType.Currency: Draw.LabelBold($"{shop.name}'s Shop Items"); break;
                    case ShopItemType.Item: Draw.LabelBold($"{shop.name}'s Shop Items "); break;
                    default: Draw.LabelBold($"Chest's Shop Items "); break;
                }

                Draw.Space();
                Draw.BeginHorizontal();
                {
                    Draw.BeginVertical();
                    {
                        shop.name = Draw.TextField("Name", shop.name, 200);
                        shop.displayName = Draw.TextField("Display Name", shop.displayName, 200);
                        shop.prefab = Draw.ObjectField("Prefab", shop.prefab, 200);
                        shop.activeOnLoad = Draw.ToggleField("Active On Load", shop.activeOnLoad, 120);
                        shop.replacePrefab = Draw.ToggleField("Replace Update *", shop.replacePrefab, 120);
                    }
                    Draw.EndVertical();

                    Draw.Space(60);
                    Draw.BeginVertical();
                    {
                        Draw.LabelBold("Refresh Config");
                        Draw.BeginChangeCheck();
                        shop.refreshConfig.enabled = Draw.ToggleField("Refresh Enabled", shop.refreshConfig.enabled, 120);
                        if(Draw.EndChangeCheck())
                        {
                            if(shop.refreshConfig.enabled) shop.sync = true;
                        }
                        Draw.BeginDisabledGroup(!shop.refreshConfig.enabled);
                        {
                            shop.refreshConfig.period = Draw.IntField("Period (minute)", shop.refreshConfig.period, 120);
                            shop.refreshConfig.maxSlot = Draw.IntField("Max Slot", shop.refreshConfig.maxSlot, 120);
                            shop.type = Draw.EnumField("Shop Type", shop.type, 120);
                        }
                        Draw.EndDisabledGroup();
                    }
                    Draw.EndVertical();

                    Draw.Space(60);
                    Draw.BeginVertical();
                    {
                        shop.removeItemOutStock = Draw.ToggleField("Remove Out Stock", shop.removeItemOutStock, 80);

                        Draw.Space();

                        DrawLayerEnabled(shop);
                        Draw.BeginDisabledGroup(shop.refreshConfig.enabled);
                        shop.sync = Draw.ToggleField("Sync", shop.sync, 80);
                        Draw.EndDisabledGroup();
                    }
                    Draw.EndDisabledGroup();
                    Draw.EndVertical();

                    Draw.FlexibleSpace();
                }
                Draw.EndHorizontal();

                Draw.Space();
                Draw.SeparateHLine();
                Draw.Space();

                shopItemTable.Shop = shop;
                shopItemTable.DoDraw(ShopAsset.Ins.FindByGroup(shop.id));
            }

            private void DrawLayerEnabled(ShopDetail shop)
            {
                Draw.BeginChangeCheck();
                Draw.BeginDisabledGroup(!ShopAsset.Ins.layerEnabled);
                {
                    shop.layerEnabled = Draw.ToggleField("Layer Enabled", shop.layerEnabled, 120);
                    if(shop.layerEnabled)
                    {
                        LayerGroup layerGroup = LayerAsset.Ins.FindGroup(ShopAsset.Ins.layerGroupId);
                        if(layerGroup)
                            Draw.LabelBoldBox($"Shop is overrided by Layer '{layerGroup.name}'", Color.blue);
                        else
                            Draw.LabelBoldBox("Select which Layer Group in LAYER TAB", Color.red);
                    }
                    Draw.EndDisabledGroup();
                }

                if(Draw.EndChangeCheck())
                {
                    LayerGroup layerGroup = LayerAsset.Ins.FindGroup(ShopAsset.Ins.layerGroupId);
                    List<LayerDetail> layers = LayerAsset.Ins.FindByGroup(ShopAsset.Ins.layerGroupId);
                    if(shop.layerEnabled)
                    {
                        var shopItems = ShopAsset.Ins.FindByGroup(shop.id);
                        layerGroup.shopIds.Add(shop.id);
                        layers.ForEach(layer => layer.shops.Add(new ShopLayer(shop, shopItems)));
                    }
                    else
                    {
                        layerGroup.shopIds.Remove(shop.id);
                        layers.ForEach(layer => layer.shops.Remove(c => c.shopId == shop.id));
                    }
                }
            }
        }
    }
}