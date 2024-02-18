using System.Collections.Generic;
using System.IO;

namespace moonNest
{
    public partial class ActionTab
    {
        private void GenerateClass(List<ActionDefinition> actions)
        {
            string filePath = GlobalConfig.Ins.GenPath + "/GameActions.cs";
            StreamWriter writer = new StreamWriter(filePath, false);
            writer.WriteLine("");
            writer.WriteLine("// This file is auto generated from Action Defintion Editor");
            writer.WriteLine("// Manually modified can caused bug");
            writer.WriteLine("using vgame;\n");
            writer.WriteLine("public class GameActions\n{");

            actions.ForEach(_ => WriteAction(writer, _));

            writer.WriteLine("}");
            writer.Flush();
            writer.Close();
        }

        private void WriteAction(StreamWriter writer, ActionDefinition action)
        {
            string methodName = action.name.ToTitleCase().RemoveSpace();
            string param = $"k{methodName}";
            string arg = "";

            action.paramTypes.ForEach((type, i) =>
            {
                if (type == ActionParamType.Enum)
                {
                    string argName = GetArgName(type);
                    arg += $"{GetEnumName(action, i)} {argName}, ";
                    param += $", (int){argName}";
                }

                else if(type != ActionParamType.None)
                {
                    string argName = GetArgName(type);
                    arg += $"int {argName}, ";
                    param += $", {argName}";
                }
            });

            if(arg.Length > 0) arg = arg.Substring(0, arg.Length - 2);

            writer.WriteLine($"\tpublic static readonly int k{methodName} = {action.id};");
            writer.WriteLine($"\tpublic static ActionData {methodName}({arg}) => new ActionData({param});\n");
        }

        public string GetEnumName(ActionDefinition action, int i)
        {
            return GameDefinitionAsset.Ins.FindEnum(action.enumTypes[i]).ToString();
        }

        public string GetArgName(ActionParamType type)
        {
            switch(type)
            {
                case ActionParamType.Currency: return "currencyId";
                case ActionParamType.Chest: return "chestDetailId";
                case ActionParamType.Quest: return "questDetailId";
                case ActionParamType.Item: return "itemDetailId";
                case ActionParamType.IntValue: return "value";
                case ActionParamType.Enum: return "value";
                case ActionParamType.QuestGroup: return "questGroupId";
            }
            return "none";
        }
    }
}