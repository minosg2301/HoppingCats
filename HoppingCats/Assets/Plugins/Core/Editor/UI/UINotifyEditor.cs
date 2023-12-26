using UnityEditor;
using moonNest;

[CustomEditor(typeof(UINotify))]
public class UINotifyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        UINotify notify = (target as UINotify);
        Draw.PropertyField(serializedObject, "notify");
        switch(notify.notify)
        {
            case NotifyType.Achievement: Draw.PropertyField(serializedObject, "achievementGroupId"); break;
            case NotifyType.Quest: Draw.PropertyField(serializedObject, "questGroupId"); break;
            case NotifyType.Shop: Draw.PropertyField(serializedObject, "shopId"); break;
            case NotifyType.IAP: Draw.PropertyField(serializedObject, "iapGroupId"); break;
        }
        serializedObject.ApplyModifiedProperties();
    }
}
