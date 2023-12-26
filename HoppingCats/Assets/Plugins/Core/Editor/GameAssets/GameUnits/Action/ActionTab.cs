using UnityEditor;
using UnityEngine;

namespace moonNest
{
    public partial class ActionTab : TabContent
    {
        private TableDrawer<ActionDefinition> tableDrawer;

        public ActionTab()
        {
            tableDrawer = new TableDrawer<ActionDefinition>("Action Definition");
            tableDrawer.AddCol("Name", 250, ele =>
            {
                if (tableDrawer.drawingInlineAdd) ele.name = Draw.Text(ele.name, 250);
                else Draw.LabelBold(ele.name, 250);
            });
            tableDrawer.AddCol("Param 1", 120, ele => DrawParam(ele, 0), false);
            tableDrawer.AddCol("Param 2", 120, ele => DrawParam(ele, 1), false);
            tableDrawer.AddCol("Param 3", 120, ele => DrawParam(ele, 2), false);
            tableDrawer.checkDeletable = ele => ele.deletable;
            tableDrawer.elementCreator = () => new ActionDefinition("New Action");
            tableDrawer.inlineAdd = true;
        }

        private void DrawParam(ActionDefinition ele, int i)
        {
            if (tableDrawer.drawingInlineAdd)
            {
                ele.paramTypes[i] = Draw.Enum(ele.paramTypes[i], 120);
                if (ele.paramTypes[i] == ActionParamType.Enum)
                {
                    ele.enumTypes[i] = Draw.IntPopup(ele.enumTypes[i], GameDefinitionAsset.Ins.enums, "name", "id", 120);
                }
            }
            else
            {
                if (ele.paramTypes[i] == ActionParamType.Enum)
                {
                    ele.enumTypes[i] = Draw.IntPopup(ele.enumTypes[i], GameDefinitionAsset.Ins.enums, "name", "id", 120);
                }
                else ele.paramTypes[i] = Draw.Enum(ele.paramTypes[i], 120);
            }
        }

        public override void DoDraw()
        {
            Draw.Space();
            tableDrawer.DoDraw(GameDefinitionAsset.Ins.actions);
            if (tableDrawer.WillAddedElement != null)
            {
                bool categoryExists = GameDefinitionAsset.Ins.actions.Find(action => action.name == tableDrawer.WillAddedElement.name);
                tableDrawer.disableInlineAddButton = tableDrawer.WillAddedElement.name.Length == 0 || categoryExists;
                if (categoryExists)
                {
                    Draw.HelpBox($"{tableDrawer.WillAddedElement.name} exists in list", MessageType.Error);
                }
            }

            Draw.Space(12);
            if (Draw.Button("Generate class", Color.magenta, Color.white, 150))
            {
                GenerateClass(GameDefinitionAsset.Ins.actions);
                AssetDatabase.Refresh();
            }
        }
    }
}