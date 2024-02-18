using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace moonNest
{
    public class UserPropertyAsset : SingletonScriptObject<UserPropertyAsset>
    {
        public UserPropertyDefinition properties = new UserPropertyDefinition();

        private Dictionary<int, StatDefinition> stats = new Dictionary<int, StatDefinition>();

#if UNITY_EDITOR
        /// <summary>
        /// Invoke when asset loaded
        /// </summary>
        void Init()
        {
            if(properties.Init())
            {
                EditorUtility.SetDirty(UserPropertyAsset.Ins);
            }
        }
#endif

        public StatDefinition FindStat(int statId)
        {
            if(!stats.TryGetValue(statId, out var stat))
            {
                StatDefinition _stat = properties.FindStat(statId);
                stats[statId] = _stat;
                stat = _stat;
            }

            return stat;
        }
    }
}