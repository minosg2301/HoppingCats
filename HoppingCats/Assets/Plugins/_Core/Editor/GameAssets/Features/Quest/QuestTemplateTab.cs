using UnityEngine;
using moonNest;

public class QuestTemplateTab : TabContent
{
    private const string _7DaysLogin = "Create 7 Day Login Quest";
    private const string _30DayLogin = "Create 30 Day Login Quest";
    private const string DailyQuest = "Create Daily Quest";

    public override void DoDraw()
    {
        if (Draw.FitButton(_7DaysLogin)) CreateDayLogin(7);
        Draw.Space();
        if (Draw.FitButton(_30DayLogin)) CreateDayLogin(30);
        Draw.Space();
        if (Draw.FitButton(DailyQuest)) CreateDailyQuest();
    }

    public override Color HeaderBackgroundColor => Color.grey;

    private void CreateDailyQuest()
    {
        QuestGroupDetail group = new QuestGroupDetail("Daily Quest");
        group.refreshConfig.type = QuestRefeshType.OnTime;
        group.refreshConfig.period.duration = 1;
        group.refreshConfig.period.type = PeriodType.Day;
        group.refreshConfig.maxQuest = 5;
        QuestAsset.Ins.AddGroup(group);
    }

    private void CreateDayLogin(int maxDay)
    {
        QuestGroupDetail group = new QuestGroupDetail(maxDay + " Days Login") { silent = true };
        group.refreshConfig.type = QuestRefeshType.OnCompleted;
        group.refreshConfig.period.duration = maxDay;
        group.refreshConfig.period.type = PeriodType.Day;
        QuestAsset.Ins.AddGroup(group);

        ActionDefinition loginDayAction = GameDefinitionAsset.Ins.actions.Find(a => a.id == ActionDefinition.kLoginDay);
        for (int day = 1; day <= maxDay; day++)
        {
            QuestDetail quest = new QuestDetail("Day " + day, group.id) { silent = true };
            ActionDetail action = new ActionDetail(loginDayAction);
            CurrencyReward currencyReward = new CurrencyReward() { contentId = GameDefinitionAsset.Ins.currencies[0].id };

            quest.displayName = "TXT_DAY_LOGIN";
            quest.description = "TXT_DAY_LOGIN_DESCRIPTION";
            quest.require = new ActionRequire() { action = action, count = day };
            quest.reward.rewards.Add(new RewardObject(currencyReward));
            QuestAsset.Ins.Add(quest);
        }
    }
}