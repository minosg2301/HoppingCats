using DG.Tweening;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using moonNest;

public class UIBattlePassFinalCell : EnhancedScrollerCellView, IObserver, IBattlePassCell
{
    public UIBattlePassReward reward;
    public GameObject highlightNode;
    public DOTweenAnimation[] showAnimations;

    private RectTransform _rect;
    public RectTransform Rect { get { if(!_rect) _rect = GetComponent<RectTransform>(); return _rect; } }

    public bool CanClaimed { get; private set; }
    public BattlePassLevel BattlePassLevel { get; private set; }

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

    public void SetData(BattlePassLevel battlePassLevel)
    {
        BattlePassLevel = battlePassLevel;
        UpdateUI(battlePassLevel);
    }

    void UpdateUI(BattlePassLevel battlePassLevel)
    {
        reward.SetReward(battlePassLevel.reward);
    }

    public virtual void PlayShowAnimation()
    {
        showAnimations.ForEach(anim =>
        {
            if(anim.tween == null)
                anim.CreateTween();
            anim.DORestart();
        });
    }
}