using UnityEditor;
using UnityEngine;
using moonNest;

public partial class CurrencyTab : TabContent
{
    private readonly TableDrawer<CurrencyDefinition> currencyTable;
    private readonly TableDrawer<CurrencyExchange> exchangeTable;

    public CurrencyTab()
    {
        bool editable = false;
        currencyTable = new TableDrawer<CurrencyDefinition>();
        currencyTable.AddCol("Name", 80, ele => ele.name = Draw.Text(ele.name, 80), editable);
        currencyTable.AddCol("Type", 60, ele => ele.type = Draw.Enum(ele.type, 60), editable);
        currencyTable.AddCol("Numeric", 60, ele => ele.numericType = Draw.Enum(ele.numericType, 60), editable);
        currencyTable.AddCol("Icon", 80, ele => ele.icon = Draw.Sprite(ele.icon, 80));
        currencyTable.AddCol("Big Icon", 80, ele => ele.bigIcon = Draw.Sprite(ele.bigIcon, 80));
        currencyTable.AddCol("Init Value", 60, ele => ele.initialValue = Draw.Int(ele.initialValue, 60));
        currencyTable.AddCol("Max Value", 100, ele => ele.maxValue = Draw.Double(ele.maxValue, 100));
        currencyTable.AddCol("Description", 150, ele => ele.description = Draw.Text(ele.description, 150));
        currencyTable.AddCol("Sync", 40, ele => ele.sync = Draw.Toggle(ele.sync, 40));
        currencyTable.AddCol("Grow", 40, ele => ele.growOverTime = Draw.Toggle(ele.growOverTime, 40));
        currencyTable.AddCol("G.Value", 60, ele => ele.growValue = Draw.Int(ele.growValue, 60), ele => ele.growOverTime);
        currencyTable.AddCol("G.Max Value", 80, ele => ele.growMaxValue = Draw.Int(ele.growMaxValue, 80), ele => ele.growOverTime);
        currencyTable.AddCol("G.Seconds", 80, ele => ele.growPeriod = Draw.Int(ele.growPeriod, 80), ele => ele.growOverTime);

        currencyTable.elementCreator = () => new CurrencyDefinition("New Currency");
        currencyTable.onElementAdded = ele => ele.name = ele.name.RemoveSpace();
        currencyTable.askBeforeDelete = ele => $"Consider before delete Currency {ele.name} !!!";
        currencyTable.inlineAdd = true;
        currencyTable.drawControl = false;

        exchangeTable = new TableDrawer<CurrencyExchange>();
        exchangeTable.AddCol("Source", 100, ele => ele.srcCurrency = Draw.IntPopup(ele.srcCurrency, GameDefinitionAsset.Ins.currencies, "name", "id", 100));
        exchangeTable.AddCol("Value", 80, ele => ele.srcValue = Draw.Int(ele.srcValue, 80));
        exchangeTable.AddCol("-", 150, ele => Draw.LabelBold("----------------------->", 150));
        exchangeTable.AddCol("Dest", 100, ele => ele.destCurrency = Draw.IntPopup(ele.destCurrency, GameDefinitionAsset.Ins.currencies, "name", "id", 100));
        exchangeTable.AddCol("Value", 80, ele => ele.destValue = Draw.Int(ele.destValue, 80));
        exchangeTable.elementCreator = () => new CurrencyExchange();
    }

    public override void DoDraw()
    {
        currencyTable.DoDraw(GameDefinitionAsset.Ins.currencies);
        Draw.Space(12);
        if(Draw.Button("Generate class", Color.magenta, Color.white, 150))
        {
            GenerateClass(GameDefinitionAsset.Ins.currencies);
            AssetDatabase.Refresh();
        }

        Draw.Space(24);
        Draw.LabelBold("Currency Exchange");
        Draw.SeparateHLine();
        exchangeTable.DoDraw(GameDefinitionAsset.Ins.currencyExchanges);
    }
}