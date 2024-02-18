using System;
using UnityEngine;
using UnityEngine.Events;

namespace moonNest
{
    public abstract class BootStep : MonoBehaviour
    {
        [SerializeField] private string status = "";
        [SerializeField] private bool checkInternet = false;
        [SerializeField] private bool calledOnce = false;
        [Space]
        [SerializeField] private UnityEvent onStepEvent;
        [SerializeField] private UnityEvent onStepCompleted;

        Action onCompleted = delegate { };
        Action onCancel = delegate { };
        public Action<string> onStatusUpdate = delegate { };
        public Action<float, string> onProgressUpdate = delegate { };

        public bool CalledOnlyOnce
        {
            get => calledOnce;
            protected set { calledOnce = value; }
        }

        public string Status => string.IsNullOrEmpty(status)
                    ? ""
                    : status.StartsWith("TXT_")
                    ? status.ToLocalized()
                    : status;

        public int StepOrder { get; internal set; } = 0;

        public abstract void OnStep();

        public async void StepIn(Action onCompleted, Action onCancel)
        {
            this.onCancel = onCancel;
            this.onCompleted = onCompleted;

            if (checkInternet)
            {
                bool haveInternet = await InternetConnectionWatcher.CheckInternet();
                if (!haveInternet) Cancel();
                else MainThreadDispatcher.Enqueue(UpdateOnStep);
                return;
            }

            UpdateOnStep();
        }

        void UpdateOnStep()
        {
            onStepEvent.Invoke();
            OnStep();
        }

        public void Complete()
        {
            onCompleted();
            onStepCompleted.Invoke();
        }

        public void Cancel() => onCancel();

        public override string ToString() => "Boot Step";
    }
}