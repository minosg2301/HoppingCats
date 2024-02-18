using System;
using System.Collections.Generic;
using UnityEngine;
using moonNest;

public class ChestDetailTabDrawer : ListTabDrawer<ChestDetail>
{
    private ListCardDrawer<CurrencyReward> currencyDrawer;
    private ListCardDrawer<ItemReward> itemDrawer;

    public ChestDetailTabDrawer()
    {
        onElementAdded = OnChestAdded;
        onElementRemoved = OnChestRemoved;

        currencyDrawer = new ListCardDrawer<CurrencyReward>();
        currencyDrawer.onDrawElement = ele => RewardDrawer.DrawCurrency(ele);
        currencyDrawer.elementCreator = () => new CurrencyReward();
        currencyDrawer.onElementAdded = ele => ele.contentId = GameDefinitionAsset.Ins.currencies[0].id;

        itemDrawer = new ListCardDrawer<ItemReward>();
        itemDrawer.onDrawElement = ele => RewardDrawer.DrawItem(ele);
        itemDrawer.elementCreator = () => new ItemReward();
        itemDrawer.onElementAdded = OnItemAdded;
        itemDrawer.CardHeight = 130;
    }

    private void OnChestRemoved(ChestDetail chest)
    {
        if(ChestAsset.Ins.layerEnabled)
        {
            List<LayerDetail> layers = LayerAsset.Ins.FindByGroup(ChestAsset.Ins.layerGroupId);
            layers.ForEach(layer => layer.chests.Remove(c => c.chestId == chest.id));
        }
    }

    private void OnChestAdded(ChestDetail chest)
    {
        //if(ChestAsset.Ins.layerEnabled)
        //{
        //    List<LayerDetail> layers = LayerAsset.Ins.FindByGroup(ChestAsset.Ins.layerGroupId);
        //    layers.ForEach(layer => layer.chests.Add(new ChestLayer(chest)));
        //}
    }

    private void OnItemAdded(ItemReward itemReward)
    {
        var items = ItemAsset.Ins.items;
        itemReward.contentId = items.Count == 0 ? -1 : items[0].id;
    }

    protected override ChestDetail CreateNewElement() => new ChestDetail("New Chest");

    protected override string GetTabLabel(ChestDetail chest) => chest.name;

    protected override void DoDrawContent(ChestDetail chest)
    {
        Draw.BeginHorizontal();
        {
            Draw.BeginVertical();
            chest.icon = Draw.Sprite(chest.icon, true, 150, 150);
            Draw.EndVertical();

            Draw.Space(10);
            Draw.BeginVertical();
            chest.name = Draw.TextField(TextPack.Name, chest.name, 200);
            chest.displayName = Draw.TextField(TextPack.DisplayName, chest.displayName, 200);
            DrawLayerEnabled(chest);
            Draw.Space(10);
            chest.trackingId = Draw.TextField(TextPack.TrackingId, chest.trackingId, 200);
            Draw.EndVertical();
            Draw.FlexibleSpace();
            Draw.BeginVertical();
            Draw.Label("Key", 70);
            chest.keyIcon = Draw.Sprite(chest.keyIcon, false, 70, 70);
            Draw.EndVertical();
        }
        Draw.EndHorizontal();

        Draw.Space();
        Draw.LabelBoldBox(TextPack.Currencies, Color.blue);

        currencyDrawer.DoDraw(chest.content.currencies);

        Draw.Space();
        Draw.LabelBoldBox(TextPack.Items, Color.blue);
        itemDrawer.DoDraw(chest.content.items);
    }

    private void DrawLayerEnabled(ChestDetail chest)
    {
        Draw.BeginChangeCheck();
        Draw.BeginDisabledGroup(!ChestAsset.Ins.layerEnabled);
        chest.layerEnabled = Draw.ToggleField(TextPack.Layer, chest.layerEnabled, 60);
        if(chest.layerEnabled)
        {
            LayerGroup layerGroup = LayerAsset.Ins.FindGroup(ChestAsset.Ins.layerGroupId);
            if(layerGroup) Draw.LabelBoldBox($"Rewards is overrided by Layer '{layerGroup.name}'", Color.blue);
            else Draw.LabelBoldBox("Select which Layer Group in LAYER TAB", Color.red);
        }
        Draw.EndDisabledGroup();
        if(Draw.EndChangeCheck())
        {
            List<LayerDetail> layers = LayerAsset.Ins.FindByGroup(ChestAsset.Ins.layerGroupId);
            if(chest.layerEnabled)
            {
                layers.ForEach(layer => layer.chests.Add(new ChestLayer(chest)));
            }
            else
            {
                layers.ForEach(layer => layer.chests.Remove(c => c.chestId == chest.id));
            }
        }
    }
}