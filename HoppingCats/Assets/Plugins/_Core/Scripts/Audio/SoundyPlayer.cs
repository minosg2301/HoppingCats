using DG.Tweening;
using Doozy.Engine.Soundy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace moonNest
{
    public class SoundyPlayer : MonoBehaviour
    {
        public bool singleChannel = false;
        public float skipDuration = 0.1f;
        public float delayOnStart = 0f;
        public bool playOnStart = false;
        public bool loop = false;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(-3f, 3f)]
        public float pitch = 1f;
        [Range(0f, 1f)]
        public float spatialBlend = 0f;

        public SoundyData soundyData;

        bool initialized = false;
        SoundyController soundController;

        int _id = -1;
        public int Id
        {
            get
            {
                if (_id == -1)
                {
                    switch (soundyData.SoundSource)
                    {
                        case SoundSource.Soundy:
                            _id = soundyData.DatabaseName.GetHashCode() + soundyData.SoundName.GetHashCode();
                            break;
                        case SoundSource.AudioClip:
                            _id = soundyData.AudioClip.name.GetHashCode();
                            break;
                    }
                }

                return _id;
            }
        }

        #region static
        static readonly Dictionary<int, bool> playingChannels = new Dictionary<int, bool>();
        static bool CheckSingleChannel(int key)
        {
            return playingChannels.TryGetValue(key, out bool playing) && playing;
        }

        static void UpdateSingleChannel(int key, bool playing)
        {
            playingChannels[key] = playing;
        }
        #endregion

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
            if (!soundController)
            {
                if (singleChannel)
                {
                    if (CheckSingleChannel(Id)) return;

                    soundController = Play(soundyData);
                    if (soundController)
                    {
                        soundController.onStop = OnStop;
                        UpdateSingleChannel(Id, true);
                        StartCoroutine(IUpdateChannel(skipDuration));
                    }
                }
                else
                {
                    soundController = Play(soundyData);
                    if (soundController)
                        soundController.onStop = OnStop;
                }
            }
        }

        IEnumerator IUpdateChannel(float skipDuration)
        {
            yield return new WaitForSeconds(skipDuration);
            UpdateSingleChannel(Id, false);
        }

        public void Stop()
        {
            if (soundController)
                soundController.Stop();
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

        void OnStop()
        {
            soundController = null;
        }
    }
}