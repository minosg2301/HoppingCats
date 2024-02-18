using System;
using System.Collections.Generic;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class ItemDetail : BaseDetail<ItemDefinition>
    {
        public string displayName;
        public Sprite icon;
        public int initAmount = 0;
        public int capacity = -1;
        public bool active = true;
        public int unlockContentId = -1;

        public List<StatDetail> stats = new List<StatDetail>();
        public List<ReferenceDetail> refs = new List<ReferenceDetail>();
        public List<AttributeDetail> attributes = new List<AttributeDetail>();
        public List<AssetReferenceDetail> assetReferences = new List<AssetReferenceDetail>();
        public List<EnumDetail> enums = new List<EnumDetail>();
        public FlexibleAsset<ScriptableObject> extendConfig;

        [NonSerialized] Dictionary<int, object> enumMap = new Dictionary<int, object>();

        [NonSerialized] private UnlockContentDetail _unlockContent = null;
        public UnlockContentDetail UnlockContent
        {
            get
            {
                if(!_unlockContent && Definition.unlockedByProgress)
                    _unlockContent = UnlockContentAsset.Ins.FindContent(unlockContentId);
                return _unlockContent;
            }

            set
            {
                _unlockContent = value;
                unlockContentId = _unlockContent ? _unlockContent.id : -1;
            }
        }

        public ItemDetail(ItemDefinition definition) : base(definition)
        {
            initAmount = definition.init;
            capacity = definition.capacity;
            definition.stats.ForEach(stat => AddStat(stat));
            definition.attributes.ForEach(attr => AddAttr(attr));
            definition.refs.ForEach(@ref => AddRef(@ref));
            definition.assetRefs.ForEach(assetRef => AddAssetReference(assetRef));
            definition.enums.ForEach(@enum => AddEnum(@enum));
        }

        protected override ItemDefinition GetDefinition(int definitionId) => ItemAsset.Ins.FindDefinition(definitionId);

        public StatDetail Stat(int id) => stats.Find(_ => _.id == id);
        public StatDetail Stat(StatDefinition statDef) => stats.Find(_ => _.id == statDef.id);
        public void AddStat(StatDefinition statDef) => stats.Add(new StatDetail(statDef));
        public void RemoveStat(StatDefinition statDef) => stats.Remove(Stat(statDef));

        public AttributeDetail Attr(int id) => attributes.Find(_ => _.id == id);
        public AttributeDetail Attr(AttributeDefinition attrDef) => attributes.Find(_ => _.id == attrDef.id);
        public void AddAttr(AttributeDefinition attrDef) => attributes.Add(new AttributeDetail(attrDef));
        public void RemoveAttr(AttributeDefinition attrDef) => attributes.Remove(Attr(attrDef));

        public AssetReferenceDetail AssetReference(int id) => assetReferences.Find(_ => _.id == id);
        public AssetReferenceDetail AssetReference(AssetReferenceDefinition assetRef) => assetReferences.Find(_ => _.id == assetRef.id);
        public void AddAssetReference(AssetReferenceDefinition assetRef) => assetReferences.Add(new AssetReferenceDetail(assetRef));
        public void RemoveAssetReference(AssetReferenceDefinition assetRef) => assetReferences.Remove(AssetReference(assetRef));

        public ReferenceDetail Reference(int id) => refs.Find(_ => _.id == id);
        public ReferenceDetail Reference(ReferenceDefinition referenceDef) => refs.Find(_ => _.id == referenceDef.id);
        public void RemoveRef(ReferenceDefinition referenceDef) => refs.Remove(Reference(referenceDef));
        public void AddRef(ReferenceDefinition referenceDef) => refs.Add(new ReferenceDetail(referenceDef));

        public EnumDetail EnumDetail(int id) => enums.Find(e => e.Id == id);
        public void RemoveEnum(EnumPropertyDefinition enumPropDef) => enums.Remove(EnumDetail(enumPropDef.id));
        public void AddEnum(EnumPropertyDefinition enumPropDef) => enums.Add(new EnumDetail(enumPropDef));

        public TEnum Type<TEnum>(int enumId) where TEnum : struct
        {
            if(enumMap == null) enumMap = new Dictionary<int, object>();

            if(enumMap.TryGetValue(enumId, out var t)) return (TEnum)t;

            EnumDetail detail = enums.Find(e => e.Id == enumId);
            if(detail != null && Enum.TryParse(detail.name, out TEnum v))
            {
                enumMap[enumId] = v;
                return v;
            }
            return default;
        }

        public override string ToString() => name;

        #region uiprefabs runtime
        [NonSerialized] private GameObject _uiPrefab;
        public GameObject UIPrefab
        {
            get
            {
                if(!Definition.uiPrefab) return null;

                if(!_uiPrefab)
                {
                    EnsurePrefabContainer();

                    var newUIPrefab = UnityEngine.Object.Instantiate(Definition.uiPrefab, uiPrefabContainer.transform);
                    newUIPrefab.gameObject.name = name;
                    newUIPrefab.SetItemDetail(this);
                    _uiPrefab = newUIPrefab.gameObject;

                }
                return _uiPrefab;
            }
        }

        private static GameObject uiPrefabContainer;
        private static void EnsurePrefabContainer()
        {
            if(uiPrefabContainer) return;
            uiPrefabContainer = new GameObject("UI Item Prefabs");
            UnityEngine.Object.DontDestroyOnLoad(uiPrefabContainer);
        }
        #endregion
    }
}