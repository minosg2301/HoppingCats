using System.Collections;
using System.Collections.Generic;
using moonNest;
using UnityEngine;

public class DrawUtils
{
    public static void DrawAddressableSprite(AssetReferenceDetail asset, float width, float height)
    {
        Draw.BeginVertical();

        if (asset != null)
        {
            asset.Draw(width);
            asset.DrawSubAsset(true, width, height);
        }
        
        Draw.EndVertical();
    }

    public static Sprite DrawSpriteCell(Sprite sprite, float width, float height)
    {
        Draw.BeginVertical();
        var _sprite = Draw.Sprite(sprite, false, width, height);
        Draw.EndVertical();
        return _sprite;
    }
}
