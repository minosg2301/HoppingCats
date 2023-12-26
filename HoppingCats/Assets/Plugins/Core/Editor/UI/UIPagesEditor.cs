using UnityEditor;

namespace moonNest.editor
{
    [CustomEditor(typeof(UIPages))]
    [CanEditMultipleObjects]
    public class UIPagesEditor : UITabEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}