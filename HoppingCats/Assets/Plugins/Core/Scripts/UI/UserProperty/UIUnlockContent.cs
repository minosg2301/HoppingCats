using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using moonNest;

public class UIUnlockContent : BaseUIData<UnlockContentDetail>
{
    public TextMeshProUGUI nameText;
    public Image icon;
    public GameObject prefabContainer;

    private Localize _nameLoc;
    public Localize NameLoc { get { if(!_nameLoc && nameText) _nameLoc = nameText.GetComponent<Localize>(); return _nameLoc; } }

    GameObject uiPrefab;

    public override void SetData(UnlockContentDetail unlockContentDetail)
    {
        if(NameLoc) NameLoc.Term = unlockContentDetail.DisplayName;
        else if(nameText) nameText.text = unlockContentDetail.DisplayName;

        if(icon) icon.sprite = unlockContentDetail.Icon;

        if(prefabContainer && unlockContentDetail.Prefab)
        {
            if(icon) icon.gameObject.SetActive(false);
            if(uiPrefab) uiPrefab.SetActive(false);
            prefabContainer.SetActive(true);

            uiPrefab = Instantiate(unlockContentDetail.Prefab);
            uiPrefab.transform.SetParent(prefabContainer.transform, false);
            uiPrefab.transform.localPosition = Vector3.zero;
            uiPrefab.transform.localScale = Vector3.one;
        }
        else if(icon && unlockContentDetail.Icon)
        {
            if(prefabContainer) prefabContainer.SetActive(false);
            icon.gameObject.SetActive(true);
            icon.sprite = unlockContentDetail.Icon;
        }
    }
}