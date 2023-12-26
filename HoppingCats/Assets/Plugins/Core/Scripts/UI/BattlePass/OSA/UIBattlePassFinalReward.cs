using UnityEngine;
using moonNest;

public class UIBattlePassFinalReward : UIBaseRewardNode, IObserver
{
    public GameObject highlightNode;

    private RectTransform _rect;
    public RectTransform Rect { get { if(!_rect) _rect = GetComponent<RectTransform>(); return _rect; } }

    public bool CanClaimed { get; private set; }

    public void OnEnable()
    {
        UserData.Ins.Subscribe(this, "Battle Pass", ArenaAsset.Ins.requireStatId.ToString());
    }

    public void OnDisable()
    {
        UserData.Ins?.Unsubscribe(this);
    }

    public void OnNotify(IObservable data, string[] scopes)
    {
        CanClaimed = UserArena.Ins.CanClaimFinalReward;
        highlightNode.SetActive(CanClaimed);
    }

    protected internal override void OnClaimClicked()
    {
        base.OnClaimClicked();
        UserArena.Ins.ClaimFinalReward();
    }
}