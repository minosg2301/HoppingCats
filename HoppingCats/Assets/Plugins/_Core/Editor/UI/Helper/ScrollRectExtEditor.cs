using UnityEditor;
using UnityEditor.UI;
using UnityEngine.UI;

namespace moonNest
{
    [CustomEditor(typeof(ScrollRectExt))]
    public class ScrollRectExtEditor : ScrollRectEditor
    {
        //%%%% Context menu for editor %%%%
        [MenuItem("CONTEXT/ScrollRect/Convert To ScrollRectExt", true)]
        private static bool _ConvertToSoftMask(MenuCommand command)
        {
            return EditorUtils.CanConvertTo<ScrollRectExt>(command.context);
        }

        [MenuItem("CONTEXT/ScrollRect/Convert To ScrollRectExt", false)]
        private static void ConvertToSoftMask(MenuCommand command)
        {
            EditorUtils.ConvertTo<ScrollRectExt>(command.context);
        }

        [MenuItem("CONTEXT/ScrollRect/Convert To ScrollRect", true)]
        private static bool _ConvertToMask(MenuCommand command)
        {
            return EditorUtils.CanConvertTo<ScrollRect>(command.context);
        }

        [MenuItem("CONTEXT/ScrollRect/Convert To ScrollRect", false)]
        private static void ConvertToMask(MenuCommand command)
        {
            EditorUtils.ConvertTo<ScrollRect>(command.context);
        }
    }
}