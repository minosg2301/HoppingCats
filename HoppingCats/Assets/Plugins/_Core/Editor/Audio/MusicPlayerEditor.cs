using Doozy.Engine.Soundy;
using UnityEditor;
using moonNest;

[CustomEditor(typeof(MusicPlayer))]
[CanEditMultipleObjects]
public class MusicPlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MusicPlayer musicPlayer = target as MusicPlayer;

        Draw.PropertyField(serializedObject, "playWithFading");
        if (musicPlayer.playWithFading) Draw.PropertyField(serializedObject, "fadingDuration");

        Draw.PropertyField(serializedObject, "playOnStart");
        if (musicPlayer.playOnStart) Draw.PropertyField(serializedObject, "delayOnStart");

        if (musicPlayer.musicData.SoundSource != SoundSource.Soundy)
        {
            Draw.PropertyField(serializedObject, "loop");
            Draw.PropertyField(serializedObject, "volume");
            Draw.PropertyField(serializedObject, "pitch");
            Draw.PropertyField(serializedObject, "spatialBlend");
        }

        Draw.PropertyField(serializedObject, "musicData");
    }
}