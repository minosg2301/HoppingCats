using System;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class Feature
    {
        [SerializeField] private int id;
        [SerializeField] private bool locked;

        public int Id => id;
        public bool Locked => locked;
        public bool Unlocked => !locked;

        private FeatureConfig _config;
        public FeatureConfig Config { get { if(_config == null) _config = GameDefinitionAsset.Ins.FindFeature(id); return _config; } }

        public Feature(FeatureConfig config)
        {
            id = config.id;
            locked = config.locked;
        }

        public void Unlock()
        {
            locked = false;
            UserData.Ins.DirtyAndNotify(ToString());
        }

        public override string ToString() => Config.name.ToString();
    }
}