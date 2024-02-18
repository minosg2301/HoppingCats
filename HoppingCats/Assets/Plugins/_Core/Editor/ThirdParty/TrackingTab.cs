using System;
using System.Collections.Generic;
using UnityEngine;
using moonNest.tracking;

namespace moonNest.ads
{
    internal class TrackingTab : TabContent
    {
        private TrackingConfig trackingConfig;
        private TableDrawer<VendorConfig> vendorTable;
        private TableDrawer<EventConfig> customEventTable;
        private List<VendorConfig> enabledVendors;

        const int kMaxVendor = 7;
        const string kGameAnalytics = "Game Analytics";
        const string kFacebook = "Facebook";
        const string kFirebase = "Firebase";
        const string kAppFlyer = "AppFlyer";
        const string kAdjust = "Adjust";
        const string kVgames = "Vgames";
        const string kLion = "Lion";

        public TrackingTab()
        {
            trackingConfig = ThirdPartyConfig.Ins.trackingConfig;

            CorrectVendors();
            UpdateEnableVendors();
            LoadCustomEvents();

            vendorTable = new TableDrawer<VendorConfig>();
            vendorTable.AddCol("Name", 120, ele => Draw.LabelBold(ele.name, 120));
            vendorTable.AddCol("ClassName", 200, ele => Draw.LabelBold(ele.className, 200));
            vendorTable.AddCol("Enabled", 60, ele => DrawVendorEnabled(ele, 60));
            vendorTable.AddCol("Default", 60, ele => ele.asDefault = Draw.Toggle(ele.asDefault, 60));
            vendorTable.drawOrder = vendorTable.drawDeleteButton = vendorTable.drawControl = false;

            customEventTable = new TableDrawer<EventConfig>();
            customEventTable.AddCol("Name", 200, ele =>
            {
                Draw.BeginDisabledGroup(!ele.deletable);
                ele.name = Draw.Text(ele.name, 200);
                Draw.EndDisabledGroup();
            });
            for (int i = 0; i < kMaxVendor; i++)
            {
                customEventTable.AddCol("Vendor" + (i + 1), 120, DrawVendor(i, 120));
            }
            customEventTable.checkDeletable = ele => ele.deletable;
            customEventTable.drawOrder = customEventTable.drawControl = false;
            customEventTable.elementCreator = () => new EventConfig() { vendorIds = enabledVendors.Map(v => v.id), deletable = true };
        }

        void CorrectVendors()
        {
            if (trackingConfig.vendors == null) trackingConfig.vendors = new List<VendorConfig>();
            if (trackingConfig.vendors.Count != kMaxVendor)
            {
                //AddVendor(kVgames, "VGameVendor");
                AddVendor(kGameAnalytics, "GATrackingVendor");
                AddVendor(kFacebook, "FBTrackingVendor");
                AddVendor(kFirebase, "FirebaseTrackingVendor");
                AddVendor(kAppFlyer, "AppFlyerTrackingVendor", false);
                AddVendor(kAdjust, "AdjustTrackingVendor", false);
                AddVendor(kLion, "LionTrackingVendor", false);

                // code used for remove
                //RemoveVendor(kAdjust, "AdjustTrackingVendor");
            }
        }

        void AddVendor(string name, string className, bool asDefault = true)
        {
            if (trackingConfig.vendors.Find(v => v.className == className) == null)
            {
                trackingConfig.vendors.Add(new VendorConfig(name) { className = className, asDefault = asDefault });
            }
        }

        void RemoveVendor(string className)
        {
            var vendor = trackingConfig.vendors.Find(v => v.className == className);
            if (vendor)
            {
                trackingConfig.vendors.Remove(vendor);
                foreach (var customEvent in trackingConfig.customEvents)
                {
                    customEvent.vendorIds.Remove(vendor.id);
                }
            }
        }

        void DrawVendorEnabled(VendorConfig ele, float maxW)
        {
            Draw.BeginChangeCheck();
            ele.enabled = Draw.Toggle(ele.enabled, maxW);
            if (Draw.EndChangeCheck()) UpdateEnableVendors();
        }

        void UpdateEnableVendors()
        {
            enabledVendors = trackingConfig.vendors.FindAll(v => v.enabled);
        }

        void LoadCustomEvents()
        {

        }

        void FixCustomEvent(string eventName)
        {
            var customEvent = trackingConfig.customEvents.Find(c => c.name == eventName);
            if (customEvent != null)
            {
                customEvent.deletable = true;
            }    
        }    

        void LoadCustomEvent(string eventName, params string[] vendors)
        {
            var customEvent = trackingConfig.customEvents.Find(c => c.name == eventName);
            if (customEvent == null)
            {
                customEvent = new EventConfig() { name = eventName, deletable = false };
                foreach (var vendorName in vendors)
                {
                    var vendor = trackingConfig.vendors.Find(v => v.name == vendorName);
                    if (vendor) customEvent.vendorIds.Add(vendor.id);
                }
                trackingConfig.customEvents.Add(customEvent);
            }
        }

        Action<EventConfig> DrawVendor(int index, float maxW)
        {
            return ele =>
            {
                int maxCount = ele.vendorIds.Count;
                if (index < maxCount)
                {
                    Draw.BeginChangeCheck();
                    int vendorId = ele.vendorIds[index];
                    vendorId = Draw.IntPopup(vendorId, enabledVendors, "name", "id", maxW);
                    if (Draw.EndChangeCheck())
                    {
                        if (vendorId == -1)
                        {
                            ele.vendorIds.RemoveAt(index);
                            GUIUtility.ExitGUI();
                        }
                        else
                        {
                            ele.vendorIds[index] = vendorId;
                        }
                    }
                }
                else if (index == maxCount)
                {
                    Draw.BeginChangeCheck();
                    int vendorId = Draw.IntPopup(-1, enabledVendors, "name", "id", maxW);
                    if (Draw.EndChangeCheck())
                    {
                        if (vendorId != -1)
                        {
                            ele.vendorIds.Add(vendorId);
                        }
                    }
                }
                else
                {
                    Draw.Label("", maxW);

                }
            };
        }

        public override void DoDraw()
        {
            Draw.BeginVertical(Draw.SubContentStyle);

            Draw.LabelBold("Adjust Token");
            trackingConfig.androidAdjustKey = Draw.TextField("Android", trackingConfig.androidAdjustKey, 200);
            trackingConfig.iosAdjustKey = Draw.TextField("IOS", trackingConfig.iosAdjustKey, 200);
            trackingConfig.huaweiAdjustKey = Draw.TextField("Huawei", trackingConfig.huaweiAdjustKey, 200);

            Draw.Space();
            trackingConfig.appFlyerKey = Draw.TextField("AppFlyer Key", trackingConfig.appFlyerKey, 200);
            trackingConfig.debugLog = Draw.ToggleField("Debug Log", trackingConfig.debugLog, 200);

            Draw.SpaceAndLabelBoldBox("Vendors", Color.blue);
            vendorTable.DoDraw(trackingConfig.vendors);

            Draw.Space();
            Draw.BeginHorizontal();
            Draw.LabelBoldBox("Custom Events", Color.green);
            if (Draw.FitButton("Load")) LoadCustomEvents();
            Draw.EndHorizontal();
            customEventTable.DoDraw(trackingConfig.customEvents);

            Draw.EndVertical();
        }
    }
}