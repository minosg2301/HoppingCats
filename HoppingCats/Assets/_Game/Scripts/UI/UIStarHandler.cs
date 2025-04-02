using TMPro;

public class UIStarHandler : UIGroupByGameState
{
    public TextMeshProUGUI starText;

    protected override void OnEnable()
    {
        base.OnEnable();
        GameEventManager.Ins.OnStartGame += UpdateUI;
        GameEventManager.Ins.OnAddStar += OnAddStar;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        GameEventManager.Ins.OnStartGame -= UpdateUI;
        GameEventManager.Ins.OnAddStar -= OnAddStar;
    }

    protected override void OnShow()
    {
        base.OnShow();
        UpdateUI();
    }

    public void OnAddStar(int amount)
    {
        UserSaveData.Ins.AddStar(amount);
        UpdateUI();
    }
    private void UpdateUI()
    {
        starText.SetText(UserSaveData.Ins.star.ToString());
    }

}
