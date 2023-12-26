using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using moonNest;

public class IAPPackageTabDrawer : ListTabDrawer<IAPPackage>
{
    public List<int> slots = new List<int>();
    public IAPPackageGroup group;
    readonly RewardDrawer rewardDrawer;
    readonly RewardListDrawer rewardListDrawer = new RewardListDrawer();

    public Action<IAPPackage> onSlotChanged = delegate { };
    public Action<IAPPackage> onPackageAdded = delegate { };
    public Action<IAPPackage> onPackageDeleted = delegate { };

    public IAPPackageTabDrawer()
    {
        rewardDrawer = new RewardDrawer();
        rewardListDrawer = new RewardListDrawer();

        onElementAdded = OnIAPPackageAdded;
        onElementRemoved = OnIAPPackageRemoved;
        onSwapPerformed = OnSwapped;
    }

    private void OnSwapped(IAPPackage package1, IAPPackage package2)
    {
        IAPPackageAsset.Ins.Editor_DoSwap(package1, package2);
        GUIUtility.ExitGUI();
    }

    private void OnIAPPackageAdded(IAPPackage iapPackage)
    {
        IAPPackageAsset.Ins.Add(iapPackage);

        if(IAPPackageAsset.Ins.layerEnabled && group.layerEnabled)
        {
            List<LayerDetail> layers = LayerAsset.Ins.FindByGroup(IAPPackageAsset.Ins.layerGroupId);
            layers.ForEach(layer =>
            {
                IAPPackageGroupLayer groupLayer = layer.iapPackageGroups.Find(g => g.groupId == group.id);
                groupLayer.packages.Add(new IAPPackageLayer(iapPackage));
            });
        }

        onPackageAdded(iapPackage);
    }

    private void OnIAPPackageRemoved(IAPPackage iapPackage)
    {
        IAPPackageAsset.Ins.Remove(iapPackage);

        if(IAPPackageAsset.Ins.layerEnabled && group.layerEnabled)
        {
            List<LayerDetail> layers = LayerAsset.Ins.FindByGroup(IAPPackageAsset.Ins.layerGroupId);
            layers.ForEach(layer =>
            {
                IAPPackageGroupLayer groupLayer = layer.iapPackageGroups.Find(g => g.groupId == group.id);
                groupLayer.packages.Remove(q => q.iapPackageId == iapPackage.id);
            });
        }

        onPackageDeleted(iapPackage);
    }

    protected override string GetTabLabel(IAPPackage package) => !package.free ? package.productId : TextPack.FreePackage;

    protected override IAPPackage CreateNewElement() => new IAPPackage("New Package", group.id) { productId = "com.package." + (List.Count + 1) };

    protected override void DoDrawContent(IAPPackage package)
    {
        Draw.BeginHorizontal();
        {
            Draw.BeginVertical();
            package.icon = Draw.Sprite(package.icon, true, 150, 150);
            Draw.EndVertical();

            Draw.Space();
            Draw.BeginVertical();
            Draw.BeginDisabledGroup(package.free);
            package.name = package.productId = Draw.TextField("Product Id", package.productId, 400);
            package.type = Draw.EnumField("Product Type", package.type, 150);
            Draw.EndDisabledGroup();
            Draw.Space();
            package.displayName = Draw.TextField("Title", package.displayName, 400);
            package.description = Draw.TextField("Description", package.description, 400);
            package.customPrefab = Draw.ObjectField("Custom Prefab", package.customPrefab, 200);
            package.quantity = Draw.IntField("Quantity", package.quantity, 80);
            Draw.EndVertical();

            Draw.Space(60);
            Draw.BeginVertical();
            package.free = Draw.ToggleField("Free Offer", package.free, 60);
            package.activeOnLoad = Draw.ToggleField("Active On Load", package.activeOnLoad, 60);
            package.promotedOnActive = Draw.ToggleField("Promoted On Active", package.promotedOnActive, 60);
            package.useMultiRewards = Draw.ToggleField("Multi Rewards", package.useMultiRewards, 60);
            package.randomOnActive = Draw.ToggleField("Random On Active", package.randomOnActive, 60);
            if(group.refreshConfig.enabled)
            {
                if(group.refreshConfig.limit.type == LimitType.ByAmount)
                {
                    package.showAlways = Draw.ToggleField("Show Always", package.showAlways, 60);
                }
            }
            Draw.Space();
            package.trackingId = Draw.TextField("Tracking Id", package.trackingId, 150);
            Draw.EndVertical();

            Draw.Space(60);
            Draw.BeginVertical();
            Draw.LabelBoldBox("Decoration");
            package.decorBackground = Draw.SpriteField("Decor Background", package.decorBackground, 120);
            package.decorContent = Draw.TextField("Decor Text", package.decorContent, 120);
            package.saleOff = Draw.IntField("Sale Off", package.saleOff, 60);
            Draw.EndVertical();

            Draw.FlexibleSpace();

        }
        Draw.EndHorizontal();

        if(package.promotedOnActive)
        {
            Draw.SpaceAndLabelBoldBox("Promotion", Color.yellow);
            Draw.BeginHorizontal();
            {
                Draw.BeginVertical();
                package.promotionIcon = Draw.Sprite(package.promotionIcon, true, 150, 150);
                Draw.EndVertical();

                Draw.Space(10);
                Draw.BeginVertical();
                package.promotionProductId = Draw.TextField("Product Id", package.promotionProductId, 400);
                package.promotionTitle = Draw.TextField("Title", package.promotionTitle, 400);
                package.promotionDescription = Draw.TextField("Description", package.promotionDescription, 400);
                package.promotionDuration = Draw.IntField("Duration (Minutes)", package.promotionDuration, 50);
                Draw.EndVertical();

                Draw.FlexibleSpace();
            }
            Draw.EndHorizontal();
        }

        Draw.Space();
        if(package.useMultiRewards)
        {
            rewardListDrawer.DoDraw(package.rewards);
        }
        else
        {
            // if use single reward, keep only first
            if(package.rewards.Count > 1) package.rewards.RemoveRange(1, package.rewards.Count - 1);
            rewardDrawer.DoDraw(package.rewards[0]);
        }
    }
}