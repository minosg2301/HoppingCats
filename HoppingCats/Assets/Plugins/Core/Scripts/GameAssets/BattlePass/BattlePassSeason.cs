using System;
using System.Collections.Generic;

namespace moonNest
{
    [Serializable]
    public class BattlePassSeason : BaseData
    {
        public int requireStatId = -1;
        public int premiumStatId = -1;
        public string requireDescription = "";
        public List<BattlePassLevel> levels = new List<BattlePassLevel>();
        public BattlePassLevel finalLevel;

        public int MaxLevel => finalLevel.level;

        public BattlePassSeason(string name) : base(name)
        {
            finalLevel = new BattlePassLevel(-1);
        }

        public BattlePassLevel FindLevel(int level) => levels.Find(battlePassLevel => battlePassLevel.level == level);
    }
}