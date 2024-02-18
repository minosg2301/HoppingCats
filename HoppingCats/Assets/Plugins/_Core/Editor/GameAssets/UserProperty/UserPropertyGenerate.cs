using System.IO;
using moonNest;

public partial class UserPropertyDefinitionTab
{
    private static readonly string itemClassTemplate = "using UnityEngine;\r\nusing vgame;\r\n\r\n";

    private void GenerateClass(UserPropertyDefinition userData)
    {
        string className = "UserProperty";
        string filePath = $"{GlobalConfig.Ins.GenPath}/{className}.cs";
        string itemClassDeclare = itemClassTemplate + $"public class {className}\r\n" + "{";

        StreamWriter writer = new StreamWriter(filePath, false);
        writer.WriteLine("");
        writer.WriteLine("// This file is auto generated from User Data Definition Editor");
        writer.WriteLine("// Manually modified can caused bug");
        writer.WriteLine(itemClassDeclare);

        writer.WriteLine("\tpublic static void Save() => UserData.Ins.Save();\n");
        writer.WriteLine("\tpublic static int OfflineDay => UserData.OfflineDay;");
        writer.WriteLine("\tpublic static bool FirstLoginDay => UserData.FirstLoginDay;");
        writer.WriteLine("\tpublic static bool Linked { get { return UserData.Linked; } set { UserData.Linked = value; } }");
        writer.WriteLine("\tpublic static bool MusicOn { get { return UserData.MusicOn; } set { UserData.MusicOn = value; } }");
        writer.WriteLine("\tpublic static bool SfxOn { get { return UserData.SfxOn; } set { UserData.SfxOn = value; } }");
        writer.WriteLine("\tpublic static bool HapticOn { get { return UserData.HapticOn; } set { UserData.HapticOn = value; } }");
        writer.WriteLine("");

        if(userData.stats.Count > 0)
        {
            userData.stats.ForEach(stat => WriteStatKey(writer, stat));
            writer.WriteLine("");
            userData.stats.ForEach(stat => WriteStat(writer, stat));
        }
        if(userData.attributes.Count > 0)
        {
            userData.attributes.ForEach(attr => WriteAttrKey(writer, attr));
            writer.WriteLine("");
            userData.attributes.ForEach(attr => WriteAttr(writer, attr));
        }

        writer.WriteLine("}");
        writer.Flush();
        writer.Close();
    }

    private void WriteStatKey(StreamWriter writer, StatDefinition stat)
    {
        writer.WriteLine($"\tpublic static readonly int k{stat.name} = {stat.id};");
    }

    private void WriteStat(StreamWriter writer, StatDefinition stat)
    {
        string propertyName = stat.name;
        string statKey = $"k{propertyName}";
        string content = "\tpublic static StatValue " + propertyName + "\r\n" +
        "\t{\r\n" +
        "\t\tget { return UserData.Stat(" + statKey + "); }\r\n" +
        "\t\tset { UserData.SetStat(" + statKey + ", value); }\r\n" +
        "\t}\r\n";

        writer.WriteLine(content);
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
            case AttributeType.String: WriteStringAttribute(writer, attrKey, propertyName); break;
            case AttributeType.GameObject: WriteObjectAttribute(writer, attrKey, propertyName); break;
            case AttributeType.Sprite: WriteSpriteAttribute(writer, attrKey, propertyName); break;
        }
    }

    private void WriteStringAttribute(StreamWriter writer, string attrKey, string propertyName)
    {
        string content;
        if(attrKey.Equals("kUserId"))
        {
            content = "\tpublic static string " + propertyName + "\r\n\t{\r\n\t\tget { return UserData.UserId; }\r\n\t\tset { UserData.UserId = value; }\r\n\t}\r\n";
        }
        else if(attrKey.Equals("kName"))
        {
            content = "\tpublic static string " + propertyName + "\r\n\t{\r\n\t\tget { return UserData.UserName; }\r\n\t\tset { UserData.UserName = value; }\r\n\t}\r\n";
        }
        else if(attrKey.Equals("kLanguage"))
        {
            content = "\tpublic static string " + propertyName + "\r\n\t{\r\n\t\tget { return UserData.Language; }\r\n\t\tset { UserData.Language = value; }\r\n\t}\r\n";
        }
        else
        {
            content = "\tpublic static string " + propertyName + "\r\n" +
            "\t{\r\n" +
            "\t\tget { return UserData.Attr(" + attrKey + ").AsString; }\r\n" +
            "\t\tset { UserData.SetAttr(" + attrKey + ", value); }\r\n" +
            "\t}\r\n";
        }

        writer.WriteLine(content);
    }

    private void WriteObjectAttribute(StreamWriter writer, string attrKey, string propertyName)
    {
        string content = "\tpublic static GameObject " + propertyName + "\r\n" +
        "\t{\r\n" +
        "\t\tget { return UserData.Attr(" + attrKey + ").AsPrefab; }\r\n" +
        "\t\tset { UserData.SetAttr(" + attrKey + ", value); }\r\n" +
        "\t}\r\n";

        writer.WriteLine(content);
    }

    private void WriteSpriteAttribute(StreamWriter writer, string attrKey, string propertyName)
    {
        string content = "\tpublic static Sprite " + propertyName + "\r\n" +
        "\t{\r\n" +
        "\t\tget { return UserData.Attr(" + attrKey + ").AsSprite; }\r\n" +
        "\t\tset { UserData.SetAttr(" + attrKey + ", value); }\r\n" +
        "\t}\r\n";

        writer.WriteLine(content);
    }
}