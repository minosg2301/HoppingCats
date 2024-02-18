using UnityEditor;
using moonNest;

[CustomEditor(typeof(CheatCurrencyBtn))]
public class CheatCurrencyBtnEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Draw.Space();
        Draw.BeginChangeCheck();

        var cheat = target as CheatCurrencyBtn;
        cheat.currencyName = Draw.StringPopupField("Currency", "Select currency", cheat.currencyName, GameDefinitionAsset.Ins.currencies, "name", 120);
        cheat.value = Draw.IntField("Value", cheat.value);

        if (Draw.EndChangeCheck())
        {
            target.name = "CheatCurrencyBtn - " + cheat.currencyName;
            EditorUtility.SetDirty(target);
        }
    }
}
