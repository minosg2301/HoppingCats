using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace moonNest.editor
{
    public class DrawUtilities
    {

        public static void AddSymbols(BuildTarget[] targets, params string[] defines)
        {
            var _defines = defines.ToList();
            foreach (var target in targets)
                AddSymbols(target, _defines);
        }

        public static void AddSymbols(BuildTarget target, params string[] defines)
        {
            AddSymbols(target, defines.ToList());
        }

        public static void AddSymbolsOnAllPlatform(params string[] defines)
        {
            AddSymbols(BuildTarget.Android, defines.ToList());
            AddSymbols(BuildTarget.iOS, defines.ToList());
            AddSymbols(BuildTarget.WebGL, defines.ToList());
        }

        public static void AddSymbols(BuildTarget target, List<string> defines)
        {
            BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(target);
            if (group == BuildTargetGroup.Unknown) return;

            var defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';').Select(d => d.Trim()).ToList();
            defines = defines.FindAll(d => !defineSymbols.Contains(d));
            if (defines.Count > 0)
            {
                defineSymbols.AddRange(defines);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", defineSymbols.ToArray()));
            }
        }

        public static void RemoveSymbols(BuildTarget[] targets, params string[] defines)
        {
            var _defines = defines.ToList();
            foreach (var target in targets)
                RemoveSymbols(target, _defines);
        }

        public static void RemoveSymbols(BuildTarget target, params string[] defines)
        {
            RemoveSymbols(target, defines.ToList());
        }

        public static void RemoveSymbolsOnAllPlatform(params string[] defines)
        {
            RemoveSymbols(BuildTarget.Android, defines.ToList());
            RemoveSymbols(BuildTarget.iOS, defines.ToList());
            RemoveSymbols(BuildTarget.WebGL, defines.ToList());
        }

        public static void RemoveSymbols(BuildTarget target, List<string> defines)
        {
            BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(target);
            if (group == BuildTargetGroup.Unknown) return;

            var defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';').Select(d => d.Trim()).ToList();
            defines = defines.FindAll(d => defineSymbols.Contains(d));
            if (defines.Count > 0)
            {
                defineSymbols.RemoveAll(defines);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", defineSymbols.ToArray()));
            }
        }
    }
}