using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationSystem
{
    public enum Language
    {
        English,
        Vietnamese
    }

    public static Language language;
    public static void SetLanguage(Language newLanguage)
    {
        language = newLanguage;
    }

    static Dictionary<string, string> localisedEN;
    static Dictionary<string, string> localisedVN;

    public static bool isInit;
    public static void Init()
    {
        CSVLoader csvLoader = new CSVLoader();
        csvLoader.LoadCSV();

        localisedEN = csvLoader.GetDictonaryValues("en");
        localisedVN = csvLoader.GetDictonaryValues("vn");

        isInit = true;

    }

    public static string GetLocalisedValue(string key)
    {
        if (!isInit) { Init(); }

        string value = key;
        switch (language)
        {
            case Language.English:
                localisedEN.TryGetValue(key, out value);
                break;
            case Language.Vietnamese:
                localisedVN.TryGetValue(key, out value);
                break;

        }
        return value;
    }
}