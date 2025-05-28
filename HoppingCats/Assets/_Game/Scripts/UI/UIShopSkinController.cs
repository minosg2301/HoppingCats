using UnityEngine;
using moonNest;

public class UIShopSkinController : MonoBehaviour
{
    public Transform elementContainer;
    protected UIListContainer<SkinConfig, UIShopSkinElement> uiListElementContainer = new();

    private void OnEnable()
    {
        uiListElementContainer.SetList(elementContainer, SkinAsset.Ins.skinConfigs);
    }
}
