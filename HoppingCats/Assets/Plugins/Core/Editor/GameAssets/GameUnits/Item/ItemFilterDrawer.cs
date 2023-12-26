using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace moonNest
{
    public class ItemFilterDrawer
    {
        private TableDrawer<ItemDetail> filterTable = new TableDrawer<ItemDetail>();

        public Action<List<ItemDetail>> onReplaceClicked;
        public Action<List<ItemDetail>> onAppendClicked;

        private SearchField searchField = new SearchField();

        public bool Dirty { get; set; } = true;
        public bool DrawFilteredList { get; set; } = true;
        public bool AutoFind { get; set; } = true;
        public bool AlwayShow { get; set; } = false;
        public bool DrawSearchName { get; set; } = true;
        public ItemDefinition FixedItemDefinition { get; set; } = null;

        public void DoDraw(ItemFilter itemFilter)
        {
            ItemDefinition itemDefinition = FixedItemDefinition;
            int itemDefinitionId = FixedItemDefinition ? FixedItemDefinition.id : -1;

            itemFilter.show = Draw.BeginFoldoutGroup(itemFilter.show, "Items Filter");
            if(itemFilter.show || AlwayShow)
            {
                Draw.BeginChangeCheck();
                Draw.BeginVertical(Draw.BoxStyle);
                {
                    if(DrawSearchName)
                        itemFilter.searchName = searchField.OnGUI(itemFilter.searchName);

                    if(FixedItemDefinition == null)
                    {
                        itemDefinitionId = Draw.IntPopupField("Item Definition", itemFilter.itemDefininitionId, ItemAsset.Ins.definitions, "name", "id", 200);
                        itemDefinition = ItemAsset.Ins.FindDefinition(itemDefinitionId);
                    }

                    if(itemDefinition != null)
                    {
                        if(filterTable == null || itemDefinitionId != itemFilter.itemDefininitionId)
                        {
                            filterTable = new TableDrawer<ItemDetail>();
                            filterTable.AddCol("Name", 120, ele => Draw.Label(ele.name, 120));
                            foreach(StatDefinition stat in itemDefinition.stats)
                            {
                                filterTable.AddCol(stat.name, 80, ele => Draw.Label(ele.Stat(stat).value.ToString(), 80));
                            }
                            foreach(var @enum in itemDefinition.enums)
                            {
                                var enumDef = GameDefinitionAsset.Ins.FindEnum(@enum.definitionId);
                                filterTable.AddCol(@enum.name, 100, ele =>
                                {
                                    var enumDetail = ele.EnumDetail(@enum.definitionId);
                                    Draw.Label(enumDetail.name, 100);
                                });
                            }
                            filterTable.drawControl = filterTable.drawOrder = false;

                            itemFilter.filteredItems.Clear();
                        }

                        Draw.BeginHorizontal();
                        {
                            // draw Enum Filter
                            Draw.BeginVertical(400);
                            Draw.LabelBold("Types Filter");
                            foreach(var @enum in itemDefinition.enums)
                            {
                                if(!itemFilter.TryGetEnumFilter(@enum.id, out var enumValue))
                                    itemFilter.AddEnumFilter(new EnumFilter(@enum));

                                DrawEnumFilter(@enum.name, itemFilter.GetEnumFilter(@enum.id));
                            }
                            Draw.EndVertical();

                            // draw Stat Filter
                            Draw.BeginVertical(400);
                            Draw.LabelBold("Stats Filter");
                            for(int i = 0; i < itemDefinition.stats.Count; i++)
                            {
                                StatDefinition stat = itemDefinition.stats[i];
                                if(stat.type == StatValueType.Int)
                                {
                                    if(!itemFilter.TryGetStatFilter(stat.name, out var statFilter))
                                        itemFilter.AddStatFilter(new StatFilter(stat.name));

                                    DrawStatFilter(itemFilter.GetStatFilter(stat.name));
                                }
                            }
                            Draw.EndVertical();

                            Draw.FlexibleSpace();
                        }
                        Draw.EndHorizontal();

                        if(!AutoFind)
                        {
                            Draw.Space();
                            Draw.BeginHorizontal();
                            {
                                if(Draw.Button("Find", 80))
                                {
                                    itemFilter.filteredItems = ItemAsset.Ins.FindWithFilter(itemDefinition, itemFilter);
                                }

                                Draw.Space();
                                if(Draw.Button("Clear", 80)) itemFilter.filteredItems.Clear();
                            }
                            Draw.EndHorizontal();
                        }

                        if(DrawFilteredList)
                        {
                            if(itemFilter.filteredItems.Count > 0)
                            {
                                Draw.Space(6);
                                Draw.SeparateHLine();
                                Draw.Space(6);

                                filterTable.DoDraw(itemFilter.filteredItems);
                            }

                            Draw.Space(6);
                            Draw.SeparateHLine();
                            Draw.Space(6);

                            Draw.BeginHorizontal();
                            {
                                if(Draw.Button("Replace", 120)) onReplaceClicked?.Invoke(itemFilter.filteredItems);

                                Draw.Space();
                                if(Draw.Button("Append", 120)) onAppendClicked?.Invoke(itemFilter.filteredItems);
                            }
                            Draw.EndHorizontal();
                        }
                    }

                    itemFilter.itemDefininitionId = itemDefinitionId;
                }
                Draw.EndVertical();

                if(Draw.EndChangeCheck())
                {
                    Dirty = true;
                }
            }
            Draw.EndFoldoutGroup();

            if(Dirty)
            {
                if(AutoFind && itemDefinition)
                {
                    itemFilter.filteredItems = ItemAsset.Ins.FindWithFilter(itemDefinition, itemFilter);

                }

                if(itemFilter.searchName.Length > 0)
                {
                    string lowerSearchName = itemFilter.searchName.ToLower();
                    itemFilter.filteredItems = itemFilter.filteredItems.FindAll(item => item.name.ToLower().Contains(lowerSearchName));
                }

                Dirty = false;
            }
        }

        private void DrawEnumFilter(string name, EnumFilter enumFilter)
        {
            var enumDef = GameDefinitionAsset.Ins.FindEnum(enumFilter.definitionId);
            Draw.BeginHorizontal();
            {
                Draw.Label(name, Draw.kLabelWidth);
                enumFilter.value = Draw.StringPopup(enumFilter.value, enumDef.stringList, 150);
                enumFilter.matchType = Draw.Enum(enumFilter.matchType, 50);
                Draw.EndDisabledGroup();
            }
            Draw.EndHorizontal();
        }

        private void DrawStatFilter(StatFilter statFilter)
        {
            Draw.BeginHorizontal();
            Draw.Label(statFilter.name, Draw.kLabelWidth);
            statFilter.type = Draw.Enum(statFilter.type, 120);
            Draw.BeginDisabledGroup(statFilter.type == CompareFunc.None);
            statFilter.value = Draw.StatValue(statFilter.value, 100);
            Draw.EndDisabledGroup();
            Draw.EndHorizontal();
        }
    }
}