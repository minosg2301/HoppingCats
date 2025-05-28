using UnityEngine;
using moonNest;
using TMPro;

public class UIShopSkinElement : BaseUIData<SkinConfig>
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public GameObject lockNode;
    public GameObject ownedNode;
    public GameObject selectedNode;

    public Vector3 selectedScale = Vector3.one;
    public Vector3 defaultScale = Vector3.one;

    protected SkinConfig config;
    protected CatController catPrefab;

    public override void SetData(SkinConfig config)
    {
        this.config = config;
        if (nameText) nameText.text = config.name;

        if (catPrefab) catPrefab.Destroy();
        catPrefab = Instantiate(config.catPrefab, transform);
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (!catPrefab) return;
        bool isSelected = config.id == UserSaveData.Ins.currentUserSkinId;
        catPrefab.transform.localScale = isSelected ? selectedScale : defaultScale;
    }
}
