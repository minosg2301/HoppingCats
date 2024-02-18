using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace moonNest.editor
{
    internal class ItemRandomTab : TabContent
    {
        readonly ItemRandomTabDrawer tabDrawer;

        public ItemRandomTab()
        {
            tabDrawer = new ItemRandomTabDrawer();
        }

        public override void DoDraw()
        {
            tabDrawer.DoDraw(ItemAsset.Ins.itemRandoms);
        }

        private class ItemRandomTabDrawer : ListTabDrawer<RandomDetail>
        {
            readonly TableDrawer<RandomContentDetail> tableDrawer;
            readonly ItemFilterDrawer itemFilterDrawer;
            readonly ColDrawer<RandomContentDetail> distinctCol1;
            readonly ColDrawer<RandomContentDetail> distinctCol2;
            readonly ColDrawer<RandomContentDetail> definitionCol;

            public List<RandomDetail> subRandoms;

            public RandomDetail ItemRandomDetail { get; private set; }

            public ItemRandomTabDrawer()
            {
                distinctCol1 = new ColDrawer<RandomContentDetail>("Stat 1", 150, ele => DrawIndividual(ele, 0, 150));
                distinctCol2 = new ColDrawer<RandomContentDetail>("Stat 2", 150, ele => DrawIndividual(ele, 1, 150));
                definitionCol = new ColDrawer<RandomContentDetail>("Definition", 120, ele => DrawItemDefinition(ele, 120), ele => ele.type == RandomContentType.Item);

                tableDrawer = new TableDrawer<RandomContentDetail>();
                tableDrawer.AddCol("Type", 120, ele => ele.type = Draw.Enum(ele.type, 120));
                tableDrawer.AddCol(definitionCol);
                tableDrawer.AddCol("Content", 150, ele => DrawContent(ele, 150));
                tableDrawer.AddCol(distinctCol1);
                tableDrawer.AddCol(distinctCol2);
                tableDrawer.AddCol("Weight", 400, ele => ele.weight = Draw.IntSlider(ele.weight, 0, 9999, 300));
                tableDrawer.AddCol("Probability", 100, ele => Draw.Label(ele.probability.ToString("00.0000%"), 100));
                tableDrawer.elementCreator = () => new RandomContentDetail();
                tableDrawer.drawControl = false;

                itemFilterDrawer = new ItemFilterDrawer();
                itemFilterDrawer.onAppendClicked = AppendFilterResults;
                itemFilterDrawer.onReplaceClicked = ReplaceFilterResults;
                itemFilterDrawer.DrawSearchName = false;
            }

            private void DrawContent(RandomContentDetail ele, float maxWidth)
            {
                if(ele.type == RandomContentType.Item)
                    ele.contentId = Draw.IntPopup(ele.contentId, ItemAsset.Ins.FindByDefinition(ele.itemDefinitionId), "name", "id", maxWidth);
                else
                    ele.contentId = Draw.IntPopup(ele.contentId, subRandoms, "name", "id", maxWidth);
            }

            private void DrawItemDefinition(RandomContentDetail ele, float maxWidth)
            {
                Draw.BeginChangeCheck();
                ele.itemDefinitionId = Draw.IntPopup(ele.itemDefinitionId, ItemAsset.Ins.definitions, "name", "id", maxWidth);
                if(Draw.EndChangeCheck() && ele.itemDefinitionId != -1)
                {
                    ItemDefinition definition = ItemAsset.Ins.definitions.Find(def => def.id == ele.itemDefinitionId);
                    List<StatDefinition> distinctStats = definition.stats.FindAll(s => s.distinct);
                    if(definition.storageType == StorageType.Several)
                    {
                        if(ele.stats.Length != distinctStats.Count)
                        {
                            ele.stats = new DistinctStat[distinctStats.Count];
                            for(int i = 0; i < distinctStats.Count; i++)
                                ele.stats[i] = new DistinctStat(distinctStats[i].id, distinctStats[i].initValue);
                        }
                    }
                    else
                    {
                        ele.stats = new DistinctStat[0];
                    }
                }
            }

            private void DrawIndividual(RandomContentDetail ele, int index, float width)
            {
                if(index < ele.stats.Length && ele.Item)
                {
                    float valueW = 50;
                    float nameW = width - valueW - 2;
                    ele.stats[index].id = Draw.IntPopup(ele.stats[index].id, ele.Item.stats.FindAll(s => s.distinct), "name", "id", nameW);
                    ele.stats[index].value = Draw.Int(ele.stats[index].value, valueW);
                }
                else
                {
                    Draw.Label(" ", width);
                }
            }

            protected override RandomDetail CreateNewElement() => new RandomDetail("New Item Random");

            protected override void DoDrawContent(RandomDetail element)
            {
                ItemRandomDetail = element;
                subRandoms = ItemAsset.Ins.itemRandoms.FindAll(r => r != element);
                Draw.BeginHorizontal();
                {
                    Draw.BeginVertical();
                    element.icon = Draw.Sprite(element.icon, true, 150, 150);
                    Draw.EndVertical();

                    Draw.Space(10);
                    Draw.BeginVertical();
                    element.name = Draw.TextField("Name", element.name, 200);
                    element.displayName = Draw.TextField("Display Name", element.displayName, 200);
                    Draw.EndVertical();
                    Draw.FlexibleSpace();
                }
                Draw.EndHorizontal();

                Draw.Space();
                itemFilterDrawer.DoDraw(element.itemFilter);

                Draw.Space();
                Draw.LabelBoldBox("Items Random", Color.green);
                Draw.BeginChangeCheck();
                if(element.randoms.Count > 0)
                {
                    definitionCol.enabled = element.randoms.Find(r => r.type == RandomContentType.Item) != null;

                    int distinctCount = element.randoms.Max(r => r.stats.Length);
                    distinctCol1.enabled = distinctCount >= 1;
                    distinctCol2.enabled = distinctCount == 2;
                }
                tableDrawer.DoDraw(element.randoms);
                if(Draw.EndChangeCheck())
                {
                    CalculateProbability(element.randoms);
                }
            }

            private void ReplaceFilterResults(List<ItemDetail> filterResults)
            {
                ItemRandomDetail.randoms.Clear();
                AddFilteredItems(filterResults);
            }

            private void AppendFilterResults(List<ItemDetail> filterResults)
            {
                AddFilteredItems(filterResults);
            }

            private void AddFilteredItems(List<ItemDetail> filterResults)
            {
                foreach(ItemDetail item in filterResults)
                {
                    RandomContentDetail random = new RandomContentDetail
                    {
                        itemDefinitionId = item.definitionId,
                        contentId = item.id,
                        type = RandomContentType.Item,
                        stats = new DistinctStat[item.Definition.stats.FindAll(stat => stat.distinct).Count]
                    };
                    ItemRandomDetail.randoms.Add(random);
                }
                CalculateProbability(ItemRandomDetail.randoms);
            }

            private void CalculateProbability(List<RandomContentDetail> randomDetails)
            {
                float totalPoint = randomDetails.Sum(_ => _.weight);
                randomDetails.ForEach(_ => _.probability = totalPoint == 0 ? 0 : _.weight / totalPoint);
            }

            protected override string GetTabLabel(RandomDetail element) => element.name;
        }
    }
}