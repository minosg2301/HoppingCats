using UnityEditor;
using UnityEngine.UI;

namespace moonNest
{
    [CustomEditor(typeof(GridLayoutGroupExt))]
    public class GridLayoutGroupExtEditor : Editor
    {
        //%%%% Context menu for editor %%%%
        [MenuItem("CONTEXT/GridLayoutGroup/Convert To GridLayoutGroupExt", true)]
        private static bool _ConvertToSoftMask(MenuCommand command)
        {
            return EditorUtils.CanConvertTo<GridLayoutGroupExt>(command.context);
        }

        [MenuItem("CONTEXT/GridLayoutGroup/Convert To GridLayoutGroupExt", false)]
        private static void ConvertToSoftMask(MenuCommand command)
        {
            EditorUtils.ConvertTo<GridLayoutGroupExt>(command.context);
        }

        [MenuItem("CONTEXT/GridLayoutGroup/Convert To GridLayoutGroup", true)]
        private static bool _ConvertToMask(MenuCommand command)
        {
            return EditorUtils.CanConvertTo<GridLayoutGroup>(command.context);
        }

        [MenuItem("CONTEXT/GridLayoutGroup/Convert To GridLayoutGroup", false)]
        private static void ConvertToMask(MenuCommand command)
        {
            EditorUtils.ConvertTo<GridLayoutGroup>(command.context);
        }
    }
}