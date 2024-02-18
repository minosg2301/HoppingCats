using DG.Tweening;
using EnhancedUI.EnhancedScroller;
using TMPro;
using UnityEngine;
using moonNest;

public class UIBattlePassCell : EnhancedScrollerCellView, IBattlePassCell
{
    public TextMeshProUGUI levelText;
    public UIBattlePassReward reward;
    public UIBattlePassReward premiumReward;
    public GameObject unlockNode;
    public GameObject lastestUnlockNode;
    public DOTweenAnimation[] showAnimations;

    private RectTransform _rect;
    public RectTransform Rect { get { if(!_rect) _rect = GetComponent<RectTransform>(); return _rect; } }

    public BattlePassLevel BattlePassLevel { get; private set; }

    void Start()
    {
        if (reward) reward.onClaimClicked += OnClaimedReward;
        if (premiumReward) premiumReward.onClaimClicked += OnClaimedPremiumReward;
    }

    void OnDestroy()
    {
        if (reward) reward.onClaimClicked -= OnClaimedReward;
        if (premiumReward) premiumReward.onClaimClicked -= OnClaimedPremiumReward;
    }

    public void SetData(BattlePassLevel battlePassLevel)
    {
        BattlePassLevel = battlePassLevel;
        UpdateUI(battlePassLevel);
    }

    void UpdateUI(BattlePassLevel battlePassLevel)
    {
        UserArena userArena = UserArena.Ins;
        bool paid = UserArena.Ins.PaidPremium;
        bool passed = userArena.Level > battlePassLevel.level;
        bool lastestPassed = userArena.Level - 1 == battlePassLevel.level;

        if(levelText) levelText.text = battlePassLevel.level.ToString();
        if(unlockNode) unlockNode.SetActive(passed);
        if(lastestUnlockNode) lastestUnlockNode.SetActive(lastestPassed);

        if(reward)
        {
            bool canClaim = userArena.CanClaimReward(battlePassLevel.level);
            reward.SetReward(userArena.GetReward(battlePassLevel));
            reward.Unlocked = passed;
            reward.CanClaim = passed && canClaim;
            reward.Claimed = passed && !canClaim;
        }

        if(premiumReward)
        {
            bool unlocked = paid && passed;
            bool canClaim = userArena.CanClaimReward(battlePassLevel.level, true);
            premiumReward.SetReward(userArena.GetReward(battlePassLevel, true));
            premiumReward.Unlocked = unlocked;
            premiumReward.CanClaim = unlocked && canClaim;
            premiumReward.Claimed = unlocked && !canClaim;
        }
    }

    protected virtual void OnClaimedReward()
    {
        UserArena.Ins.ClaimReward(BattlePassLevel.level);
        UpdateUI(BattlePassLevel);
    }

    protected virtual void OnClaimedPremiumReward()
    {
        UserArena.Ins.ClaimReward(BattlePassLevel.level, premium: true);
        UpdateUI(BattlePassLevel);
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