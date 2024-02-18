using System;
using System.Collections.Generic;

namespace moonNest.tracking
{
    [Serializable]
    public class TrackingConfig
    {
        public bool debugLog = false;
        public string appFlyerKey = "";
        public string iosAdjustKey = "";
        public string androidAdjustKey = "";
        public string huaweiAdjustKey = "";

        public List<VendorConfig> vendors = new List<VendorConfig>();
        public List<EventConfig> customEvents = new List<EventConfig>();
    }

    [Serializable]
    public class VendorConfig : BaseData
    {
        public string className;
        public bool enabled = true;
        public bool asDefault = true;

        public VendorConfig(string name) : base(name)
        {
        }
    }

    [Serializable]
    public class EventConfig
    {
        public string name;
        public List<int> vendorIds = new List<int>();

        // for editor
        public bool deletable;
    }
}