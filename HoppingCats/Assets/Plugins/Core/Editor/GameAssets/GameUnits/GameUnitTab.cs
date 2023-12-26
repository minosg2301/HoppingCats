using UnityEditor;
using UnityEngine;

namespace moonNest.editor
{
    public class GameUnitTab : TabContent
    {
        public TabContainer tabContainer;

        public GameUnitTab()
        {
            tabContainer = new TabContainer();
            tabContainer.AddTab("Currency", new CurrencyTab());
            tabContainer.AddTab("Location", new LocationTab());
            tabContainer.AddTab("Action", new ActionTab());
            tabContainer.AddTab("Enums", new EnumTab());
            tabContainer.AddTab("Game Event", new GameEventTab());
            tabContainer.HeaderType = HeaderType.Vertical;
        }

        public override void DoDraw()
        {
            Undo.RecordObject(GameDefinitionAsset.Ins, "Game Defines");
            tabContainer.DoDraw();
            if(GUI.changed) EditorUtility.SetDirty(GameDefinitionAsset.Ins);
        }
    }
}