using System;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class UnlockConditionDetail : BaseData
    {
        public string displayName;
        public int groupId;
        public int statId;
        public int requireValue;

        public UnlockConditionDetail(string name) : base(name) { }

        public bool Satify => UserData.Stat(statId).AsInt >= requireValue;
    }

    [Serializable]
    public class UnlockContentDetail : BaseData
    {
        public UnlockContentType type;
        public Sprite icon;
        public int conditionId = -1;
        public int itemDefinitionId = -1;
        public int contentId = -1;

        public string UnlockConditionName => UnlockCondition ? UnlockCondition.name : "No Condition";
        public string UnlockConditionDisplayName => UnlockCondition ? UnlockCondition.displayName : "No Condition";

        [NonSerialized] private UnlockConditionDetail _unlockCondition = null;
        public UnlockConditionDetail UnlockCondition
        {
            get
            {
                if(!_unlockCondition && conditionId != -1)
                    _unlockCondition = UnlockContentAsset.Ins.FindCondition(conditionId);
                return _unlockCondition;
            }

            set
            {
                _unlockCondition = value;
            }
        }

        [NonSerialized] private Sprite _icon = null;
        public Sprite Icon
        {
            get
            {
                if(icon) return icon;

                if(!_icon)
                {
                    if(type == UnlockContentType.Item)
                    {
                        ItemDetail item = ItemAsset.Ins.Find(contentId);
                        _icon = item ? item.icon : null;
                    }
                    else
                    {
                        FeatureConfig feature = GameDefinitionAsset.Ins.FindFeature(contentId);
                        _icon = feature ? feature.icon : null;
                    }
                }

                return _icon;
            }
        }

        [NonSerialized] private GameObject _prefab = null;
        public GameObject Prefab
        {
            get
            {
                if(!_prefab)
                {
                    if(type == UnlockContentType.Item)
                    {
                        ItemDetail item = ItemAsset.Ins.Find(contentId);
                        _prefab = item ? item.UIPrefab : null;
                    }
                    else
                    {
                        FeatureConfig feature = GameDefinitionAsset.Ins.FindFeature(contentId);
                        _prefab = feature ? feature.uiPrefab : null;
                    }
                }

                return _prefab;
            }
        }

        [NonSerialized] private string _displayName = null;
        public string DisplayName
        {
            get
            {
                if(_displayName == null)
                {
                    if(type == UnlockContentType.Item)
                    {
                        ItemDetail item = ItemAsset.Ins.Find(contentId);
                        _displayName = item ? item.displayName : "";
                    }
                    else
                    {
                        FeatureConfig feature = GameDefinitionAsset.Ins.FindFeature(contentId);
                        _displayName = feature ? feature.displayName : "";
                    }
                }

                return _displayName;
            }
        }

        public UnlockContentDetail(string name) : base(name) { }
    }

    public enum UnlockContentType { None = -1, Item, Feature }
}