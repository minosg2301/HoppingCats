using System;
using System.Collections.Generic;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class QuestDetail : BaseData
    {
        public int groupId;
        public string displayName;
        public string description;
        public Sprite icon;
        public bool activeOnLoad = true;
        public bool removeOnClaimed;
        public bool showAlways;
        public int serverId;
        public ActionRequire require = new ActionRequire();

        //Multi action + bool check
        public ActionRequires requires = new ActionRequires();
        public bool useMultiActions;

        public StatRequireDetail statRequire = new StatRequireDetail();
        public RewardDetail reward = new RewardDetail("");

        // quest in same group will be actived when this quest completed
        public List<int> activeOnCompleteds = new List<int>();

        public int slot;

        public int point;
        public bool pointEnabled = false;

        // notify for quest progress drawer
        public bool silent = false;

        //Navigation
        public bool isNavigation;
        public int navigationId;

        public QuestDetail(string name, int groupId) : base(name)
        {
            this.groupId = groupId;
        }
    }
}