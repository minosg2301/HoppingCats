using Doozy.Engine;
using Doozy.Engine.SceneManagement;
using LateExe;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace moonNest
{
    public class BootLoader : MonoBehaviour
    {
        public bool bootOnStart = false;
        public float bootDelay = 0.5f;
        public string bootStatusEvent = "bootStatus";
        public string progressEvent = "progressUpdate";
        public string bootCompletedEvent = "GoToMain";
        public bool logStep = false;

        [Space]
        public SceneLoader sceneLoader;
        public UnityEvent startBootEvent;
        public UnityEvent endBootEvent;

        private List<BootStep> bootSteps;
        private BootStep currentBootStep;
        private Executer executer;
        private bool stepCancel;
        private bool initialized = false;
        private bool firstBoot = true;

        public BootStep ForceNextStep { get; set; }

        public static BootLoader Ins { get; private set; }

        void Awake()
        {
            // update scaling
            ResolutionScalingUpdate();
        }

        void Start()
        {
            Ins = this;
            GlobalConfig.Ins.SelectedProfile = Profiles.Ins.profiles.Find(p => p.id == GlobalConfig.Ins.selectedProfileId);

            if (GlobalConfig.Ins.DebugLog)
            {
                var fpsObject = new GameObject("FPS Display", typeof(FPSDisplay));
                DontDestroyOnLoad(fpsObject);
            }

            SceneManager.sceneLoaded += OnSceneLoad;

#if !UNITY_WEBGL
            Application.targetFrameRate = 60;
#endif
            Screen.sleepTimeout = GlobalConfig.Ins.screenSleepMode;
            Debug.unityLogger.logEnabled = GlobalConfig.Ins.VerboseLog || GlobalConfig.Ins.DebugLog;

            initialized = true;
            executer = new Executer(this);
            bootSteps = GetComponentsInChildren<BootStep>().ToList();
            for (int i = 0; i < bootSteps.Count; i++)
                bootSteps[i].StepOrder = i + 1;

            if (bootOnStart) StartBoot();
        }

        void OnEnable()
        {
            InternetConnectionWatcher.OnNoConnection += OnNoConnection;
            InternetConnectionWatcher.OnHaveConnection += OnHaveConnection;

            if (bootOnStart && initialized) StartBoot();
        }

        void OnDisable()
        {
            InternetConnectionWatcher.OnNoConnection -= OnNoConnection;
            InternetConnectionWatcher.OnHaveConnection -= OnHaveConnection;
        }

        public void StartBoot()
        {
            startBootEvent.Invoke();
            executer.DelayExecute(bootDelay, () =>
            {
                UpdateBootStep();
                PerformBootStep();
            });
        }

        void UpdateBootStep()
        {
            if (currentBootStep == null)
            {
                currentBootStep = bootSteps.First();
            }
            else
            {
                if (ForceNextStep)
                {
                    var prevStep = currentBootStep;
                    currentBootStep = ForceNextStep;
                    ForceNextStep = null;
                    if (prevStep.CalledOnlyOnce) bootSteps.Remove(prevStep);
                }
                else
                {
                    var prevStep = currentBootStep;
                    currentBootStep = bootSteps.Next(prevStep);
                    if (prevStep.CalledOnlyOnce) bootSteps.Remove(prevStep);
                }
            }

            if (currentBootStep)
            {
                currentBootStep.onStatusUpdate = OnStatusUpdate;
                currentBootStep.onProgressUpdate = OnProgressUpdate;
            }
        }

        void OnProgressUpdate(float progress, string content)
        {
            ProgressMessage.SendEvent(progressEvent, progress, content);
        }

        void OnStatusUpdate(string status)
        {
            EventMessage.SendEvent(bootStatusEvent, status);
        }

        void PerformBootStep()
        {
            if (currentBootStep != null)
            {
                if (GlobalConfig.Ins.VerboseLog && logStep)
                {
                    Debug.Log($"Boot - Step [{currentBootStep}]");
                }

                var status = currentBootStep.Status;
                if (!string.IsNullOrEmpty(status))
                    EventMessage.SendEvent(bootStatusEvent, status);

                currentBootStep.StepIn(onCompleted: () =>
                {
                    UpdateBootStep();
                    executer.DelayExecuteByFrame(1, PerformBootStep);
                },
                onCancel: () => stepCancel = true);
            }
            else if (sceneLoader)
            {
                if (GlobalConfig.Ins.VerboseLog && logStep)
                {
                    Debug.Log($"Boot - Completed");
                }

                if (firstBoot) sceneLoader.LoadSceneAsync();
                else BootCompleted();
            }
            else
            {
                Debug.LogError("Missing SceneLoader in BootLoader!!");
            }
        }

        void OnHaveConnection()
        {
            if (stepCancel)
            {
                executer.DelayExecuteByFrame(1, PerformBootStep);
                stepCancel = false;
            }
        }

        void OnNoConnection()
        {
            Debug.Log("BootLoader - OnNoConnection");
        }

        void OnSceneLoad(Scene arg0, LoadSceneMode arg1)
        {
            SceneManager.sceneLoaded -= OnSceneLoad;
            BootCompleted();
        }

        void BootCompleted()
        {
            firstBoot = false;

            endBootEvent.Invoke();
            QuestProgressDrawer.Open();
            GameEventMessage.SendEvent(bootCompletedEvent, null);
        }

        void ResolutionScalingUpdate()
        {
            if (!GlobalConfig.Ins.resolutionScaling) return;

#if UNITY_ANDROID
            Debug.Log($"{Screen.width} - {Screen.height} - {Screen.currentResolution.width} - {Screen.currentResolution.height}");
            CoreHandler.ScaleFactor = Mathf.Min(GlobalConfig.Ins.targetDPI / Screen.dpi, 1f);
            Size newSize = new Size(Screen.width, Screen.height) * CoreHandler.ScaleFactor;
            Screen.SetResolution((int)newSize.width, (int)newSize.height, FullScreenMode.FullScreenWindow);
            StartCoroutine(Log());
#endif
        }

        IEnumerator Log()
        {
            yield return null;
            Debug.Log($"{Screen.width} - {Screen.height} - {Screen.currentResolution.width} - {Screen.currentResolution.height}");
        }
    }
}