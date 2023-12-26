using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using moonNest;

public partial class EnumTab : TabContent
{
    private ListCardDrawer<EnumDefinition> cardDrawer;
    private TableDrawer<string> stringTable;

    public EnumTab()
    {
        cardDrawer = new ListCardDrawer<EnumDefinition>();
        cardDrawer.onDrawElement = OnDrawEnum;
        cardDrawer.onDrawEditElement = OnDrawEditEnum;
        cardDrawer.elementCreator = () => new EnumDefinition("");
        cardDrawer.EditBeforeAdd = true;
        cardDrawer.onElementAdded = OnEnumAdded;

        stringTable = new TableDrawer<string>("Enums");
        stringTable.AddCol("Type", 200, ele =>
        {
            if (stringTable.drawingInlineAdd) return Draw.Text(ele, 200);
            else Draw.LabelBold(ele, 200);
            return ele.RemoveSpace();
        });
        stringTable.inlineAdd = true;
        stringTable.drawHeader = false;
        stringTable.drawIndex = false;
        stringTable.drawOrder = false;
        stringTable.elementCreator = () => "";
        stringTable.drawControl = false;
    }

    private void OnEnumAdded(EnumDefinition enumDefinition)
    {
        enumDefinition.name = enumDefinition.name.RemoveSpace();
    }

    private bool OnDrawEditEnum(EnumDefinition enumDefinition)
    {
        Draw.LabelBoldBox("Enter Enum Name");

        enumDefinition.name = Draw.TextField("Name", enumDefinition.name, 120);

        string finalName = enumDefinition.name.RemoveSpace();
        return finalName.Length > 2 && GameDefinitionAsset.Ins.enums.Find(@enum => @enum.name == finalName) == null;
    }

    private void OnDrawEnum(EnumDefinition enumDefinition)
    {
        Draw.LabelBoldBox(enumDefinition.name, Color.blue);

        Draw.Space();
        stringTable.DoDraw(enumDefinition.stringList);

        Draw.SpaceAndLabelBold("Editor");
        enumDefinition.colWidth = Draw.IntField("Col Width", enumDefinition.colWidth, 100);
    }

    public override void DoDraw()
    {
        Draw.Space();
        cardDrawer.MaxWidth = Screen.width - 250;
        cardDrawer.DoDraw(GameDefinitionAsset.Ins.enums);
        Draw.Space();
        
        if (Draw.Button("Generate Class", Color.magenta, Color.white, 150))
        {
            GenerateClass(GameDefinitionAsset.Ins.enums);
            AssetDatabase.Refresh();
        }
    }

    private void GenerateClass(List<EnumDefinition> enumDefs)
    {
        string directory = $"{GlobalConfig.Ins.GenPath}/Enums";
        if(!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        foreach (var enumDef in enumDefs)
        {
            string filePath = $"{directory}/{enumDef.name}.cs";
            StreamWriter writer = new StreamWriter(filePath, false);
            writer.WriteLine("");
            writer.WriteLine("// This file is auto generated from Enum Defintion Editor");
            writer.WriteLine("// Manually modified can caused bug");
            writer.WriteLine($"public enum {enumDef.name} : byte");
            writer.WriteLine("{");

            enumDef.stringList.ForEach(s => writer.WriteLine($"{s},"));

            writer.WriteLine("}");
            writer.Flush();
            writer.Close();
        }
    }
}