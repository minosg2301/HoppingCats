#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.UI;

public static class ImageExt
{
    public const string kStandardSpritePath = "UI/Skin/UISprite.psd";
    public const string kWhiteSprite = "Assets/Plugins/_Core/Sprites/white.png";

#if UNITY_EDITOR
    public static void SetSpriteDefault(this Image image, bool raycastTarget = true)
    {
        image.raycastTarget = raycastTarget;
        image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
        image.type = Image.Type.Sliced;
    }
#endif
}

