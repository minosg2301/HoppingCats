using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace moonNest.ads
{
    internal class AdsFormatTab : TabContent
    {
        readonly AdsPlatformConfig adsPlatformConfig;
        readonly TableDrawer<NetworkUsage> networkUsageTable;
        readonly Dictionary<AdsType, TableDrawer<DisplayConfig>> displayTables = new Dictionary<AdsType, TableDrawer<DisplayConfig>>();
        readonly Dictionary<AdsType, TableDrawer<string>> placementTables = new Dictionary<AdsType, TableDrawer<string>>();
        private TableDrawer<string> nativeAdUnitTable;
        private ColDrawer<string> adUnitColDrawer;

        public AdsFormatTab(AdsPlatformConfig adsPlatform)
        {
            adsPlatformConfig = adsPlatform;

            networkUsageTable = new TableDrawer<NetworkUsage>();
            networkUsageTable.drawControl = false;
            networkUsageTable.drawDeleteButton = false;
            networkUsageTable.drawIndex = false;
            networkUsageTable.drawOrder = false;
            networkUsageTable.AddCol("Network", 100, ele => Draw.LabelBold(ele.id.ToString(), 100));
            foreach (var type in Enum.GetValues(typeof(AdsType)))
                CreateAdsTypeCol(networkUsageTable, (AdsType)type);

            CorrectData();

            foreach (var format in adsPlatformConfig.formats)
            {
                CreateTable(format.type);
            }

            adUnitColDrawer = new ColDrawer<string>("", 350, (s) => Draw.Text(s, 350));
            nativeAdUnitTable = new TableDrawer<string>();
            nativeAdUnitTable.AddCol(adUnitColDrawer);
            nativeAdUnitTable.elementCreator = () => "";
            nativeAdUnitTable.drawHeader = false;
            nativeAdUnitTable.drawIndex = false;
            nativeAdUnitTable.drawControl = false;
        }

        private void CreateTable(AdsType type)
        {
            var table = new TableDrawer<DisplayConfig>();
            table.drawHeader = false;
            table.AddCol(" ", 150, ele => ele.networkId = Draw.Enum(ele.networkId, 150));
            table.AddCol(" ", 30, ele => ele.order = Draw.Int(ele.order, 30), false);
            table.AddCol(" ", 350, ele => ele.placement = Draw.Text(ele.placement, 350));
            table.AddCol(" ", 350, ele => ele.placement2nd = Draw.Text(ele.placement2nd, 350),
                ele => ele.networkId != AdsNetworkID.IRONSRC);
#if USE_AMAZON_SDK && IS_USE_AMAZON_MEDIATION
            table.AddCol("Amazon Ads Id", 350, ele => ele.amazonSlotId = Draw.Text(ele.amazonSlotId, 350),
                ele => ele.networkId == AdsNetworkID.IRONSRC);
            table.drawHeader = true;
#endif
            table.drawDeleteButton = table.drawControl = false;
            table.drawOrder = table.drawIndex = false;
            displayTables[type] = table;
        }

        void CreateAdsTypeCol(TableDrawer<NetworkUsage> networkUsageTable, AdsType adsType)
        {
            networkUsageTable.AddCol(adsType.ToString(), 100, ele =>
            {
                if (!ele.Formats.ContainsKey(adsType)) return;

                Draw.BeginChangeCheck();
                ele.Formats[adsType].order = Draw.Enum(ele.Formats[adsType].order, 100);
                if (Draw.EndChangeCheck())
                {
                    var newOrder = ele.Formats[adsType].order;
                    var others = adsPlatformConfig.usages
                        .Where(u => u != ele)
                        .Map(u => u.formats.Find(f => f.type == adsType))
                        .Where(u => u != null).ToList();

                    if (newOrder == FormatUsage.ORDER.MAIN)
                    {
                        var sameOrders = others.FindAll(o => o.order == newOrder);
                        sameOrders.ForEach(o => o.order = FormatUsage.ORDER.BACKUP);
                        others.RemoveAll(sameOrders);
                        others.FindAll(o => o.order == FormatUsage.ORDER.BACKUP).ForEach(o => o.order = FormatUsage.ORDER.__);
                    }
                    else if (newOrder == FormatUsage.ORDER.BACKUP)
                    {
                        others.FindAll(o => o.order == newOrder).ForEach(o => o.order = FormatUsage.ORDER.__);
                    }

                    // update display lists
                    var removeds = new List<DisplayConfig>();
                    var displays = adsPlatformConfig.formats.Find(f => f.type == adsType).displays;
                    foreach (var display in displays)
                    {
                        var format = adsPlatformConfig.usages.Find(u => u.id == display.networkId).formats.Find(f => f.type == adsType);
                        display.order = (int)format.order;
                        if (display.order == 0) removeds.Add(display);
                    }
                    displays.RemoveAll(removeds);

                    if (newOrder != FormatUsage.ORDER.__)
                    {
                        var exists = displays.Find(d => d.networkId == ele.id);
                        if (exists == null)
                        {
                            var displayConfig = new DisplayConfig() { networkId = ele.id, order = (int)newOrder };
                            HardCodeAdsPlacement(displayConfig, adsType);
                            displays.Add(displayConfig);
                        }
                    }
                }
            });
        }

        void HardCodeAdsPlacement(DisplayConfig displayConfig, AdsType adsType)
        {
            if (AdsNetworkID.IRONSRC == displayConfig.networkId)
            {
                switch (adsType)
                {
                    case AdsType.BANNER: displayConfig.placement = "DefaultBanner"; break;
                    case AdsType.INTERSTITIAL: displayConfig.placement = "DefaultInterstitial"; break;
                    case AdsType.REWARDED: displayConfig.placement = "DefaultRewardedVideo"; break;
                }
            }
        }

        void CorrectData()
        {
            // correct network usages
            if (adsPlatformConfig.usages.Count < Enum.GetNames(typeof(AdsNetworkID)).Length)
            {
                var networkIds = Enum.GetValues(typeof(AdsNetworkID));
                foreach (var id in networkIds)
                {
                    AdsNetworkID networkID = (AdsNetworkID)id;
                    var networkUsage = adsPlatformConfig.usages.Find(u => u.id == networkID);
                    if (networkUsage == null)
                    {
                        var newNetworkUsage = new NetworkUsage() { id = networkID };
                        var adsTypes = Enum.GetValues(typeof(AdsType));
                        foreach (var type in adsTypes)
                        {
                            var adsType = (AdsType)type;
                            if (adsType == AdsType.APP_OPEN && networkID != AdsNetworkID.ADMOB)
                                continue;

                            newNetworkUsage.formats.Add(new FormatUsage() { type = (AdsType)adsType });
                        }
                        adsPlatformConfig.usages.Add(newNetworkUsage);
                    }
                    else if (networkUsage.formats.Count < Enum.GetNames(typeof(AdsType)).Length)
                    {
                        var adsTypes = Enum.GetValues(typeof(AdsType));
                        foreach (var type in adsTypes)
                        {
                            var adsType = (AdsType)type;
                            if (adsType == AdsType.APP_OPEN && networkID != AdsNetworkID.ADMOB)
                                continue;

                            if (adsPlatformConfig.formats.Find(format => format.type == adsType) == null)
                            {
                                adsPlatformConfig.formats.Add(new AdsFormatConfig() { type = adsType });
                            }
                        }
                    }
                }
            }

            // correct formats list
            if (adsPlatformConfig.formats.Count < Enum.GetNames(typeof(AdsType)).Length)
            {
                var adsTypes = Enum.GetValues(typeof(AdsType));
                foreach (var type in adsTypes)
                {
                    var adsType = (AdsType)type;
                    if (adsPlatformConfig.FindAdsFormat(adsType) == null)
                    {
                        adsPlatformConfig.formats.Add(new AdsFormatConfig() { type = adsType });
                    }
                }
            }
        }

        public override void DoDraw()
        {
            networkUsageTable.DoDraw(adsPlatformConfig.usages);

            Draw.SpaceAndLabelBoldBox("Placements", Color.blue);
            foreach (var format in adsPlatformConfig.formats)
            {
                if (format.displays.Count == 0) continue;

                Draw.SpaceAndLabelBoldBox(format.type.ToString(), Color.green);
                if (format.type == AdsType.NATIVE)
                {
                    _DrawnNativeAds(format);
                }
                else
                {
                    displayTables[format.type].DoDraw(format.displays);
                }
            }
        }

        void _DrawnNativeAds(AdsFormatConfig format)
        {
            foreach (var displayConfig in format.displays)
            {
                Draw.BeginHorizontal();
                {
                    displayConfig.useMultiPlacements = Draw.ToggleField("Use Multi", displayConfig.useMultiPlacements, 50);
                    //Draw.Space(180);
                    Draw.LabelBold("Small Ads Unit", 350);
                    if (displayConfig.useMultiPlacements) Draw.Space(170);
                    Draw.LabelBold("Medium Ads Unit", 350);
                    Draw.FlexibleSpace();
                }
                Draw.EndHorizontal();

                Draw.BeginHorizontal();
                {
                    displayConfig.networkId = Draw.Enum(displayConfig.networkId, 150);
                    Draw.BeginDisabledGroup(true);
                    displayConfig.order = Draw.Int(displayConfig.order, 30);
                    Draw.EndDisabledGroup();
                    Draw.BeginDisabledGroup(displayConfig.useMultiPlacements);
                    displayConfig.placement = Draw.Text(displayConfig.placement, 350);
                    if (displayConfig.useMultiPlacements) Draw.Space(170);
                    displayConfig.placement2nd = Draw.Text(displayConfig.placement2nd, 350);
                    Draw.EndDisabledGroup();
                }
                Draw.EndHorizontal();

                Draw.BeginHorizontal();
                {
                    Draw.Space(180);
                    if (displayConfig.useMultiPlacements)
                    {
                        adUnitColDrawer.name = "Small Ads Unit";
                        nativeAdUnitTable.DoDraw(displayConfig.placements);
                        Draw.Space(96);
                        adUnitColDrawer.name = "Medium Ads Unit";
                        nativeAdUnitTable.DoDraw(displayConfig.placement2nds);
                    }
                    Draw.FlexibleSpace();
                }
                Draw.EndHorizontal();
            }
        }
    }
}