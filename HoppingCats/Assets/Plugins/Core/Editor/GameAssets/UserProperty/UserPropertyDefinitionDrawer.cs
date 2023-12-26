using UnityEditor;
using UnityEngine;
using moonNest;

public class UserPropertyDefinitionDrawer
{
    public TableDrawer<PropertyDefinition> tableDrawer;

    public UserPropertyDefinitionDrawer()
    {
        tableDrawer = new TableDrawer<PropertyDefinition>();
        tableDrawer.AddCol("ID", 80, ele =>
        {
            if(!tableDrawer.drawingInlineAdd) Draw.Label(ele.Id.ToString(), 80);
        });
        tableDrawer.AddCol("Type", 60, ele =>
        {
            if(tableDrawer.drawingInlineAdd) ele.type = Draw.Enum(ele.type, 60);
            else Draw.Label(ele.type.ToString(), 60);
        });
        tableDrawer.AddCol("Value Type", 80, ele =>
        {
            if(tableDrawer.drawingInlineAdd) DrawValueType(ele, 80);
            else Draw.Label(ele.ValueType, 100);
        }, SkipDraw);
        tableDrawer.AddCol("Name", 120, ele =>
        {
            if(tableDrawer.drawingInlineAdd) ele.Name = Draw.Text(ele.Name, 120);
            else Draw.LabelBold(ele.Name, 120);
        }, SkipDraw);
        tableDrawer.AddCol("Protected", 80, ele => DrawProtectedStat(ele, 80), ele => ele.type == PropertyType.Stat, false);
        tableDrawer.AddCol("Layer", 50, ele => DrawStatLayer(ele, 50));
        tableDrawer.AddCol("Progress", 100, ele => DrawProgressStat(ele, 100));
        tableDrawer.AddCol("Unlock", 60, ele => DrawStatUnlock(ele, 60));
        tableDrawer.AddCol("Sync", 50, ele => DrawSync(ele, 50));
        tableDrawer.AddCol("Init Value", 80, ele => DrawInitValue(ele, 80), SkipDraw);
        tableDrawer.AddCol("Display Name", 120, ele => ele.DisplayName = Draw.Text(ele.DisplayName, 120), SkipDraw);
        tableDrawer.AddCol("Display Icon", 120, ele => ele.DisplayIcon = Draw.Sprite(ele.DisplayIcon, 120), SkipDraw);
        tableDrawer.checkDeletable = ele => ele.deletable;
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
        if(propDef.type == PropertyType.Stat)
        {
            var stat = propDef.statDefinition;
            if(stat.layer) LayerAsset.Ins.DeleteGroup(stat);
            if(stat.progress) StatProgressAsset.Ins.DeleteGroup(stat);
        }
    }

    private void OnElementAdded(PropertyDefinition propDef)
    {
        if(propDef.type == PropertyType.Stat)
        {
            StatDefinition stat = propDef.statDefinition;
            stat.name = stat.name.RemoveSpace();
            stat.displayName = stat.displayName.Length > 0 ? stat.displayName : stat.name;
            if(stat.type == StatValueType.Float) stat.initValue = 0f;

            // update Layer Asset
            if(stat.layer) LayerAsset.Ins.CreateGroup(stat);

            // update segment
            if(stat.progress) StatProgressAsset.Ins.CreateGroup(stat);

            // update unlock content
            if(stat.lockContent) UnlockContentAsset.Ins.AddGroup(new UnlockConditionGroup(stat));
        }
        else if(propDef.type == PropertyType.Attribute)
        {
            AttributeDefinition attr = propDef.attributeDefinition;
            attr.name = attr.name.RemoveSpace();
        }
    }

    private bool SkipDraw(PropertyDefinition propDef)
    {
        return propDef.type != PropertyType.Enum && propDef.type != PropertyType.Reference;
    }

    private void DrawProtectedStat(PropertyDefinition propDef, float maxWidth = -1)
    {
        if(propDef.type == PropertyType.Stat)
        {
            if(propDef.statDefinition.type == StatValueType.Int)
            {
                propDef.statDefinition.safe = Draw.Toggle(propDef.statDefinition.safe, maxWidth);
                return;
            }
        }
        Draw.Label(" ", maxWidth);
    }

    private void DrawStatLayer(PropertyDefinition propDef, float maxWidth = -1)
    {
        if(propDef.type == PropertyType.Stat)
        {
            StatDefinition statDef = propDef.statDefinition;
            if(statDef.type == StatValueType.Int)
            {
                Draw.BeginChangeCheck();
                statDef.layer = Draw.Toggle(statDef.layer, maxWidth);
                if(Draw.EndChangeCheck())
                {
                    if(statDef.layer) LayerAsset.Ins.CreateGroup(statDef);
                    else LayerAsset.Ins.DeleteGroup(statDef);
                }

                return;
            }
        }

        Draw.Label(" ", maxWidth);
    }

    private void DrawProgressStat(PropertyDefinition propDef, float maxWidth = -1)
    {
        if(propDef.type == PropertyType.Stat)
        {
            StatDefinition statDef = propDef.statDefinition;
            if(statDef.type == StatValueType.Int)
            {
                Draw.BeginHorizontal();
                Draw.BeginChangeCheck();
                statDef.progress = Draw.Toggle(statDef.progress, statDef.progress ? 20 : maxWidth);
                if(!tableDrawer.drawingInlineAdd && Draw.EndChangeCheck())
                {
                    if(statDef.progress) StatProgressAsset.Ins.CreateGroup(statDef);
                    else StatProgressAsset.Ins.DeleteGroup(statDef);
                }
                if(statDef.progress)
                {
                    Draw.BeginChangeCheck();
                    statDef.progressType = Draw.Enum(statDef.progressType, maxWidth - 20);
                    if(!tableDrawer.drawingInlineAdd && Draw.EndChangeCheck())
                    {
                        StatProgressAsset.Ins.UpdateProgressType(statDef);
                    }
                }
                Draw.EndHorizontal();
                return;
            }
        }

        Draw.Label(" ", maxWidth);
    }


    private void DrawSync(PropertyDefinition propDef, float maxWidth = -1)
    {
        if(propDef.drawSync) propDef.Sync = Draw.Toggle(propDef.Sync, 50);
        else Draw.Label(" ", maxWidth);
    }

    private void DrawStatUnlock(PropertyDefinition propDef, float maxWidth = -1)
    {
        if(propDef.type == PropertyType.Stat)
        {
            StatDefinition statDef = propDef.statDefinition;
            if(statDef.type == StatValueType.Int)
            {
                Draw.BeginChangeCheck();
                statDef.lockContent = Draw.Toggle(statDef.lockContent, maxWidth);
                if(Draw.EndChangeCheck())
                {
                    if(statDef.lockContent) UnlockContentAsset.Ins.AddGroup(new UnlockConditionGroup(statDef));
                    else UnlockContentAsset.Ins.RemoveGroup(statDef.id);
                }
                return;
            }
        }

        Draw.Label(" ", maxWidth);
    }

    private void DrawValueType(PropertyDefinition ele, float maxWidth = -1)
    {
        switch(ele.type)
        {
            case PropertyType.Stat:
            {
                StatValueType type = Draw.Enum(ele.statDefinition.type, maxWidth);
                if(ele.statDefinition.type != type)
                {
                    if(type == StatValueType.Int) ele.statDefinition.initValue = 0;
                    else if(type == StatValueType.Float) ele.statDefinition.initValue = 0f;
                }
                ele.statDefinition.type = type;
            }
            break;
            case PropertyType.Attribute:
            {
                AttributeType type = Draw.Enum(ele.attributeDefinition.type, maxWidth);
                if(ele.attributeDefinition.type != type)
                {
                    switch(type)
                    {
                        case AttributeType.String: ele.attributeDefinition.initValue = ""; break;
                        case AttributeType.Sprite: ele.attributeDefinition.initValue = default(Sprite); break;
                        case AttributeType.GameObject: ele.attributeDefinition.initValue = default(GameObject); break;
                    }
                }
                ele.attributeDefinition.type = type;
            }
            break;
            default: Draw.Label("", maxWidth); break;
        }
    }

    private void DrawInitValue(PropertyDefinition ele, float maxWidth = -1)
    {
        if(ele.type == PropertyType.Stat)
            ele.statDefinition.initValue = Draw.StatValue(ele.statDefinition.initValue, maxWidth);
        else if(ele.type == PropertyType.Attribute)
            ele.attributeDefinition.initValue = Draw.AttributeValue(ele.attributeDefinition.initValue, maxWidth);
        else
            Draw.Label("", maxWidth);
    }

    public void DoDraw(BaseObjectDefinition objectDefinition)
    {
        Draw.BeginChangeCheck();
        tableDrawer.DoDraw(objectDefinition.propertyDefinitions);
        if(Draw.EndChangeCheck())
        {
            EditorUtility.SetDirty(UserPropertyAsset.Ins);
        }

        PropertyDefinition willAdded = tableDrawer.WillAddedElement;
        bool disableAdd = string.IsNullOrEmpty(willAdded.Name)
                || willAdded.type == PropertyType.Reference
                || willAdded.type == PropertyType.Enum;
        bool nameValidated = ValidateName(objectDefinition, willAdded);
        tableDrawer.disableInlineAddButton = disableAdd || nameValidated;

        if(nameValidated)
        {
            if(willAdded.type == PropertyType.Stat)
                Draw.HelpBox($"{willAdded.Name} exists in Stats list or is (\"name\", \"userid\", \"language\")", MessageType.Error);
            else if(willAdded.type == PropertyType.Attribute)
                Draw.HelpBox($"{willAdded.Name} exists in Attributes list or is (\"name\", \"userid\", \"language\")", MessageType.Error);
        }

        if(willAdded.type == PropertyType.Reference) Draw.HelpBox($"Reference is not available", MessageType.Error);
        else if(willAdded.type == PropertyType.Enum) Draw.HelpBox($"Enum is not available", MessageType.Error);
    }

    private static bool ValidateName(BaseObjectDefinition objectDefinition, PropertyDefinition willAdded)
    {
        return objectDefinition.propertyDefinitions.Find(a => a.Name.ToLower() == willAdded.Name.ToLower()) != null
            || willAdded.Name.Trim().ToLower() == "name"
            || willAdded.Name.Trim().ToLower() == "userid"
            || willAdded.Name.Trim().ToLower() == "language";
    }
}