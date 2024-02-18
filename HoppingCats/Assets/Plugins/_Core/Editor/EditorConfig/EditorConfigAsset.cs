using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace moonNest
{
    public class EditorConfigAsset : SingletonScriptObject<EditorConfigAsset>
    {
        public bool neverAskUploadSymbol;

        public ExportConfig exportConfig = new ExportConfig();
        [SerializeField] List<ScopeConfig> scopes = new List<ScopeConfig>();

        public static ScopeConfig Get(string scope)
        {
            return Ins.GetOrCreate(scope);
        }

        public ScopeConfig GetOrCreate(string scope)
        {
            var scopeConfig = scopes.Find(s => s.Scope == scope);
            if(scopeConfig == null)
            {
                scopeConfig = new ScopeConfig(scope);
                scopes.Add(scopeConfig);

                EditorUtility.SetDirty(Ins);
            }
            return scopeConfig;
        }
    }

    [Serializable]
    public class ExportConfig
    {
        public string exportFileId = "";
        public bool exportChest;
        public bool exportArena;
        public bool exportBattlePass;
    }

    [Serializable]
    public class ScopeConfig
    {
        [SerializeField] private string scope = "";
        [SerializeField] List<IntConfig> intConfigs = new List<IntConfig>();
        [SerializeField] List<StringConfig> stringConfigs = new List<StringConfig>();

        public string Scope => scope;

        public ScopeConfig(string scope)
        {
            this.scope = scope;
        }

        public string Get(string key, string defValue)
        {
            var config = stringConfigs.Find(c => c.nameCode == key.GetHashCode());
            if(config == null)
            {
                config = new StringConfig(key.GetHashCode(), defValue);
                stringConfigs.Add(config);

                EditorUtility.SetDirty(EditorConfigAsset.Ins);
            }
            return config.value;
        }

        public void Set(string key, string value)
        {
            var config = stringConfigs.Find(c => c.nameCode == key.GetHashCode());
            if(config != null)
            {
                config.value = value;
                EditorUtility.SetDirty(EditorConfigAsset.Ins);
            }
        }

        public int Get(string key, int defValue)
        {
            var config = intConfigs.Find(c => c.nameCode == key.GetHashCode());
            if(config == null)
            {
                config = new IntConfig(key.GetHashCode(), defValue);
                intConfigs.Add(config);

                EditorUtility.SetDirty(EditorConfigAsset.Ins);
            }
            return config.value;
        }

        public void Set(string key, int value)
        {
            var config = intConfigs.Find(c => c.nameCode == key.GetHashCode());
            if(config != null)
            {
                config.value = value;
                EditorUtility.SetDirty(EditorConfigAsset.Ins);
            }
        }
    }

    [Serializable]
    internal class StringConfig
    {
        public int nameCode;
        public string value;

        public StringConfig(int nameCode, string value)
        {
            this.nameCode = nameCode;
            this.value = value;
        }
    }

    [Serializable]
    internal class IntConfig
    {
        public int nameCode;
        public int value;

        public IntConfig(int nameCode, int value)
        {
            this.nameCode = nameCode;
            this.value = value;
        }
    }
}