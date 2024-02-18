using Doozy.Engine;
using Doozy.Engine.Progress;
using Doozy.Engine.UI;
using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using moonNest;

public class QuestProgressDrawer : BaseDrawer
{
    public Localize titleText;
    public Localize descriptionText;
    public Progressor progressor;

    static Quest currentQuest;
    static List<Quest> pendingQuests = new List<Quest>();
    static bool openInNextFrame = false;
    const string drawerName = "QuestProgress";

    public static void Init()
    {
        UserQuest.Ins.onQuestUpdated += (quests) =>
        {
            var _quests = quests.FindAll(quest =>
            {
                return !quest.Claimed && !quest.Detail.silent && quest.Progress.lastValue < quest.RequireValue;
            });
            if (_quests.Count > 0) Open(_quests);
        };
    }

    static void Open(List<Quest> quests)
    {
        if (pendingQuests.Count > 10)
        {
            Debug.LogWarning("PendingQuests reaches capacity (10). Perhaps game does not config Quest Progress Drawer.");
            return;
        }

        pendingQuests.AddRange(quests);
        Open();
    }

    internal static void Open()
    {
        if (pendingQuests.Count == 0) return;

        UIDrawer thisDrawer = UIDrawer.Get(drawerName);
        if (thisDrawer)
        {
            if (thisDrawer.Closed && !openInNextFrame)
                thisDrawer.Open();
        }
        else
        {
            Debug.LogWarning($"{drawerName} does not configured!!");
        }
    }

    protected override void OnOpen()
    {
        base.OnOpen();

        UpdateUI(pendingQuests.Shift());
    }

    protected override void OnOpenFinished()
    {
        base.OnOpenFinished();
        progressor.SetValue(currentQuest.Progress.value);
    }

    protected override void OnClose()
    {
        base.OnClose();

        openInNextFrame = false;
        if (pendingQuests.Count > 0)
        {
            openInNextFrame = true;
            Coroutiner.Start(OpenInNextFrame());
        }
    }

    IEnumerator OpenInNextFrame()
    {
        yield return null;
        openInNextFrame = false;
        Open();
    }

    protected virtual void UpdateUI(Quest quest)
    {
        currentQuest = quest;

        if (titleText) titleText.Term = quest.Detail.displayName;
        if (descriptionText) descriptionText.Term = quest.Detail.description;

        progressor.SetMin(0);
        progressor.SetMax(quest.Detail.require.count);
        progressor.InstantSetValue(quest.Progress.lastValue);
    }
}