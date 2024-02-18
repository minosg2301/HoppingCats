using System.Collections.Generic;
using System.IO;

namespace moonNest.editor
{
    public partial class ItemDefinitionTab
    {
        private ItemDefinition item;
        private string className;
        private string classDetailName;

        private void GenerateClass(List<ItemDefinition> items)
        {
            foreach(var item in items)
            {
                this.item = item;
                className = item.name;
                classDetailName = item.name + "Detail";
                GenerateItemDetailClass(item);
                GenerateItemClass(item);
            }
        }

        private void GenerateItemDetailClass(ItemDefinition item)
        {
            string directory = $"{GlobalConfig.Ins.GenPath}/Items";
            if(!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string filePath = $"{directory}/{classDetailName}.cs";

            StreamWriter writer = new StreamWriter(filePath, false);
            writer.WriteLine("");
            writer.WriteLine("// This file is auto generated from Item Definition Editor");
            writer.WriteLine("// Manually modified can caused bug");
            writer.WriteLine("using UnityEngine.AddressableAssets;\nusing UnityEngine;\nusing vgame;\nusing System;\n");
            writer.WriteLine("using System.Collections.Generic;\nusing System.Threading.Tasks;");
            writer.WriteLine($"");
            writer.WriteLine($"public partial struct {classDetailName}\n" + "{");
            writer.WriteLine("\tpublic int Id { get; private set; }\n");
            writer.WriteLine("\tpublic ItemDetail Detail { get; private set;}\n");

            // ctor
            writer.WriteLine($"\tpublic {classDetailName}(int detailId)" + "\n\t{");
            writer.WriteLine("\t\tId = detailId;");
            writer.WriteLine("\t\tDetail = ItemAsset.Ins.Find(detailId);\n\t}\n");

            // ctor
            writer.WriteLine($"\tpublic {classDetailName}(ItemDetail detail)" + "\n\t{");
            writer.WriteLine("\t\tId = detail.id;");
            writer.WriteLine("\t\tDetail = detail;\n\t}\n");

            writer.WriteLine("\tpublic string Name => Detail.name;");
            writer.WriteLine("\tpublic Sprite Icon => Detail.icon;");
            writer.WriteLine("\tpublic int InitAmount => Detail.initAmount;");
            writer.WriteLine("\tpublic int Capacity => Detail.capacity;");
            writer.WriteLine("\tpublic bool Active => Detail.active;");
            writer.WriteLine("\tpublic GameObject UIPrefab => Detail.UIPrefab;");
            writer.WriteLine("");

            WriteExtendConfig(writer, item);

            if(item.stats.Count > 0)
            {
                item.stats.ForEach(stat => WriteStatKey(writer, stat));
                writer.WriteLine("");
                item.stats.ForEach(stat => WriteStatDetail(writer, stat));
            }
            if(item.attributes.Count > 0)
            {
                item.attributes.ForEach(attr => WriteAttrKey(writer, attr));
                writer.WriteLine("");
                item.attributes.ForEach(attr => WriteAttr(writer, attr));
            }
            if(item.assetRefs.Count > 0)
            {
                item.assetRefs.ForEach(assetRef => WriteAssetRefKey(writer, assetRef));
                writer.WriteLine("");
                item.assetRefs.ForEach(assetRef => WriteAssetRef(writer, assetRef));
            }
            if(item.refs.Count > 0)
            {
                item.refs.ForEach(@ref => WriteRefKey(writer, @ref));
                writer.WriteLine("");
                item.refs.ForEach(@ref => WriteRefDetail(writer, @ref));
            }
            if(item.enums.Count > 0)
            {
                item.enums.ForEach(@enum => WriteEnumKey(writer, @enum));
                writer.WriteLine("");
                item.enums.ForEach(@enum => WriteEnumDetail(writer, @enum));
            }

            WriteDetailStaticMethods(writer);

            writer.WriteLine("}");
            writer.Flush();
            writer.Close();
        }

        private void GenerateItemClass(ItemDefinition item)
        {
            string directory = $"{GlobalConfig.Ins.GenPath}/Items";
            if(!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string filePath = $"{directory}/{className}.cs";

            StreamWriter writer = new StreamWriter(filePath, false);
            writer.WriteLine("");
            writer.WriteLine("// This file is auto generated from Item Definition Editor");
            writer.WriteLine("// Manually modified can caused bug");
            writer.WriteLine("using System;\nusing UnityEngine.AddressableAssets;\nusing UnityEngine;\nusing vgame;");
            writer.WriteLine("using System.Collections.Generic;\nusing System.Threading.Tasks;\nusing UnityEngine.Scripting;");
            writer.WriteLine($"[Serializable]");
            writer.WriteLine($"[Preserve]");
            writer.WriteLine($"public partial class {className} : Item\n" + "{");
            writer.WriteLine($"\tpublic {className}()" + " { }\n");
            //writer.WriteLine($"\tpublic {className}(ItemDetail detailConfig) : base(detailConfig) " + "{}\n");

            writer.WriteLine($"\tprivate {classDetailName} _{classDetailName};");
            writer.WriteLine("\tpublic " + classDetailName + " " + classDetailName + " { get { if (_"
                + classDetailName + ".Id == 0) _" + classDetailName + " = new " + classDetailName + "(DetailId); return _" + classDetailName + "; } }\n");

            writer.WriteLine("\tpublic string Name => Detail.name;\n");
            writer.WriteLine("\tpublic Sprite Icon => Detail.icon;\n");

            WriteExtendConfig(writer, item);

            if(item.stats.Count > 0)
            {
                item.stats.ForEach(stat => WriteStatKey(writer, stat));
                writer.WriteLine("");
                item.stats.ForEach(stat => WriteStat(writer, stat));
            }
            if(item.attributes.Count > 0)
            {
                item.attributes.ForEach(attr => WriteAttrKey(writer, attr));
                writer.WriteLine("");
                item.attributes.ForEach(attr => WriteAttr(writer, attr));
            }
            if(item.assetRefs.Count > 0)
            {
                item.assetRefs.ForEach(assetRef => WriteAssetRefKey(writer, assetRef));
                writer.WriteLine("");
                item.assetRefs.ForEach(assetRef => WriteAssetRef(writer, assetRef));
            }
            if(item.refs.Count > 0)
            {
                item.refs.ForEach(@ref => WriteRefKey(writer, @ref));
                writer.WriteLine("");
                item.refs.ForEach(@ref => WriteRefDetail(writer, @ref));
            }
            if(item.enums.Count > 0)
            {
                item.enums.ForEach(@enum => WriteEnumKey(writer, @enum));
                writer.WriteLine("");
                item.enums.ForEach(@enum => WriteEnum(writer, @enum));
            }

            WriteStaticMethods(writer);

            writer.WriteLine("}");
            writer.Flush();
            writer.Close();
        }

        private static void WriteExtendConfig(StreamWriter writer, ItemDefinition item)
        {
            if(string.IsNullOrEmpty(item.scriptableCastName)) return;

            writer.WriteLine($"\tpublic {item.scriptableCastName} {item.scriptableCastName} => ({item.scriptableCastName})Detail.extendConfig;\n");
            writer.WriteLine($"\tpublic bool UseConfigReference => Detail.extendConfig.useReference;\n");
            writer.WriteLine($"\tpublic Task LoadConfigAsync() => Detail.extendConfig.LoadAssetAsync();\n");
        }

        private void WriteDetailStaticMethods(StreamWriter writer)
        {
            string content =
                $"\tstatic List<{classDetailName}> database;\n" +
                $"\tpublic static {classDetailName} Find(int {item.name}DetailId)\n" +
                "\t{\n" +
                $"\t\treturn FindAll().Find(item => item.Id == {item.name}DetailId);\n" +
                "\t}\n\n";

            content +=
                $"\tpublic static List<{classDetailName}> FindAll()\n" +
                "\t{\n" +
                "\t\tif(database == null)\n" +
                $"\t\t\tdatabase = ItemAsset.Ins.FindByDefinition(\"{item.name}\").Map(d => new {classDetailName}(d));\n" +
                "\t\treturn database;\n" +
                "\t}\n\n";

            content +=
                $"\tpublic static List<{classDetailName}> FindAll(Predicate<{classDetailName}> predicate)\n" +
                "\t{\n" +
                "\t\tif(database == null)\n" +
                $"\t\t\tdatabase = ItemAsset.Ins.FindByDefinition(\"{item.name}\").Map(d => new {classDetailName}(d));\n" +
                "\t\treturn database.FindAll(predicate);\n" +
                "\t}\n";

            writer.WriteLine(content);
        }

        private void WriteStaticMethods(StreamWriter writer)
        {
            string functions = "";

            if(item.storageType == StorageType.Several)
            {
                functions += "\tpublic static {0} Create(int {1}DetailId) => UserInventory.Create<{0}>({1}DetailId);\n\n";
            }

            functions +=
                "\tpublic static {0} Find(int {1}Id) => UserInventory.Find<{0}>({1}Id);\n\n" +
                "\tpublic static List<{0}> FindAll() => UserInventory.FindAll<{0}>();\n\n" +
                "\tpublic static List<{0}> FindAll(Predicate<{0}> predicate) => UserInventory.FindAll(predicate);\n\n";

            var stats = item.stats.FindAll(stat => stat.distinct);
            var enums = item.enums.FindAll(@enum => @enum.distinct);

            string template =
                "\tpublic static {0} FindByDetail(int {1}DetailId, __PARAMS__)\n\t\t=> UserInventory.FindByDetail<{0}>({1}DetailId__FUNCTION_PARAMS__);\n\n" +
                "\tpublic static {0} FindOrCreate(int {1}DetailId, __PARAMS__)\n\t\t=> UserInventory.FindOrCreate<{0}>({1}DetailId__FUNCTION_PARAMS__);";

            if(stats.Count > 0 || enums.Count > 0)
            {
                string @params = "";
                string functionParams = "";

                if(stats.Count > 0)
                {
                    functionParams = ",\n\t\t\tnew DistinctStat[] {{ ";
                    foreach(var stat in stats)
                    {
                        @params += $"int {stat.name.ToLower()}, ";
                        functionParams += $"new DistinctStat(k{stat.name}, {stat.name.ToLower()}), ";
                    }
                    functionParams = functionParams.Remove(functionParams.LastIndexOf(","), 2);
                    functionParams += " }}";
                }

                if(enums.Count > 0)
                {
                    functionParams += ",\n\t\t\tnew DistinctEnum[] {{ ";
                    foreach(var @enum in enums)
                    {
                        @params += $"{@enum.Definition.name} {@enum.name.ToLower()}, ";
                        functionParams += $"new DistinctEnum(k{@enum.name}, (byte){@enum.name.ToLower()}), ";
                    }
                    functionParams = functionParams.Remove(functionParams.LastIndexOf(","), 2);
                    functionParams += " }}";
                }

                @params = @params.Remove(@params.LastIndexOf(","), 2);

                template = template
                    .Replace("__PARAMS__", @params)
                    .Replace("__FUNCTION_PARAMS__", functionParams);

                functions += template;
            }

            writer.WriteLine(string.Format(functions, className, className.ToLower()));
        }

        private void WriteStatDetail(StreamWriter writer, StatDefinition stat)
        {
            string propertyName = stat.name;
            string statKey = $"k{propertyName}";
            string content = stat.type == StatValueType.Float
            ? "\tpublic float " + propertyName + $" => Detail.Stat({statKey}).value.AsFloat;\n"
            : "\tpublic int " + propertyName + $" => Detail.Stat({statKey}).value.AsInt;\n";
            writer.WriteLine(content);
        }

        private void WriteStatKey(StreamWriter writer, StatDefinition stat)
        {
            writer.WriteLine($"\tpublic static readonly int k{stat.name} = {stat.id};");
        }

        private void WriteStat(StreamWriter writer, StatDefinition stat)
        {
            string propertyName = stat.name;
            string statKey = $"k{propertyName}";
            string variableType = stat.type == StatValueType.Float ? "float" : "int";
            if(stat.savable)
            {
                string savableContent = $"\tpublic {variableType} " + propertyName + "\n" +
                    "\t{\n" +
                    $"\t\tget {{ return GetStat({statKey}); }}\n" +
                    $"\t\tset {{ SetStat({statKey}, value); }}\n" +
                    "\t}\n";

                writer.WriteLine(savableContent);
            }
            else
            {
                string content = stat.type == StatValueType.Float
                ? "\tpublic float " + propertyName + $" => Detail.Stat({statKey}).value.AsFloat;\n"
                : "\tpublic int " + propertyName + $" => Detail.Stat({statKey}).value.AsInt;\n";
                writer.WriteLine(content);
            }
        }

        private void WriteAttrKey(StreamWriter writer, AttributeDefinition attr)
        {
            writer.WriteLine($"\tpublic static readonly int k{attr.name} = {attr.id};");
        }

        private void WriteAttr(StreamWriter writer, AttributeDefinition attr)
        {
            string propertyName = attr.name;
            string attrKey = $"k{propertyName}";

            switch(attr.type)
            {
                case AttributeType.String: writer.WriteLine($"\tpublic string {propertyName} => Detail.Attr({attrKey}).value.AsString;\n"); break;
                case AttributeType.GameObject: writer.WriteLine($"\tpublic GameObject {propertyName} => Detail.Attr({attrKey}).value.AsPrefab;\n"); break;
                case AttributeType.Sprite: writer.WriteLine($"\tpublic Sprite {propertyName} => Detail.Attr({attrKey}).value.AsSprite;\n"); break;
            }
        }

        private void WriteAssetRefKey(StreamWriter writer, AssetReferenceDefinition assetRef)
        {
            writer.WriteLine($"\tpublic static readonly int k{assetRef.name} = {assetRef.id};");
        }

        private void WriteAssetRef(StreamWriter writer, AssetReferenceDefinition assetRef)
        {
            string assetName = assetRef.name;
            string key = $"k{assetName}";

            string format = "public AssetReference {0} => Detail.AssetReference({1}).assetReference;";
            writer.WriteLine($"\t{string.Format(format, assetName, key)}\n");

            format = "public AssetReferenceDetail {0}AssetReference => Detail.AssetReference({1});";
            writer.WriteLine($"\t{string.Format(format, assetName, key)}\n");
        }

        private void WriteRefKey(StreamWriter writer, ReferenceDefinition @ref)
        {
            writer.WriteLine($"\tpublic static readonly int k{@ref.name} = {@ref.id};");
        }

        private void WriteRefDetail(StreamWriter writer, ReferenceDefinition @ref)
        {
            ItemDefinition itemDef = ItemAsset.Ins.FindDefinition(@ref.itemDefinitionId);
            string className = itemDef.name;
            string fieldName = @ref.name;
            string refKey = $"k{@ref.name}";
            writer.WriteLine($"\tpublic {className} {fieldName} => {className}.Find(Detail.Reference({refKey}).referenceId);\n");
        }

        private void WriteRef(StreamWriter writer, ReferenceDefinition @ref)
        {
            ItemDefinition itemDef = ItemAsset.Ins.FindDefinition(@ref.itemDefinitionId);

            string className = itemDef.name;
            string fieldName = @ref.name;
            string refKey = $"k{@ref.name}";
            writer.WriteLine($"\tpublic {className} {fieldName} => {className}.Find(Detail.Reference({refKey}).referenceId);\n");
        }

        private void WriteEnumKey(StreamWriter writer, EnumPropertyDefinition @enum)
        {
            writer.WriteLine($"\tpublic static readonly int k{@enum.name} = {@enum.id};");
        }

        private void WriteEnumDetail(StreamWriter writer, EnumPropertyDefinition @enum)
        {
            EnumDefinition enumDefinition = GameDefinitionAsset.Ins.FindEnum(@enum.definitionId);

            string refKey = $"k{@enum.name}";
            writer.WriteLine($"\tpublic {enumDefinition.name} {@enum.name} => Detail.Type<{enumDefinition.name}>({refKey});\n");
        }

        private void WriteEnum(StreamWriter writer, EnumPropertyDefinition @enum)
        {
            EnumDefinition enumDefinition = GameDefinitionAsset.Ins.FindEnum(@enum.definitionId);

            string enumType = enumDefinition.name;
            string fieldName = @enum.name;
            string refKey = $"k{@enum.name}";

            if(@enum.savable)
            {
                string savableContent = $"\tpublic {enumType} {fieldName}\n" +
                    "\t{\n" +
                    $"\t\tget {{ return ({enumType})GetEnum({refKey}); }}\n" +
                    $"\t\tset {{ SetEnum({refKey}, (byte)value); }}\n" +
                    "\t}\n";

                writer.WriteLine(savableContent);
            }
            else
            {

                writer.WriteLine($"\tpublic {enumType} {fieldName} => Detail.Type<{enumType}>({refKey});\n");
            }
        }
    }
}