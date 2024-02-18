using System;

namespace moonNest
{
    [Serializable]
    public class IAPPackageGroup : BaseData
    {
        public bool x2FirstBuy = false;
        public RefreshConfig refreshConfig = new RefreshConfig();
        public bool layerEnabled = false;
        public bool sync = false;

        public IAPPackageGroup(string name) : base(name) { }

    }
}