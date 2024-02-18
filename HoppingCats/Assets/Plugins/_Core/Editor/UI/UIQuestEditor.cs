using UnityEditor;
using moonNest;

[CustomEditor(typeof(UIQuest))]
public class UIQuestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Draw.PropertyField(serializedObject, "questId");
        Draw.PropertyField(serializedObject, "nameText");
        Draw.PropertyField(serializedObject, "descriptionText");
        Draw.PropertyField(serializedObject, "pointRewardText");
        Draw.PropertyField(serializedObject, "rewardsContainer");

        Draw.SpaceAndLabelBold("In Progress");
        Draw.PropertyField(serializedObject, "inProgressNode");
        Draw.PropertyField(serializedObject, "progress");
        Draw.PropertyField(serializedObject, "progressor");

        Draw.SpaceAndLabelBold("Claim Button");
        Draw.PropertyField(serializedObject, "claimButton");
        Draw.PropertyField(serializedObject, "hideClaimButtonWhenInProgress", "Hide In Progress");
        Draw.PropertyField(serializedObject, "hideClaimButtonWhenClaimed", "Hide After Claimed");

        Draw.SpaceAndLabelBold("Claimed Reward");
        Draw.PropertyField(serializedObject, "claimedNode");

        Draw.SpaceAndLabelBold("Navigation");
        Draw.PropertyField(serializedObject, "navigationButton");

        serializedObject.ApplyModifiedProperties();
    }
}