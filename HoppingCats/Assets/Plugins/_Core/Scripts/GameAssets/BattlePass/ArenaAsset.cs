using System;
using System.Collections.Generic;

namespace moonNest
{
    public class ArenaAsset : SingletonScriptObject<ArenaAsset>
    {
        public int leagueStatId = -1;
        public int seasonDuration = 30;
        public List<LeagueDetail> leagues = new List<LeagueDetail>();

        public int requireStatId = -1;
        public int premiumStatId = -1;
        public string requireDescription = "";
        public List<BattlePassLevel> levels = new List<BattlePassLevel>();
        public BattlePassLevel finalLevel;

        public bool layerEnabled;
        public int layerGroupId = -1;

        public int MaxLevel => finalLevel.level;

        public LeagueDetail FindLeague(int league) => leagues.Find(l => l.league == league);
        public LeagueDetail FindLeagueByStatValue(int statValue) => leagues.Find(l => l.statValue == statValue);
        public BattlePassLevel FindLevel(int level) => levels.Find(l => l.level == level);

        #region layer methods
        public BattlePassLevelLayer GetLevelLayer(LayerDetail layer, int level)
        {
            if(!layer || !layerEnabled) return null;

            var actualLayer = layer.arenaLinkedLayer == -1 ? layer : GetLayerById(layer.arenaLinkedLayer);
            if(!actualLayer) return null;

            return actualLayer.battlePassLevels.Find(l => l.level == level);
        }

        public LayerDetail GetActiveLayer()
        {
            return layerEnabled ? LayerHelper.GetActiveLayer(layerGroupId) : null;
        }

        public LayerDetail GetLayerById(int layerId)
        {
            if(!layerEnabled || layerGroupId == -1) return null;
            return LayerAsset.Ins.FindLayer(layerGroupId, layerId);
        }
        #endregion
    }

    [Serializable]
    public class LeagueDetail : ICloneable
    {
        public string name;
        public string displayName;
        public int league;
        public int statValue;
        public RewardDetail reward;

        public LeagueDetail() { }

        public LeagueDetail(ProgressDetail progress)
        {
            name = progress.name;
            displayName = progress.displayName;
            statValue = progress.statValue;
        }

        public object Clone()
        {
            return null;
        }
    }
}