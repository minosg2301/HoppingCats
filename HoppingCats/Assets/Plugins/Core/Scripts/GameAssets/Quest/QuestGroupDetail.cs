using System;
using System.Collections.Generic;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class QuestGroupDetail : BaseData
    {
        public QuestRefreshConfig refreshConfig = new QuestRefreshConfig();

        public bool activeOnLoad = true;
        public bool removeOnCompleted = false;
        public bool layerEnabled;
        public int serverId;

        public QuestGroupType type;

        // quest point
        public Sprite pointIcon;
        public bool pointEnabled;
        public bool pointRewardOnly;
        public bool pointAutoClaimed;
        public List<PointReward> pointRewards = new List<PointReward>();

        // quest group unlocked when this quest group completed
        public List<int> activeOnCompleteds = new List<int>();

        // for editor
        public DrawMethod drawMethod = DrawMethod.Tab;

        // notify for quest progress drawer
        public bool silent = false;

        public QuestGroupDetail(string name) : base(name) { }
    }

    public enum DrawMethod { Table, Tab }
    public enum QuestGroupType { List, Slot }
    public enum QuestRefeshType { None, OnTime, OnCompleted }

    [Serializable]
    public class QuestRefreshConfig
    {
        public QuestRefeshType type;
        public PeriodConfig period = new PeriodConfig();
        public int maxQuest = -1;

        public bool Enabled => type != QuestRefeshType.None;
    }
}