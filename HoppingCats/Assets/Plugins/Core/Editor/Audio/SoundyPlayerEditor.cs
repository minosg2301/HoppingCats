using Doozy.Engine.Soundy;
using UnityEditor;
using moonNest;

[CustomEditor(typeof(SoundyPlayer))]
[CanEditMultipleObjects]
public class SoundyPlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SoundyPlayer soundyPlayer = target as SoundyPlayer;

        Draw.PropertyField(serializedObject, "singleChannel");
        if (soundyPlayer.singleChannel) Draw.PropertyField(serializedObject, "skipDuration");

        Draw.PropertyField(serializedObject, "playOnStart");
        if (soundyPlayer.playOnStart) Draw.PropertyField(serializedObject, "delayOnStart");

        if(soundyPlayer.soundyData.SoundSource != SoundSource.Soundy)
        {
            Draw.PropertyField(serializedObject, "loop");
            Draw.PropertyField(serializedObject, "volume");
            Draw.PropertyField(serializedObject, "pitch");
            Draw.PropertyField(serializedObject, "spatialBlend");
        }

        Draw.PropertyField(serializedObject, "soundyData");
    }
}