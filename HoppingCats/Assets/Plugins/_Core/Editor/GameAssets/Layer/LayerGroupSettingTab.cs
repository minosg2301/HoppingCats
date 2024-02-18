using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace moonNest.editor
{
    public class LayerGroupSettingTab : TabContent
    {
        private readonly LayerGroup layerGroup;
        private readonly bool isProgressEnabled;
        private readonly ColDrawer<LayerDetail> requireCol;
        private readonly TableDrawer<LayerDetail> tableDrawer;

        public bool
            disableQuest,
            disableChest,
            disableShop,
            disableOnlineReward,
            disableIAPOffer,
            disableBattlePass;

        public LayerGroupSettingTab(LayerGroup layerGroup)
        {
            this.layerGroup = layerGroup;
            isProgressEnabled = layerGroup.StatDefinition.progress;

            requireCol = new ColDrawer<LayerDetail>(TextPack.Require, 80, ele => ele.requireValue = Draw.Int(ele.requireValue, 80), !isProgressEnabled);
            tableDrawer = new TableDrawer<LayerDetail>("Layer " + this.layerGroup.name);
            tableDrawer.AddCol(TextPack.Name, 120, ele => ele.name = Draw.Text(ele.name, 120));
            tableDrawer.AddCol(requireCol);
            tableDrawer.onElementDeleted = OnLayerDeleted;
            tableDrawer.drawControl = !isProgressEnabled;
            if(!isProgressEnabled) tableDrawer.elementCreator = CreateLayer;
        }

        /// <summary>
        /// Copy layer content from last layer to new layer
        /// </summary>
        /// <param name="last"></param>
        /// <param name="newLayer"></param>
        void UpdateLayerContent(LayerDetail last, LayerDetail newLayer)
        {
            if(last)
            {
                newLayer.chests.AddRange(last.chests.Map(chest => chest.Clone() as ChestLayer));
                newLayer.questGroups.AddRange(last.questGroups.Map(questGroup => questGroup.Clone() as QuestGroupLayer));
                newLayer.onlineRewards.AddRange(last.onlineRewards.Map(o => o.Clone() as OnlineRewardLayer));
                newLayer.shops.AddRange(last.shops.Map(o => o.Clone() as ShopLayer));
                newLayer.iapPackageGroups.AddRange(last.iapPackageGroups.Map(o => o.Clone() as IAPPackageGroupLayer));
            }
            else
            {
                if(ChestAsset.Ins.layerGroupId == layerGroup.id)
                    newLayer.chests = ChestAsset.Ins.chests
                                        .FindAll(chest => chest.layerEnabled)
                                        .Map(chest => new ChestLayer(chest));

                if(QuestAsset.Ins.layerGroupId == layerGroup.id)
                    newLayer.questGroups = QuestAsset.Ins.Groups
                                            .FindAll(questGroup => questGroup.layerEnabled)
                                            .Map(questGroup => new QuestGroupLayer(questGroup, QuestAsset.Ins.FindByGroup(questGroup.id)));

                if(OnlineRewardAsset.Ins.layerGroupId == layerGroup.id)
                    newLayer.onlineRewards = OnlineRewardAsset.Ins.onlineRewards.Map(or => new OnlineRewardLayer(or));

                if(ShopAsset.Ins.layerGroupId == layerGroup.id)
                    newLayer.shops = ShopAsset.Ins.Shops
                                    .FindAll(shop => shop.layerEnabled)
                                    .Map(shop => new ShopLayer(shop, ShopAsset.Ins.FindByGroup(shop.id)));

                if(IAPPackageAsset.Ins.layerGroupId == layerGroup.id)
                    newLayer.iapPackageGroups = IAPPackageAsset.Ins.Groups
                                                .FindAll(group => group.layerEnabled)
                                                .Map(group => new IAPPackageGroupLayer(group, IAPPackageAsset.Ins.FindByGroup(group.id)));
            }
        }

        public override void DoDraw()
        {
            Draw.BeginHorizontal();
            {
                Draw.BeginVertical(300);
                {
                    Draw.LabelBold("Override");
                    Draw.IndentLevel++;
                    DrawOverrideShop(layerGroup);
                    DrawOverrideQuest(layerGroup);
                    DrawOverrideChest(layerGroup);
                    DrawOverrideOnlineReward(layerGroup);
                    DrawOverrideIAPOffer(layerGroup);
                    DrawOverrideBattlePass(layerGroup);
                    Draw.IndentLevel--;

                    if(isProgressEnabled)
                    {
                        Draw.Space();
                        if(Draw.FitButton($"Sync with [{layerGroup.name}] Progresses", Color.cyan))
                        {
                            DoSyncDataWithProgress();
                        }
                    }
                }
                Draw.EndVertical();

                Draw.BeginVertical();
                tableDrawer.DoDraw(LayerAsset.Ins.FindByGroup(layerGroup.id));
                Draw.EndVertical();

                Draw.FlexibleSpace();
            }
            Draw.EndHorizontal();
        }

        void OnLayerDeleted(LayerDetail layer)
        {
            LayerAsset.Ins.Remove(layer);
        }

        LayerDetail CreateLayer()
        {
            int size = tableDrawer.FullList.Count;
            var last = size > 0 ? tableDrawer.FullList.Last() : null;
            var newLayer = new LayerDetail(string.Format("{0} {1}", layerGroup.name, size + 1)) { groupId = layerGroup.id, requireValue = size };
            UpdateLayerContent(last, newLayer);
            LayerAsset.Ins.Add(newLayer);
            return newLayer;
        }

        void DoSyncDataWithProgress()
        {
            bool haveChanged = false;
            var layers = LayerAsset.Ins.FindByGroup(layerGroup.id);
            var progressGroup = StatProgressAsset.Ins.FindGroupByStat(layerGroup.statId);
            progressGroup.progresses.ForEach(progress =>
            {
                int value = progressGroup.type == StatProgressType.Passive ? progress.statValue : progress.requireValue;
                var layer = layers.Find(layer => layer.requireValue == value);
                if(!layer)
                {
                    int size = tableDrawer.FullList.Count;
                    var last = size > 0 ? tableDrawer.FullList.Last() : null;
                    var newLayer = new LayerDetail(string.Format("{0} {1}", layerGroup.name, value)) { groupId = layerGroup.id, requireValue = value };
                    UpdateLayerContent(last, newLayer);
                    LayerAsset.Ins.Add(newLayer);
                    haveChanged = true;
                }
                else
                {
                    layer.name = progress.name;
                }
            });

            if(haveChanged)
            {
                layers = LayerAsset.Ins.FindByGroup(layerGroup.id);
                layers.SortAsc(c => c.requireValue);
                LayerAsset.Ins.UpdateList(layerGroup.id, layers);
            }
        }

        void DrawOverrideShop(LayerGroup group)
        {
            Draw.BeginChangeCheck();
            Draw.BeginDisabledGroup(disableShop);
            group.overrideShop = Draw.ToggleField("Shop", group.overrideShop, 60);
            Draw.EndDisabledGroup();
            if(Draw.EndChangeCheck())
            {
                List<LayerDetail> layers = LayerAsset.Ins.FindByGroup(group.id);
                layers.ForEach(layer => layer.shops.Clear());

                if(group.overrideShop)
                {
                    // enable flag for shop
                    ShopAsset.Ins.layerEnabled = true;
                    ShopAsset.Ins.layerGroupId = group.id;

                    // update shop item details for layer
                    group.shopIds.Clear();
                    ShopAsset.Ins.Shops.ForEach(shop =>
                    {
                        if(shop.layerEnabled)
                        {
                            group.shopIds.Add(shop.id);

                            List<ShopItemDetail> shopItems = ShopAsset.Ins.FindByGroup(shop.id);
                            layers.ForEach(layer => layer.shops.Add(new ShopLayer(shop, shopItems)));
                        }
                    });
                }
                else
                {
                    // disable flag for chests
                    ShopAsset.Ins.layerEnabled = false;
                    ShopAsset.Ins.layerGroupId = -1;
                }

                LayerAsset.Ins.onOverrideUpdated?.Invoke(group);
                Draw.SetDirty(ShopAsset.Ins);
            }
        }

        void DrawOverrideOnlineReward(LayerGroup group)
        {
            Draw.BeginChangeCheck();
            Draw.BeginDisabledGroup(disableOnlineReward);
            group.overrideOnlineReward = Draw.ToggleField("OnlineReward", group.overrideOnlineReward, 60);
            Draw.EndDisabledGroup();
            if(Draw.EndChangeCheck())
            {
                if(group.overrideOnlineReward)
                {
                    // enable flag for online reward
                    OnlineRewardAsset.Ins.layerEnabled = true;
                    OnlineRewardAsset.Ins.layerGroupId = group.id;

                    // update online rewards detail for layer
                    List<LayerDetail> layers = LayerAsset.Ins.FindByGroup(group.id);
                    layers.ForEach(layer =>
                    {
                        layer.onlineRewards.Clear();
                        layer.onlineRewards.AddRange(OnlineRewardAsset.Ins.onlineRewards.Map(o => new OnlineRewardLayer(o)));
                    });
                }
                else
                {
                    // disable flag for online reward
                    OnlineRewardAsset.Ins.layerEnabled = false;
                    OnlineRewardAsset.Ins.layerGroupId = -1;

                    // clear online reward layer
                    LayerAsset.Ins.FindByGroup(group.id).ForEach(layer => layer.onlineRewards.Clear());
                }

                LayerAsset.Ins.onOverrideUpdated?.Invoke(group);
                Draw.SetDirty(OnlineRewardAsset.Ins);
            }
        }

        void DrawOverrideChest(LayerGroup group)
        {
            Draw.BeginChangeCheck();
            Draw.BeginDisabledGroup(disableChest);
            group.overrideChest = Draw.ToggleField("Chest", group.overrideChest, 60);
            Draw.EndDisabledGroup();
            if(Draw.EndChangeCheck())
            {
                List<LayerDetail> layers = LayerAsset.Ins.FindByGroup(group.id);
                layers.ForEach(layer => layer.chests.Clear());

                if(group.overrideChest)
                {
                    // enable flag for chests
                    ChestAsset.Ins.layerEnabled = true;
                    ChestAsset.Ins.layerGroupId = group.id;

                    // update chest layers
                    ChestAsset.Ins.chests.ForEach(chest =>
                    {
                        if(chest.layerEnabled)
                            layers.ForEach(layer => layer.chests.Add(new ChestLayer(chest)));
                    });
                }
                else
                {
                    // disable flag for chests
                    ChestAsset.Ins.layerEnabled = false;
                    ChestAsset.Ins.layerGroupId = -1;
                }

                LayerAsset.Ins.onOverrideUpdated?.Invoke(group);
                Draw.SetDirty(ChestAsset.Ins);
            }
        }

        void DrawOverrideQuest(LayerGroup group)
        {
            Draw.BeginChangeCheck();
            Draw.BeginDisabledGroup(disableQuest);
            group.overrideQuest = Draw.ToggleField("Quest", group.overrideQuest, 60);
            Draw.EndDisabledGroup();
            if(Draw.EndChangeCheck())
            {
                List<LayerDetail> layers = LayerAsset.Ins.FindByGroup(group.id);
                layers.ForEach(layer => layer.questGroups.Clear());

                if(group.overrideQuest)
                {
                    // enable flag for chests
                    QuestAsset.Ins.layerEnabled = true;
                    QuestAsset.Ins.layerGroupId = group.id;

                    // update quest group  layers
                    group.questGroupIds.Clear();
                    QuestAsset.Ins.Groups.ForEach(questGroup =>
                    {
                        if(questGroup.layerEnabled)
                        {
                            group.questGroupIds.Add(questGroup.id);
                            List<QuestDetail> quests = QuestAsset.Ins.FindByGroup(questGroup.id);
                            layers.ForEach(layer => layer.questGroups.Add(new QuestGroupLayer(questGroup, quests)));
                        }
                    });
                }
                else
                {
                    // disable flag for chests
                    QuestAsset.Ins.layerEnabled = false;
                    QuestAsset.Ins.layerGroupId = -1;
                }

                LayerAsset.Ins.onOverrideUpdated?.Invoke(group);
                Draw.SetDirty(QuestAsset.Ins);
            }
        }

        void DrawOverrideIAPOffer(LayerGroup group)
        {
            Draw.BeginChangeCheck();
            Draw.BeginDisabledGroup(disableIAPOffer);
            group.overrideIAPOffer = Draw.ToggleField("IAP Offer", group.overrideIAPOffer, 60);
            Draw.EndDisabledGroup();
            if(Draw.EndChangeCheck())
            {
                List<LayerDetail> layers = LayerAsset.Ins.FindByGroup(group.id);
                layers.ForEach(layer => layer.iapPackageGroups.Clear());

                if(group.overrideIAPOffer)
                {
                    // enable flag for iap package
                    IAPPackageAsset.Ins.layerEnabled = true;
                    IAPPackageAsset.Ins.layerGroupId = group.id;

                    // update quest group  layers
                    group.packageGroupIds.Clear();
                    IAPPackageAsset.Ins.Groups.ForEach(packageGroup =>
                    {
                        if(packageGroup.layerEnabled)
                        {
                            group.packageGroupIds.Add(packageGroup.id);
                            List<IAPPackage> packages = IAPPackageAsset.Ins.FindByGroup(packageGroup.id);
                            layers.ForEach(layer => layer.iapPackageGroups.Add(new IAPPackageGroupLayer(packageGroup, packages)));
                        }
                    });
                }
                else
                {
                    // disable flag for chests
                    IAPPackageAsset.Ins.layerEnabled = false;
                    IAPPackageAsset.Ins.layerGroupId = -1;
                }

                LayerAsset.Ins.onOverrideUpdated?.Invoke(group);
                Draw.SetDirty(IAPPackageAsset.Ins);
            }
        }

        void DrawOverrideBattlePass(LayerGroup group)
        {
            Draw.BeginChangeCheck();
            Draw.BeginDisabledGroup(disableBattlePass);
            group.overrideBattlePass = Draw.ToggleField("Arena", group.overrideBattlePass, 60);
            Draw.EndDisabledGroup();
            if(Draw.EndChangeCheck())
            {
                List<LayerDetail> layers = LayerAsset.Ins.FindByGroup(group.id);
                if(group.overrideBattlePass)
                {
                    // enable flag for chests
                    ArenaAsset.Ins.layerEnabled = true;
                    ArenaAsset.Ins.layerGroupId = group.id;

                    // update chest layers
                    layers.ForEach(layer => layer.battlePassLevels = ArenaAsset.Ins.levels.Map(l => new BattlePassLevelLayer(l)));
                }
                else
                {
                    // disable flag for chests
                    ArenaAsset.Ins.layerEnabled = false;
                    ArenaAsset.Ins.layerGroupId = -1;
                    layers.ForEach(layer => layer.battlePassLevels.Clear());
                }

                LayerAsset.Ins.onOverrideUpdated?.Invoke(group);
                Draw.SetDirty(ArenaAsset.Ins);
            }
        }
    }
}