using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using moonNest.remotedata;

namespace moonNest
{
    [Preserve]
    public class UserTutorial : RemotableUserData<FirestoreUserData>
    {
        public static UserTutorial Ins => LocalData.Get<UserTutorial>();

        public bool localOnly = false;

        [SerializeField] private List<int> tutorialPlayeds = new List<int>();
        [SerializeField] private Dictionary<int, int> playingSteps = new Dictionary<int, int>();

        public event Action<int> OnTutorialCompleted = delegate { };

        protected internal override void OnLoad()
        {
            base.OnLoad();

            if (GlobalConfig.Ins.StoreRemoteData)
            {
                OnTutorialCompleted += HandleTutorialCompleted;
            }
        }

        public bool IsTutorialPlayed(int tutorialId)
        {
            return tutorialPlayeds.Contains(tutorialId);
        }

        internal void OnTutorialEnd(int tutorialId)
        {
            if (!tutorialPlayeds.Contains(tutorialId))
            {
                tutorialPlayeds.Add(tutorialId);
                OnTutorialCompleted(tutorialId);
                dirty = true;
            }
        }

        internal void ResetTutorial(int tutorialId)
        {
            tutorialPlayeds.Remove(tutorialId);
            if (GlobalConfig.Ins.StoreRemoteData)
            {
                UpdateRemoteData();
            }
            dirty = true;
        }

        internal int GetCurrentStep(int tutorialId)
        {
            return playingSteps.TryGetValue(tutorialId, out int stepId) ? stepId : -1;
        }

        internal void SetCurrentStep(int tutoriaId, int stepId)
        {
            playingSteps[tutoriaId] = stepId;
            dirty = true;
        }

        #region remote methods
        void HandleTutorialCompleted(int tutorialId)
        {
            if (localOnly) return;

            UpdateRemoteData();
        }

        private void UpdateRemoteData()
        {
            if (localOnly) return;

            RemoteData.TutorialPlayeds = tutorialPlayeds;
            RemoteData.AddRequest("TutorialPlayeds", tutorialPlayeds);
        }

        protected override void OnRemoteDataSync(FirestoreUserData remoteData)
        {
            if (localOnly) return;

            if (RemoteData.TutorialPlayeds == null)
            {
                OnRemoteDataCreated(remoteData);
                return;
            }
            tutorialPlayeds = RemoteData.TutorialPlayeds;
        }

        protected override void OnRemoteDataCreated(FirestoreUserData remoteData)
        {
            if (localOnly) return;

            RemoteData.TutorialPlayeds = tutorialPlayeds;
            RemoteData.AddRequest("TutorialPlayeds", tutorialPlayeds);
        }
        #endregion
    }
}