using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ThemeUtils
{
    public static bool Contains(this List<ThemeTile> list, string type)
    {
        foreach(var t in list) 
        { 
            if (t.type == type) return true;
        }

        return false;
    }
}
