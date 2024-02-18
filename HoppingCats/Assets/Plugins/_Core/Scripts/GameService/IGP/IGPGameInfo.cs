using System;

namespace moonNest
{
    [Serializable]
    public class IGPGameInfo
    {
        public string Id { get; internal set; }
        public string Name { get; set; }
        public string Excludes { get; set; }
        public bool Show { get; set; }
        public string Identifier { get; set; }
        public string IosAppId { get; set; }
        public string IconUrl { get; set; }
        public string ImageUrl { get; set; }
        public float IosRankPoint { get; set; }
        public float AndroidRankPoint { get; set; }
        public string PolicyLink { get; set; }
    }

    [Serializable]
    public class IGPGameInfos
    {
        public int result;
        public IGPGameInfo[] data;
    }
}