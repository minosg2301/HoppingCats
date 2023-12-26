using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using moonNest;

public class ArenaAssetTab : TabContent
{
    readonly ArenaTab arenaTab;
    readonly TabContainer tabContainer;
    StatDefinition stat;

    public ArenaAssetTab()
    {
        stat = UserPropertyAsset.Ins.properties.FindStat(ArenaAsset.Ins.leagueStatId);
        arenaTab = new ArenaTab();
        arenaTab.SetStat(stat);
        tabContainer = new TabContainer();
        tabContainer.AddTab("Rewards", arenaTab);
        tabContainer.AddTab("Battle Pass", new BattlePassTab());
    }

    public override void DoDraw()
    {
        Undo.RecordObject(ArenaAsset.Ins, "Arena");
        Draw.BeginHorizontal(Draw.SubContentStyle);
        Draw.BeginVertical();
        ArenaAsset.Ins.seasonDuration = Draw.IntField("Duration (Day)", ArenaAsset.Ins.seasonDuration, 60);
        DrawLeagueStat();
        Draw.EndVertical();
        Draw.EndHorizontal();
        tabContainer.DoDraw();

        if(GUI.changed) Draw.SetDirty(ArenaAsset.Ins);
    }

    void DrawLeagueStat()
    {
        Draw.BeginChangeCheck();
        ArenaAsset.Ins.leagueStatId = Draw.IntPopupField("League Stat", ArenaAsset.Ins.leagueStatId, UserPropertyAsset.Ins.properties.stats, "name", "id", 120);
        if(Draw.EndChangeCheck())
        {
            stat = UserPropertyAsset.Ins.properties.FindStat(ArenaAsset.Ins.leagueStatId);
            if(ValidateStat(stat))
            {
                arenaTab.SetStat(stat);
                if(ArenaAsset.Ins.leagues.Count == 0)
                {
                    SyncFromStatProgress();
                }
            }
            arenaTab.SetStat(null);
        }

        if(stat == null)
        {
            Draw.HelpBox($"League Stat is null", MessageType.Error);
        }
        else if(!stat.layer || !stat.progress || stat.progressType != StatProgressType.Passive)
        {
            Draw.HelpBox($"'PROGRESS' and 'LAYER' of [{stat.name.ToUpper()}] must be enabled.\nProgress Type must be 'PASSIVE'.", MessageType.Error);
        }
        else if(Draw.FitButton($"Sync data with [{stat.name}] Progresses", Color.cyan))
        {
            SyncFromStatProgress();
        }
    }

    void SyncFromStatProgress()
    {
        StatProgressGroupDetail statProgress = StatProgressAsset.Ins.FindGroup(ArenaAsset.Ins.leagueStatId);

        // remove redundant league
        ArenaAsset.Ins.leagues.RemoveAll(league => statProgress.FindByStatValue(league.statValue) == null);

        // add new league
        statProgress.progresses.ForEach((progress, i) =>
        {
            LeagueDetail league = ArenaAsset.Ins.FindLeagueByStatValue(progress.statValue);
            if(league == null)
            {
                ArenaAsset.Ins.leagues.Add(new LeagueDetail(progress));
            }
            else
            {
                league.name = progress.name;
                league.displayName = progress.displayName;
            }
        });

        ArenaAsset.Ins.leagues.SortAsc(l => l.statValue);
        ArenaAsset.Ins.leagues.ForEach((leagueDetail, i) => leagueDetail.league = i + 1);
    }

    bool ValidateStat(StatDefinition stat)
    {
        return !(stat == null || !stat.layer || !stat.progress || stat.progressType != StatProgressType.Passive);
    }
}