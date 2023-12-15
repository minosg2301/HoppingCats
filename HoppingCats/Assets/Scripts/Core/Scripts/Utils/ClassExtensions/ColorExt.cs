using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class ColorExt
{
    public static Color Parse(string htmlColor)
    {
        Color color;
        ColorUtility.TryParseHtmlString(htmlColor, out color);
        return color;
    }
}

