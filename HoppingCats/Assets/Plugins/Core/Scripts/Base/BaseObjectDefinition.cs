using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace moonNest
{
    [Serializable]
    public class BaseObjectDefinition : BaseDefinition, ISerializationCallbackReceiver
    {
        public List<StatDefinition> stats = new List<StatDefinition>();
        public List<AttributeDefinition> attributes = new List<AttributeDefinition>();
        public List<AssetReferenceDefinition> assetRefs = new List<AssetReferenceDefinition>();

        public List<ReferenceDefinition> refs = new List<ReferenceDefinition>();
        public List<EnumPropertyDefinition> enums = new List<EnumPropertyDefinition>();

        // editor
        [NonSerialized] public List<PropertyDefinition> propertyDefinitions = new List<PropertyDefinition>();

        public BaseObjectDefinition(string name) : base(name) { }

        protected BaseObjectDefinition() : base() { }

        public StatDefinition FindStat(int id) => stats.Find(_ => _.id == id);
        public StatDefinition FindStat(string name) => stats.Find(_ => _.name == name);
        public AttributeDefinition FindAttribute(int id) => attributes.Find(attr => attr.id == id);
        public AttributeDefinition FindAttribute(string name) => attributes.Find(attr => attr.name == name);
        public ReferenceDefinition FindRef(int id) => refs.Find(r => r.id == id);
        public ReferenceDefinition FindRef(string name) => refs.Find(r => r.name == name);
        public EnumPropertyDefinition FindEnum(int id) => enums.Find(e => e.id == id);
        public EnumPropertyDefinition FindEnum(string name) => enums.Find(e => e.name == name);

        public void DoDeserialize()
        {
            ((ISerializationCallbackReceiver)this).OnAfterDeserialize();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if(propertyDefinitions == null) propertyDefinitions = new List<PropertyDefinition>();
            propertyDefinitions.Clear();
            stats.ForEach(stat => propertyDefinitions.Add(new PropertyDefinition(stat)));
            attributes.ForEach(attr => propertyDefinitions.Add(new PropertyDefinition(attr)));
            refs.ForEach(@ref => propertyDefinitions.Add(new PropertyDefinition(@ref)));
            assetRefs.ForEach(assetRef => propertyDefinitions.Add(new PropertyDefinition(assetRef)));
            enums.ForEach(@enum => propertyDefinitions.Add(new PropertyDefinition(@enum)));
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            stats.Clear();
            attributes.Clear();
            refs.Clear();
            assetRefs.Clear();
            enums.Clear();
            propertyDefinitions.ForEach(pd =>
            {
                switch(pd.type)
                {
                    case PropertyType.Stat: stats.Add(pd.statDefinition); break;
                    case PropertyType.Attribute: attributes.Add(pd.attributeDefinition); break;
                    case PropertyType.Reference: refs.Add(pd.referenceDefinition); break;
                    case PropertyType.AssetReference: assetRefs.Add(pd.assetReferenceDefinition); break;
                    case PropertyType.Enum: enums.Add(pd.enumPropertyDefinition); break;
                }
            });
        }
    }

    public enum PropertyType { Stat, Attribute, Reference, Enum, AssetReference }

    public class PropertyDefinition
    {
        public PropertyType type;
        public StatDefinition statDefinition = new StatDefinition();
        public AttributeDefinition attributeDefinition = new AttributeDefinition();
        public ReferenceDefinition referenceDefinition = new ReferenceDefinition();
        public AssetReferenceDefinition assetReferenceDefinition = new AssetReferenceDefinition();
        public EnumPropertyDefinition enumPropertyDefinition = new EnumPropertyDefinition();
        public bool deletable = true;

        private bool _sync = false;
        public bool Sync
        {
            get { return _sync; }
            set
            {
                _sync = value;
                switch(type)
                {
                    case PropertyType.Stat: statDefinition.sync = _sync; break;
                    case PropertyType.Attribute: attributeDefinition.sync = _sync; break;
                }
            }
        }
        public bool drawSync = true;

        public int Id
        {
            get
            {
                switch(type)
                {
                    case PropertyType.Stat: return statDefinition.id;
                    case PropertyType.Attribute: return attributeDefinition.id;
                    case PropertyType.AssetReference: return assetReferenceDefinition.id;
                    case PropertyType.Reference: return referenceDefinition.id;
                    case PropertyType.Enum: return enumPropertyDefinition.id;
                    default: return -1;
                }
            }
        }

        public PropertyDefinition()
        {
            type = PropertyType.Stat;
            statDefinition.id = Random.Range(0, int.MaxValue);
            attributeDefinition.id = Random.Range(0, int.MaxValue);
            referenceDefinition.id = Random.Range(0, int.MaxValue);
            assetReferenceDefinition.id = Random.Range(0, int.MaxValue);
            enumPropertyDefinition.id = Random.Range(0, int.MaxValue);
        }

        public PropertyDefinition(StatDefinition statDefinition)
        {
            type = PropertyType.Stat;
            deletable = statDefinition.deletable;
            this.statDefinition = statDefinition;

            _sync = statDefinition.sync;
            drawSync = true;
        }

        public PropertyDefinition(AttributeDefinition attributeDefinition)
        {
            type = PropertyType.Attribute;
            deletable = attributeDefinition.deletable;
            this.attributeDefinition = attributeDefinition;

            _sync = attributeDefinition.sync;
            drawSync = attributeDefinition.name != UserPropertyDefinition.kUserId && attributeDefinition.type == AttributeType.String;
        }

        public PropertyDefinition(ReferenceDefinition referenceDefinition)
        {
            type = PropertyType.Reference;
            this.referenceDefinition = referenceDefinition;
        }

        public PropertyDefinition(AssetReferenceDefinition assetReferenceDefinition)
        {
            type = PropertyType.AssetReference;
            this.assetReferenceDefinition = assetReferenceDefinition;
        }

        public PropertyDefinition(EnumPropertyDefinition enumPropertyDefinition)
        {
            type = PropertyType.Enum;
            this.enumPropertyDefinition = enumPropertyDefinition;
        }

        public string Name
        {
            get
            {
                switch(type)
                {
                    case PropertyType.Stat: return statDefinition.name;
                    case PropertyType.Attribute: return attributeDefinition.name;
                    case PropertyType.Reference: return referenceDefinition.name;
                    case PropertyType.AssetReference: return assetReferenceDefinition.name;
                    case PropertyType.Enum: return enumPropertyDefinition.name;
                }
                return "";
            }

            set
            {
                switch(type)
                {
                    case PropertyType.Stat: statDefinition.name = value; break;
                    case PropertyType.Attribute: attributeDefinition.name = value; break;
                    case PropertyType.Reference: referenceDefinition.name = value; break;
                    case PropertyType.AssetReference: assetReferenceDefinition.name = value; break;
                    case PropertyType.Enum: enumPropertyDefinition.name = value; break;
                }
            }
        }

        public string ValueType
        {
            get
            {
                switch(type)
                {
                    case PropertyType.Stat: return statDefinition.type.ToString();
                    case PropertyType.Attribute: return attributeDefinition.type.ToString();
                    case PropertyType.AssetReference: return assetReferenceDefinition.type.ToString();
                    case PropertyType.Reference: return "Item";
                    case PropertyType.Enum: return "Enum";
                }
                return "";
            }
        }

        public string DisplayName
        {
            get
            {
                switch(type)
                {
                    case PropertyType.Stat: return statDefinition.displayName;
                    case PropertyType.Attribute: return attributeDefinition.displayName;
                    case PropertyType.Reference: return "Item Id";
                    case PropertyType.AssetReference: return assetReferenceDefinition.displayName;
                    case PropertyType.Enum: return "";
                }
                return "";
            }

            set
            {
                switch(type)
                {
                    case PropertyType.Stat: statDefinition.displayName = value; break;
                    case PropertyType.Attribute: attributeDefinition.displayName = value; break;
                    case PropertyType.AssetReference: assetReferenceDefinition.displayName = value; break;
                }
            }
        }

        public Sprite DisplayIcon
        {
            get
            {
                switch(type)
                {
                    case PropertyType.Stat: return statDefinition.displayIcon;
                    case PropertyType.Attribute: return attributeDefinition.displayIcon;
                    case PropertyType.AssetReference: return assetReferenceDefinition.displayIcon;
                }
                return null;
            }

            set
            {
                switch(type)
                {
                    case PropertyType.Stat: statDefinition.displayIcon = value; break;
                    case PropertyType.Attribute: attributeDefinition.displayIcon = value; break;
                    case PropertyType.AssetReference: assetReferenceDefinition.displayIcon = value; break;
                }
            }
        }

        public int ColWidth
        {
            get
            {
                switch(type)
                {
                    case PropertyType.Stat: return statDefinition.colWidth;
                    case PropertyType.Attribute: return attributeDefinition.colWidth;
                    case PropertyType.Reference: return referenceDefinition.colWidth;
                    case PropertyType.AssetReference: return assetReferenceDefinition.colWidth;
                    case PropertyType.Enum: return enumPropertyDefinition.colWidth;
                }
                return 80;
            }

            set
            {
                switch(type)
                {
                    case PropertyType.Stat: statDefinition.colWidth = value; break;
                    case PropertyType.Attribute: attributeDefinition.colWidth = value; break;
                    case PropertyType.Reference: referenceDefinition.colWidth = value; break;
                    case PropertyType.AssetReference: assetReferenceDefinition.colWidth = value; break;
                    case PropertyType.Enum: enumPropertyDefinition.colWidth = value; break;
                }
            }
        }
    }
}