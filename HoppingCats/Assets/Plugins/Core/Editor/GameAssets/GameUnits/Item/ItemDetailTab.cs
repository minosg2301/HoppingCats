using System;
using UnityEngine;

namespace moonNest.editor
{
    public class ItemDetailTab : TabContent
    {
        private TabContainer tabContainer;

        public override void OnFocused()
        {
            tabContainer = new TabContainer();
            foreach(ItemDefinition itemDefinition in ItemAsset.Ins.definitions)
                tabContainer.AddTab(itemDefinition.name, new ItemConfigTab(itemDefinition));
        }

        public override bool DoDrawWindow() => tabContainer.DoDrawWindow();

        public override void DoDraw()
        {
            tabContainer.DoDraw();
        }

        private class ItemConfigTab : TabContent
        {
            private readonly ItemFilterDrawer itemFilterDrawer;
            private readonly ItemFilter itemFilter = new ItemFilter();
            private readonly ItemDefinition itemDefinition;

            private ColDrawer<ItemDetail> unlockByCol;
            private TableDrawer<ItemDetail> itemTable;
            private ListCardDrawer<ItemDetail> cardDrawer;

            Vector2 scrollPos;

            public ItemConfigTab(ItemDefinition definition)
            {
                itemDefinition = definition;
                itemFilter = itemDefinition.itemFilter;

                itemFilterDrawer = new ItemFilterDrawer();
                itemFilterDrawer.DrawFilteredList = false;
                itemFilterDrawer.FixedItemDefinition = itemDefinition;

                InitCardDrawer();
                InitTableDrawer();
            }

            private void InitCardDrawer()
            {
                cardDrawer = new ListCardDrawer<ItemDetail>();
                cardDrawer.onDrawElement = OnDrawElement;
                cardDrawer.elementCreator = () =>
                {
                    ItemDetail item = new ItemDetail(itemDefinition) { name = $"{itemDefinition.name} {ItemAsset.Ins.items.Count}" };
                    ItemAsset.Ins.Editor_AddItem(item);
                    itemFilterDrawer.Dirty = true;
                    return item;
                };
                cardDrawer.onElementRemoved = item =>
                {
                    ItemAsset.Ins.Editor_RemoveItem(item);
                    itemFilterDrawer.Dirty = true;
                };
                cardDrawer.CardWidth = 200;
                cardDrawer.CardHeight = 200;
                cardDrawer.LabelWidth = 80;
                cardDrawer.LineGap = 100;
            }

            private void OnDrawElement(ItemDetail ele)
            {
                ele.icon = Draw.Sprite(ele.icon, true, 250, 250);

                Draw.Space();
                ele.name = Draw.TextField("Name", ele.name, 200);
                ele.initAmount = Draw.IntField("Init", ele.initAmount, 60);
                ele.capacity = Draw.IntField("Capacity", ele.capacity, 60);
                ele.active = Draw.ToggleField("Active", ele.active, 50);

                Draw.Space();
                foreach(var @enum in ele.enums)
                {
                    EnumDefinition enumDef = GameDefinitionAsset.Ins.FindEnum(@enum.definitionId);
                    @enum.name = Draw.StringPopupField(enumDef.name, @enum.name, enumDef.stringList);
                }

                Draw.Space();
                for(int i = 0; i < itemDefinition.stats.Count; i++)
                    Draw.StatField(ele.Stat(itemDefinition.stats[i]));

                Draw.Space();
                foreach(var @ref in itemDefinition.refs)
                    Draw.ReferenceField(ele.Reference(@ref));

                Draw.Space();
                foreach(var attr in itemDefinition.attributes.ToArray())
                    Draw.AttributeField(ele.Attr(attr));
            }

            private void InitTableDrawer()
            {
                unlockByCol = new ColDrawer<ItemDetail>("Unlock By", 80, ele => DrawUnlockContent(ele, 80));
                itemTable = new TableDrawer<ItemDetail>();
                itemTable.AddCol("Name", 120, ele => ele.name = Draw.Text(ele.name, 120));
                if(itemDefinition.showDisplayName) itemTable.AddCol("Display Name", 120, ele => ele.displayName = Draw.Text(ele.displayName, 120));
                itemTable.AddCol("Icon", 100, ele => ele.icon = Draw.Sprite(ele.icon, 100));
                if(itemDefinition.showInitAmount) itemTable.AddCol("Init", 40, ele => ele.initAmount = Draw.Int(ele.initAmount, 40));
                if(itemDefinition.showCapacity) itemTable.AddCol("Capacity", 60, ele => ele.capacity = Draw.Int(ele.capacity, 60));
                if(itemDefinition.showExtended) itemTable.AddCol("Extended", 120, ele => ele.extendConfig = Draw.FlexiblePrefab(ele.extendConfig, 120));
                itemTable.AddCol("Active", 50, ele => ele.active = Draw.Toggle(ele.active, 50));
                itemTable.AddCol(unlockByCol);

                foreach(var enumProp in itemDefinition.enums)
                {
                    itemTable.AddCol(enumProp.name, enumProp.colWidth, ele =>
                    {
                        EnumDetail enumDetail = ele.EnumDetail(enumProp.id);
                        enumDetail.name = Draw.StringPopup(enumDetail.name, enumProp.Definition.stringList, enumProp.colWidth);
                    });
                }

                foreach(var stat in itemDefinition.stats)
                    itemTable.AddCol(stat.name, stat.colWidth, ele => Draw.Stat(ele.Stat(stat), stat.colWidth));

                foreach(var @ref in itemDefinition.refs)
                    itemTable.AddCol(@ref.name, @ref.colWidth, ele => Draw.Reference(ele.Reference(@ref), @ref.colWidth));

                foreach(var attr in itemDefinition.attributes)
                    itemTable.AddCol(attr.name, attr.colWidth, ele => Draw.Attribute(ele.Attr(attr), attr.colWidth));

                foreach(var assetRef in itemDefinition.assetRefs)
                    itemTable.AddCol(assetRef.name, assetRef.colWidth, ele => ele.AssetReference(assetRef).Draw(assetRef.colWidth));

                itemTable.elementCreator = () =>
                {
                    ItemDetail item = new ItemDetail(itemDefinition) { name = $"{itemDefinition.name} {itemTable.FullList.Count + 1}" };
                    ItemAsset.Ins.Editor_AddItem(item);
                    itemFilterDrawer.Dirty = true;
                    return item;
                };
                itemTable.onGetName = ele => ele.name;
                itemTable.onElementDeleted = OnItemDeleted;
                itemTable.onElementAdded = OnItemAdded;
                itemTable.onOrderChanged = OnOrderChanged;
            }

            private void OnItemDeleted(ItemDetail itemDetail)
            {
                ItemAsset.Ins.Editor_RemoveItem(itemDetail);
                UnlockContentAsset.Ins.RemoveContent(itemDetail.unlockContentId);
                itemFilterDrawer.Dirty = true;
            }

            private void OnItemAdded(ItemDetail itemDetail)
            {
                if(itemDefinition.unlockedByProgress)
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
                    Draw.SetDirty(UnlockContentAsset.Ins);
                }
            }

            private void DrawUnlockContent(ItemDetail ele, float maxWidth = -1)
            {
                Draw.BeginDisabledGroup(true);
                if(ele.UnlockContent)
                {
                    ele.UnlockContent.conditionId = Draw.IntPopup(ele.UnlockContent.conditionId, UnlockContentAsset.Ins.Datas, "name", "id", maxWidth);
                }
                else
                {
                    Draw.Label("-", maxWidth);
                }

                Draw.EndDisabledGroup();
            }

            private void OnOrderChanged(ItemDetail a, ItemDetail b)
            {
                ItemAsset.Ins.Editor_SwapItem(a, b);
                Draw.SetDirty(ItemAsset.Ins);
            }

            public override void DoDraw()
            {
                unlockByCol.enabled = itemDefinition.unlockedByProgress;

                itemFilterDrawer.DoDraw(itemFilter);

                Draw.Space();
                itemDefinition.drawMethod = Draw.EnumField("Draw Method", itemDefinition.drawMethod, 120);
                if(itemDefinition.drawMethod == ItemDrawMethod.Card)
                {
                    scrollPos = Draw.BeginScrollView(scrollPos);
                    cardDrawer.DoDraw(itemFilter.filteredItems);
                    Draw.EndScrollView();
                }
                else itemTable.DoDraw(itemFilter.filteredItems);
            }

            public override bool DoDrawWindow()
            {
                if(itemDefinition.drawMethod == ItemDrawMethod.Card)
                {
                    return false;// cardDrawer.DoDrawW;
                }
                else
                {
                    return itemTable.DoDrawWindow();
                }
            }
        }
    }
}