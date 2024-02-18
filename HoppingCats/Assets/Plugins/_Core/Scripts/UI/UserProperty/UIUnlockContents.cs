using System.Collections.Generic;
using UnityEngine;
using moonNest;

public class UIUnlockContents : MonoBehaviour
{
    private UIUnlockContent[] _ui;
    public UIUnlockContent[] UnlockContents { get { if(_ui == null) _ui = GetComponentsInChildren<UIUnlockContent>(true); return _ui; } }

    readonly UIListContainer<UnlockContentDetail, UIUnlockContent> listContainer = new UIListContainer<UnlockContentDetail, UIUnlockContent>();

    public void SetData(List<UnlockContentDetail> unlockContentDetails)
    {
        listContainer.SetList(transform, unlockContentDetails);
    }
}