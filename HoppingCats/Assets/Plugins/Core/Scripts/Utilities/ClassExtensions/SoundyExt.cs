using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.Audio;

namespace moonNest
{
    public partial class Soundy
    {
        static readonly Dictionary<int, AudioMixer> audioMixers = new Dictionary<int, AudioMixer>();
        static readonly Dictionary<int, Tweener> dts = new Dictionary<int, Tweener>();

        public static string MusicParam => GlobalConfig.Ins.musicParam;
        public static string SfxParam => GlobalConfig.Ins.sfxParam;
        public static string IngameSfxParam => GlobalConfig.Ins.ingameSfxParam;

        public static void ToUnmute(string volumeParam, float duration = 0.5f)
        {
            int code = volumeParam.GetHashCode();
            if(dts.TryGetValue(code, out var dt)) DOTween.Kill(dt);
            AudioMixer mixer = GlobalConfig.Ins.mainMixer.audioMixer;
            mixer.GetFloat(volumeParam, out var current);
            dts[code] = DOTween.To(() => current, val => mixer.SetFloat(volumeParam, val), 0f, duration).SetEase(Ease.Linear);
        }

        public static void ToMute(string volumeParam, float duration = 0.5f)
        {
            int code = volumeParam.GetHashCode();
            if(dts.TryGetValue(code, out var dt)) DOTween.Kill(dt);
            var mixer = GlobalConfig.Ins.mainMixer.audioMixer;
            mixer.GetFloat(volumeParam, out var current);
            dts[code] = DOTween.To(() => current, val => mixer.SetFloat(volumeParam, val), -80f, duration).SetEase(Ease.Linear);
        }

        public static void SetVolume(string volumeParam,float volume)
        {
            var mixer = GlobalConfig.Ins.mainMixer.audioMixer;
            mixer.SetFloat(volumeParam, volume);
        }

        public static float GetVolume(string volumeParam)
        {
            float volume = 0.0f;
            var mixer = GlobalConfig.Ins.mainMixer.audioMixer;
            mixer.GetFloat(volumeParam, out volume);
            return volume;
        }
    }
}
