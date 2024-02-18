using UnityEditor;
using UnityEngine;

namespace moonNest.editor
{
    [CustomPropertyDrawer(typeof(TutorialId))]
    public class TutorialIdDrawer : BasePropertyDrawer
    {
        public override void DoDrawProperty()
        {
            rect = PrefixLabel(rect, property.displayName);
            DrawIntPopup(rect, "id", TutorialAsset.Ins.tutorials, "name", "id");
        }
    }

    [CustomPropertyDrawer(typeof(TutorialStepId))]
    public class TutorialStepIdDrawer : BasePropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyPath.Contains("Array")) return 40;
            else return 60;
        }

        public override void DoDrawProperty()
        {
            if (!property.propertyPath.Contains("Array"))
            {
                EditorGUI.LabelField(rect, property.displayName);
                rect = NextLine(rect);
            }

            rect = PrefixLabel(rect, "Tutorial Id");
            int tutorialId = DrawIntPopup(rect, "tutorialId", TutorialAsset.Ins.tutorials, "name", "id");
            if (tutorialId != -1)
            {
                rect = NextLine(rect);
                rect = PrefixLabel(rect, "Tutorial Step Id");
                DrawIntPopup(rect, "stepId", TutorialAsset.Ins.FindSteps(tutorialId), "name", "id");
            }
        }
    }
}