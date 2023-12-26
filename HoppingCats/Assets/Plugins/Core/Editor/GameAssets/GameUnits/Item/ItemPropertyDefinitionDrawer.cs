using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace moonNest.editor
{
    public class ItemPropertyDefinitionDrawer
    {
        public TableDrawer<PropertyDefinition> tableDrawer;
        private ColDrawer<PropertyDefinition> distinctCol;
        private ItemDefinition itemDefinition;

        public ItemPropertyDefinitionDrawer()
        {
            distinctCol = new ColDrawer<PropertyDefinition>("Distinct", 60, ele => DrawDistinct(ele, 60), CheckDrawSavable);

            tableDrawer = new TableDrawer<PropertyDefinition>();
            tableDrawer.AddCol("Type", 100, ele =>
            {
                if(tableDrawer.drawingInlineAdd) ele.type = Draw.Enum(ele.type, 100);
                else Draw.Label(ele.type.ToString(), 100);
            });
            tableDrawer.AddCol("Value Type", 120, ele =>
            {
                if(tableDrawer.drawingInlineAdd) DrawValueType(ele, 120);
                else Draw.Label(ele.ValueType, 120);
            });
            tableDrawer.AddCol("Name", 120, ele =>
            {
                if(tableDrawer.drawingInlineAdd) ele.Name = Draw.Text(ele.Name, 120);
                else Draw.LabelBold(ele.Name, 120);
            });
            tableDrawer.AddCol("Savable", 60, ele => DrawSavable(ele, 60), CheckDrawSavable);
            tableDrawer.AddCol(distinctCol);
            tableDrawer.AddCol("Init Value", 80, ele => DrawInitValue(ele, 80), CheckDraw);
            tableDrawer.AddCol("Display Name", 120, ele => ele.DisplayName = Draw.Text(ele.DisplayName, 120));
            tableDrawer.AddCol("Display Icon", 120, ele => ele.DisplayIcon = Draw.Sprite(ele.DisplayIcon, 120));
            tableDrawer.AddCol("Col Width", 80, ele => ele.ColWidth = Draw.Int(ele.ColWidth, 80));
            tableDrawer.inlineAdd = true;
            tableDrawer.elementCreator = () => new PropertyDefinition();
            tableDrawer.drawControl = false;
            tableDrawer.alwayHidePage = true;
            tableDrawer.pageEnabled = false;
            tableDrawer.onElementAdded = OnElementAdded;
            tableDrawer.onElementDeleted = OnElementDeleted;
        }

        private void OnElementDeleted(PropertyDefinition propDef)
        {
            switch(propDef.type)
            {
                case PropertyType.Stat: OnStatDeleted(propDef.statDefinition); break;
                case PropertyType.Attribute: OnAttributeDeleted(propDef.attributeDefinition); break;
                case PropertyType.AssetReference: OnAssetReferenceDeleted(propDef.assetReferenceDefinition); break;
                case PropertyType.Reference: OnReferenceDeleted(propDef.referenceDefinition); break;
                case PropertyType.Enum: OnEnumDeleted(propDef.enumPropertyDefinition); break;
            }
        }

        private void OnElementAdded(PropertyDefinition propDef)
        {
            switch(propDef.type)
            {
                case PropertyType.Stat: OnStatAdded(propDef.statDefinition); break;
                case PropertyType.Attribute: OnAttributeAdded(propDef.attributeDefinition); break;
                case PropertyType.AssetReference: OnAssetReferenceAdded(propDef.assetReferenceDefinition); break;
                case PropertyType.Reference: OnReferenceAdded(propDef.referenceDefinition); break;
                case PropertyType.Enum: OnEnumAdded(propDef.enumPropertyDefinition); break;
            }
        }

        bool CheckDrawSavable(PropertyDefinition propDef)
        {
            return propDef.type != PropertyType.Attribute
                && propDef.type != PropertyType.AssetReference
                && propDef.type != PropertyType.Reference;
        }

        bool CheckDraw(PropertyDefinition propDef)
        {
            return propDef.type != PropertyType.Enum
                && propDef.type != PropertyType.AssetReference
                && propDef.type != PropertyType.Reference;
        }

        private void DrawDistinct(PropertyDefinition propDef, float maxWidth = -1)
        {
            switch(propDef.type)
            {
                case PropertyType.Stat:
                {
                    Draw.BeginChangeCheck();
                    propDef.statDefinition.distinct = Draw.Toggle(propDef.statDefinition.distinct, maxWidth);
                    if(Draw.EndChangeCheck())
                    {
                        propDef.statDefinition.savable = propDef.statDefinition.distinct;
                        foreach(var item in ItemAsset.Ins.FindByDefinition(itemDefinition))
                        {
                            item.stats.FindAll(s => s.name == propDef.statDefinition.name)
                                .ForEach(stat => stat.distinct = propDef.statDefinition.distinct);
                        }
                    }
                }
                break;
                case PropertyType.Enum:
                {
                    Draw.BeginChangeCheck();
                    propDef.enumPropertyDefinition.distinct = Draw.Toggle(propDef.enumPropertyDefinition.distinct, maxWidth);
                    if(Draw.EndChangeCheck())
                    {
                        propDef.enumPropertyDefinition.savable = propDef.enumPropertyDefinition.distinct;
                        foreach(var item in ItemAsset.Ins.FindByDefinition(itemDefinition))
                        {
                            item.enums.FindAll(s => s.Id == propDef.enumPropertyDefinition.id)
                                .ForEach(e => e.distinct = propDef.enumPropertyDefinition.distinct);
                        }
                    }
                }
                break;
            }
        }

        private void DrawSavable(PropertyDefinition propDef, float maxWidth = -1)
        {
            switch(propDef.type)
            {
                case PropertyType.Stat:
                {
                    Draw.BeginDisabledGroup(propDef.statDefinition.distinct);
                    propDef.statDefinition.savable = Draw.Toggle(propDef.statDefinition.savable, maxWidth);
                    Draw.EndDisabledGroup();
                }
                break;
                case PropertyType.Enum:
                {
                    Draw.BeginDisabledGroup(propDef.enumPropertyDefinition.distinct);
                    propDef.enumPropertyDefinition.savable = Draw.Toggle(propDef.enumPropertyDefinition.savable, maxWidth);
                    Draw.EndDisabledGroup();
                }
                break;
            }
        }

        private void DrawValueType(PropertyDefinition ele, float maxWidth = -1)
        {
            switch(ele.type)
            {
                case PropertyType.Stat:
                {
                    var type = Draw.Enum(ele.statDefinition.type, maxWidth);
                    if(ele.statDefinition.type != type)
                    {
                        if(type == StatValueType.Int) ele.statDefinition.initValue = 0;
                        else if(type == StatValueType.Float) ele.statDefinition.initValue = 0f;
                    }
                    ele.statDefinition.type = type;
                }
                break;
                case PropertyType.Attribute: ele.attributeDefinition.type = Draw.Enum(ele.attributeDefinition.type, maxWidth); break;
                case PropertyType.AssetReference: ele.assetReferenceDefinition.type = Draw.Enum(ele.assetReferenceDefinition.type, maxWidth); break;
                case PropertyType.Enum:
                {
                    Draw.BeginChangeCheck();
                    var enumPropDef = ele.enumPropertyDefinition;
                    enumPropDef.definitionId = Draw.IntPopup(enumPropDef.definitionId, GameDefinitionAsset.Ins.enums, "name", "id", maxWidth);
                    if(Draw.EndChangeCheck())
                    {
                        var enumDef = GameDefinitionAsset.Ins.FindEnum(enumPropDef.definitionId);
                        enumPropDef.name = enumDef.name;
                    }
                }
                break;
                case PropertyType.Reference:
                {
                    Draw.BeginChangeCheck();
                    var refDef = ele.referenceDefinition;
                    refDef.itemDefinitionId = Draw.IntPopup(refDef.itemDefinitionId, ItemAsset.Ins.definitions, "name", "id", maxWidth);
                    if(Draw.EndChangeCheck())
                    {
                        var itemDef = ItemAsset.Ins.FindDefinition(refDef.itemDefinitionId);
                        refDef.name = itemDef.name;
                    }
                }
                break;
                default: Draw.Label("", maxWidth); break;
            }
        }

        private void DrawInitValue(PropertyDefinition ele, float maxWidth = -1)
        {
            if(ele.type == PropertyType.Stat) ele.statDefinition.initValue = Draw.StatValue(ele.statDefinition.initValue, maxWidth);
            else if(ele.type == PropertyType.Attribute) ele.attributeDefinition.initValue = Draw.AttributeValue(ele.attributeDefinition.initValue, maxWidth);
            else Draw.Label("", maxWidth);
        }

        public void DoDraw(ItemDefinition itemDefinition)
        {
            this.itemDefinition = itemDefinition;
            distinctCol.enabled = itemDefinition.storageType == StorageType.Several;

            tableDrawer.minElementCount = itemDefinition.propertyDefinitions.FindAll(ele => !ele.deletable).Count;
            tableDrawer.DoDraw(itemDefinition.propertyDefinitions);

            PropertyDefinition willAdded = tableDrawer.WillAddedElement;
            bool disableAdd = string.IsNullOrEmpty(willAdded.Name);
            bool nameValidated = disableAdd || ValidateName(itemDefinition, willAdded);
            tableDrawer.disableInlineAddButton = nameValidated;

            if(nameValidated && willAdded.Name.Length > 0)
            {
                Draw.HelpBox($"{willAdded.Name} exists in is [\"name\", \"Name\", \"icon\", \"Icon\", \"class\", \"Class\"]", MessageType.Error);
            }
        }

        private static bool ValidateName(BaseObjectDefinition objectDefinition, PropertyDefinition willAdded)
        {
            return objectDefinition.propertyDefinitions.Find(a => a.Name == willAdded.Name.RemoveSpace()) != null
            || willAdded.Name.ToLower() == "name"
            || willAdded.Name.ToLower() == "icon"
            || willAdded.Name.ToLower() == "class";
        }

        private void OnStatAdded(StatDefinition stat)
        {
            stat.displayName = stat.name.Trim();
            stat.name = stat.name.RemoveSpace();

            List<ItemDetail> items = ItemAsset.Ins.FindByDefinition(itemDefinition);
            items.ForEach(item => item.AddStat(stat));
            Draw.SetDirty(ItemAsset.Ins);
        }

        private void OnStatDeleted(StatDefinition stat)
        {
            List<ItemDetail> items = ItemAsset.Ins.FindByDefinition(itemDefinition);
            items.ForEach(item => item.RemoveStat(stat));
            Draw.SetDirty(ItemAsset.Ins);

            itemDefinition.itemFilter.RemoveStatFilter(stat.name);
        }

        private void OnAttributeAdded(AttributeDefinition attribute)
        {
            attribute.displayName = attribute.name.Trim();
            attribute.name = attribute.name.RemoveSpace();

            switch(attribute.type)
            {
                case AttributeType.Sprite: attribute.initValue = (Sprite)default; break;
                case AttributeType.GameObject: attribute.initValue = (GameObject)default; break;
            }

            List<ItemDetail> items = ItemAsset.Ins.FindByDefinition(itemDefinition);
            items.ForEach(item => item.AddAttr(attribute));
            Draw.SetDirty(ItemAsset.Ins);
        }

        private void OnAttributeDeleted(AttributeDefinition attribute)
        {
            List<ItemDetail> items = ItemAsset.Ins.FindByDefinition(itemDefinition);
            items.ForEach(item => item.RemoveAttr(attribute));
            Draw.SetDirty(ItemAsset.Ins);
        }

        private void OnAssetReferenceAdded(AssetReferenceDefinition assetReferenceDefinition)
        {
            assetReferenceDefinition.displayName = assetReferenceDefinition.name.Trim();
            assetReferenceDefinition.name = assetReferenceDefinition.name.RemoveSpace();

            List<ItemDetail> items = ItemAsset.Ins.FindByDefinition(itemDefinition);
            items.ForEach(item => item.AddAssetReference(assetReferenceDefinition));
            Draw.SetDirty(ItemAsset.Ins);
        }

        private void OnAssetReferenceDeleted(AssetReferenceDefinition assetReferenceDefinition)
        {
            List<ItemDetail> items = ItemAsset.Ins.FindByDefinition(itemDefinition);
            items.ForEach(item => item.RemoveAssetReference(assetReferenceDefinition));
            Draw.SetDirty(ItemAsset.Ins);
        }

        private void OnReferenceAdded(ReferenceDefinition reference)
        {
            reference.name = reference.name.RemoveSpace();

            List<ItemDetail> items = ItemAsset.Ins.FindByDefinition(itemDefinition);
            items.ForEach(item => item.AddRef(reference));
            Draw.SetDirty(ItemAsset.Ins);
        }

        private void OnReferenceDeleted(ReferenceDefinition reference)
        {
            List<ItemDetail> items = ItemAsset.Ins.FindByDefinition(itemDefinition);
            items.ForEach(item => item.RemoveRef(reference));
            Draw.SetDirty(ItemAsset.Ins);
        }

        private void OnEnumAdded(EnumPropertyDefinition enumProp)
        {
            List<ItemDetail> items = ItemAsset.Ins.FindByDefinition(itemDefinition);
            items.ForEach(item => item.AddEnum(enumProp));
            Draw.SetDirty(ItemAsset.Ins);
        }

        private void OnEnumDeleted(EnumPropertyDefinition enumProp)
        {
            List<ItemDetail> items = ItemAsset.Ins.FindByDefinition(itemDefinition);
            items.ForEach(item => item.RemoveEnum(enumProp));
            Draw.SetDirty(ItemAsset.Ins);

            itemDefinition.itemFilter.RemoveEnumFilter(enumProp.id);
        }
    }
}