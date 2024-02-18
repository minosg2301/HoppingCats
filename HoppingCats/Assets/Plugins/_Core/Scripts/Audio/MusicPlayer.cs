using DG.Tweening;
using Doozy.Engine.Soundy;
using UnityEngine;

namespace moonNest
{
    public class MusicPlayer : MonoBehaviour
    {
        public bool playWithFading = true;
        public float fadingDuration = 0.5f;
        public float delayOnStart = 0f;
        public bool playOnStart = true;
        public bool loop = false;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(-3f, 3f)]
        public float pitch = 1f;
        [Range(0f, 1f)]
        public float spatialBlend = 0f;

        public SoundyData musicData;

        static SoundyData playingMusicData;
        static SoundyController playingMusicController;
        static SoundyController fadingOutMusicController;

        bool initialized = false;

        void Start()
        {
            initialized = true;
            if (playOnStart)
            {
                if (delayOnStart > 0) DOVirtual.DelayedCall(delayOnStart, Play);
                else Play();
            }
        }

        void OnEnable()
        {
            if (playOnStart && initialized)
            {
                if (delayOnStart > 0) DOVirtual.DelayedCall(delayOnStart, Play);
                else Play();
            }
        }

        public void Play()
        {
            if (playingMusicData != null
                && playingMusicData.DatabaseName == musicData.DatabaseName
                && playingMusicData.SoundName == musicData.SoundName)
                return;

            if (playWithFading)
            {
                if (playingMusicController)
                {
                    fadingOutMusicController = playingMusicController;
                    FadeOut(fadingOutMusicController, fadingDuration);
                }

                playingMusicData = musicData;
                playingMusicController = Play(musicData);
                FadeIn(playingMusicController, playingMusicController.AudioSource.volume, fadingDuration);
            }
            else
            {
                playingMusicController = Play(musicData);
            }
        }

        public void Stop(bool withFading = true)
        {
            if (!playingMusicController) return;

            if (withFading)
            {
                fadingOutMusicController = playingMusicController;
                FadeOut(fadingOutMusicController, fadingDuration);
            }
            else
            {
                playingMusicController.Stop();
            }

            playingMusicData = null;
            playingMusicController = null;
        }

        static void FadeIn(SoundyController soundyController, float endValue, float duration)
        {
            var audioSource = soundyController.AudioSource;
            audioSource.volume = 0;
            DOTween.To(() => 0, val => audioSource.volume = val, endValue, duration).SetEase(Ease.Linear);
        }

        static void FadeOut(SoundyController soundyController, float duration)
        {
            var audioSource = soundyController.AudioSource;
            DOTween.To(() => audioSource.volume, val => audioSource.volume = val, 0, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() => soundyController.Stop());
        }

        SoundyController Play(SoundyData soundyData)
        {
            switch (soundyData.SoundSource)
            {
                case SoundSource.Soundy:
                    return SoundyManager.Play(soundyData.DatabaseName, soundyData.SoundName);
                case SoundSource.AudioClip:
                    return SoundyManager.Play(soundyData.AudioClip, soundyData.OutputAudioMixerGroup, null, volume, pitch, loop, spatialBlend);
                case SoundSource.MasterAudio:
                    if (GlobalConfig.Ins.VerboseLog) Debug.Log("Play '" + soundyData.SoundName + "' with MasterAudio");
                    break;
            }
            return null;
        }
    }
}