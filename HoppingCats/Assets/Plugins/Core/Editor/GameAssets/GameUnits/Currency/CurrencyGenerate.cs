using System.Collections.Generic;
using System.IO;
using moonNest;

public partial class CurrencyTab
{
    private static void GenerateClass(List<CurrencyDefinition> currencies)
    {
        string directory = $"{GlobalConfig.Ins.GenPath}/Currencies";
        if(!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        foreach(var currency in currencies)
        {
            string className = currency.name;
            string filePath = $"{directory}/{className}.cs";

            StreamWriter writer = new StreamWriter(filePath, false);
            writer.WriteLine("");
            writer.WriteLine("// This file is auto generated from Currency Setting Editor");
            writer.WriteLine("// Manually modified can caused bug");
            writer.WriteLine("using vgame;\n");
            writer.WriteLine($"public class {className}\n" + "{");

            writer.WriteLine("\tprivate static Currency _currency;");

            writer.WriteLine("\tpublic static Currency Currency");
            writer.WriteLine("\t{\n\t\tget\n\t\t{\n\t\t\tif (_currency == null)");
            writer.WriteLine("\t\t\t{");
            writer.WriteLine($"\t\t\t\t_currency = UserCurrency.Get(\"{className}\");");
            writer.WriteLine("\t\t\t\tUserCurrency.Ins.OnDelete += OnUserCurrencyDeleted;");
            writer.WriteLine("\t\t\t}\n\t\t\treturn _currency;\n\t\t}\n\t}\n");

            writer.WriteLine("\tprivate static void OnUserCurrencyDeleted()");
            writer.WriteLine("\t{");
            writer.WriteLine("\t\tUserCurrency.Ins.OnDelete -= OnUserCurrencyDeleted;");
            writer.WriteLine("\t\t_currency = null;");
            writer.WriteLine("\t}\n");

            writer.WriteLine("\tpublic static double Value => Currency.Value;\n");
            writer.WriteLine("\tpublic static void Subscribe(IObserver observer, bool first = false)\n\t{");
            writer.WriteLine("\t\tCurrency.Subscribe(observer, first);\n\t}\n");
            writer.WriteLine("\tpublic static void Unsubscribe(IObserver observer)\n\t{");
            writer.WriteLine("\t\tCurrency.Unsubscribe(observer);\n\t}\n");
            writer.WriteLine("\tpublic static void AddValue(double value, bool save = true)\n\t{");
            writer.WriteLine("\t\tCurrency.AddValue(value, save);\n\t}\n");
            writer.WriteLine("\tpublic static void SetValue(double value)\n\t{");
            writer.WriteLine("\t\tCurrency.SetValue(value);\n\t}");

            writer.WriteLine("}");
            writer.Flush();
            writer.Close();

            /*
             * 
    public static Currency Currency
    {
        get
        {
            if (_currency == null)
            {
                _currency = UserCurrency.Get("IGC");
                UserCurrency.Ins.OnDelete += OnUserCurrencyDeleted;
            }
            return _currency;
        }
    }

    private static void OnUserCurrencyDeleted()
    {
        UserCurrency.Ins.OnDelete -= OnUserCurrencyDeleted;
        _currency = null;
    }
             */
        }
    }
}
