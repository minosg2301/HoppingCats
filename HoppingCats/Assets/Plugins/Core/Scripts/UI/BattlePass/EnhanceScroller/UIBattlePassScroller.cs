using EnhancedUI.EnhancedScroller;
using System.Collections.Generic;
using moonNest;

public class UIBattlePassScroller : EnhancedScroller, IEnhancedScrollerDelegate, IObserver
{
    public UIBattlePassCell prefab;
    public UIBattlePassFinalCell finalCell;

    List<BattlePassLevel> levels;
    Dictionary<int, bool> animateds = new Dictionary<int, bool>();

    protected override void Start()
    {
        base.Start();

        Delegate = this;
        cellViewVisibilityChanged += OnCellViewVisibilityChanged;
        UserArena.Ins.Subscribe(this);
    }

    public void OnNotify(IObservable data, string[] scopes)
    {
        levels = ArenaAsset.Ins.levels;
        ReloadData();

        int currentLevel = UserArena.Ins.Level;
        int jumpIndex = currentLevel > levels.Count ? levels.Count : currentLevel - 1;
        JumpToDataIndex(jumpIndex, 0.5f, 0f);
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        bool lastIndex = dataIndex >= levels.Count;
        var battlePassLevel = lastIndex ? ArenaAsset.Ins.finalLevel : levels[dataIndex];

        EnhancedScrollerCellView cellPrefab;
        if(lastIndex) cellPrefab = finalCell;
        else cellPrefab = prefab;

        var cellView = GetCellView(cellPrefab);
        cellView.gameObject.SetActive(true);
        cellView.customData = battlePassLevel;
        var battleCellView = cellView as IBattlePassCell;
        battleCellView.SetData(battlePassLevel);
        return cellView;
    }

    private void OnCellViewVisibilityChanged(EnhancedScrollerCellView cellView)
    {
        if(!cellView.active) return;

        var battlePassLevel = cellView.customData as BattlePassLevel;
        if(!animateds.ContainsKey(battlePassLevel.level))
        {
            animateds[battlePassLevel.level] = true;
            (cellView as IBattlePassCell).PlayShowAnimation();
        }
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        bool lastIndex = dataIndex >= levels.Count;
        return lastIndex ? finalCell.Rect.rect.height : prefab.Rect.rect.height;
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return levels.Count + 1;
    }
}
