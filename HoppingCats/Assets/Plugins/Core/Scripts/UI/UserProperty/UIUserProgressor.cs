using DG.Tweening;
using Doozy.Engine.Progress;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace moonNest
{
    public class UIUserProgressor : MonoBehaviour, IObserver
    {
        public UserStatId stat = -1;
        public TextMeshProUGUI target;
        public string prefix = "";
        public string suffix = "";
        public bool autoUpdate;
        public Progressor progressor;
        public RectTransform lineContainer;
        public RectTransform cursor;
        public ProgressTargetImage targetImage;
        public UIRewardDetail rewards;
        public Progressor bufferProgressor;

        private StatProgressGroupDetail progressGroup;
        private int lastValue;
        private bool useBuffer;
        private int bufferStat = -1;

        #region unity methods
        void Reset()
        {
            if(!target) target = GetComponent<TextMeshProUGUI>();
            if(!progressor) progressor = GetComponent<Progressor>();
        }

        void OnValidate()
        {
            StatDefinition statDef = UserPropertyAsset.Ins.FindStat(stat);
            gameObject.name = "User Stat Progress - " + (statDef ? statDef.name : "");
            if(target) target.text = prefix + "-" + suffix;
        }

        void Start()
        {
            if(stat.id != -1)
            {
                lastValue = UserData.Stat(stat);
                progressGroup = StatProgressAsset.Ins.FindGroupByStat(stat);
                if(progressGroup && progressor)
                {
                    Range range = progressGroup.GetRange(lastValue);
                    SetRange(progressor, range, lastValue);

                    bufferStat = (progressGroup.type == StatProgressType.Active ? progressGroup.bufferedStatId : progressGroup.bufferedProgressId);
                    useBuffer = bufferStat != -1 && bufferProgressor;
                    if(useBuffer)
                    {
                        int bufferValue = UserData.Stat(bufferStat);
                        SetRange(bufferProgressor, range, bufferValue);
                    }

                    if(!autoUpdate) UpdateProgress();

                    UserData.Ins.Subscribe(this, progressGroup.statId.ToString(), progressGroup.progressId.ToString());
                }
            }
        }
        #endregion

        public void OnNotify(IObservable data, string[] scopes)
        {
            if(autoUpdate) UpdateProgress();
        }

        public void UpdateProgress()
        {
            if(!progressGroup) return;

            if(progressGroup.accumulatable) UpdateAccumulatedProgress();
            else UpdateNormalProgress();

            UpdateReward();
        }

        #region private methods
        /// <summary>
        /// Update normal progress, aka non-accumulatable progress
        /// </summary>
        void UpdateNormalProgress()
        {
            // TODO: Update normal progress, aka non-accumulatable progress
        }

        void UpdateAccumulatedProgress()
        {
            int currentValue = UserData.Stat(progressGroup.statId);
            int progressValue = UserData.Stat(progressGroup.progressId);
            if(lastValue == currentValue)
            {
                progressor.SetValue(progressValue);
                UpdateLines();

                if(useBuffer)
                    bufferProgressor.SetValue((int)UserData.Stat(bufferStat));
            }
            else if(lastValue < currentValue) // ---> upgrade
            {
                progressor.SetValue(progressor.MaxValue);
                lastValue = currentValue;
                DOVirtual.DelayedCall(progressor.AnimationDuration, () =>
                {
                    Range range = progressGroup.GetRange(lastValue);
                    progressor.SetValue(range.min, true);
                    SetRange(progressor, range, progressValue);
                    SetTargetText(lastValue.ToString());
                    UpdateLines();

                    if(useBuffer)
                        SetRange(bufferProgressor, range, UserData.Stat(bufferStat));
                });
            }
            else // lastValue > currentValue ---> downgrade
            {
                progressor.SetValue(progressor.MinValue);
                lastValue = currentValue;
                DOVirtual.DelayedCall(progressor.AnimationDuration, () =>
                {
                    Range range = progressGroup.GetRange(lastValue);
                    progressor.SetValue(range.max, true);
                    SetRange(progressor, range, progressValue);
                    SetTargetText(lastValue.ToString());
                    UpdateLines();

                    if(useBuffer)
                        SetRange(bufferProgressor, range, UserData.Stat(bufferStat));
                });
            }
        }

        void SetRange(Progressor progressor, Range range, int value)
        {
            progressor.SetMax(range.max);
            progressor.SetMin(range.min);
            progressor.SetValue(value);
        }

        void UpdateLines()
        {
            if(!lineContainer) return;

            int progressValue = UserData.Stat(progressGroup.progressId);
            Rect rect = lineContainer.rect;
            List<RectTransform> lines = lineContainer.GetComponentsInChildren<RectTransform>(true).ToList();
            lines.Remove(lineContainer);
            lines.ForEach(line => line.gameObject.SetActive(false));
            List<ProgressDetail> progresses = progressGroup.FindLinkedProgresses(lastValue);
            int requireValue = useBuffer ? (int)UserData.Stat(bufferStat) : progressValue;
            ProgressDetail nextProgress = progresses.Find(progress => progress.requireValue > requireValue);
            if(targetImage.Image.fillMethod == Image.FillMethod.Vertical)
            {
                for(int i = 1; i < progresses.Count; i++)
                {
                    if(i >= lines.Count) lines.Add(Instantiate(lines[0], lineContainer));

                    float percent = (float)i / progresses.Count;
                    RectTransform line = lines[i];
                    line.gameObject.SetActive(true);
                    line.anchoredPosition = new Vector2(0, rect.y + percent * rect.height);

                    if(nextProgress == progresses[i] && cursor)
                    {
                        cursor.anchoredPosition = line.anchoredPosition;
                    }
                }

                if(cursor && !nextProgress)
                {
                    cursor.anchoredPosition = new Vector2(0, rect.y + rect.height);
                }
            }
            else
            {
                for(int i = 1; i < progresses.Count; i++)
                {
                    if(i >= lines.Count) lines.Add(Instantiate(lines[0], lineContainer));

                    float percent = (float)i / progresses.Count;
                    RectTransform line = lines[i];
                    line.gameObject.SetActive(true);
                    line.anchoredPosition = new Vector2(rect.x + percent * rect.width, 0);

                    if(nextProgress == progresses[i] && cursor)
                    {
                        cursor.anchoredPosition = line.anchoredPosition;
                    }
                }

                if(cursor && !nextProgress)
                {
                    cursor.anchoredPosition = new Vector2(rect.x + rect.width, 0);
                }
            }
        }

        void SetTargetText(string text)
        {
            if(target) target.text = (prefix.Length > 0 ? prefix.ToLocalized() : "") + text + (suffix.Length > 0 ? suffix.ToLocalized() : "");
        }

        void UpdateReward()
        {
            if(!rewards) return;

            if(progressGroup.type == StatProgressType.Passive)
            {
                List<ProgressDetail> progresses = progressGroup.linkedProgress ? progressGroup.FindLinkedProgresses(lastValue) : progressGroup.progresses;
                if(progressGroup.accumulatable)
                {
                    int requireValue = useBuffer ? UserData.Stat(bufferStat) : UserData.Stat(progressGroup.progressId);
                    ProgressDetail nextProgress = progresses.Find(progress => progress.requireValue > requireValue);
                    if(nextProgress) rewards.SetData(nextProgress.reward);
                }
                else
                {
                    int statValue = UserData.Stat(progressGroup.statId);
                    ProgressDetail nextProgress = progresses.Find(progress => progress.statValue > statValue);
                    if(nextProgress) rewards.SetData(nextProgress.reward);
                }
            }
            else
            {
                int requireValue = UserData.Stat(progressGroup.statId);
                ProgressDetail nextProgress = progressGroup.progresses.Find(progress => progress.requireValue > requireValue);
                if(nextProgress) rewards.SetData(nextProgress.reward);
            }
        }
        #endregion
    }
}