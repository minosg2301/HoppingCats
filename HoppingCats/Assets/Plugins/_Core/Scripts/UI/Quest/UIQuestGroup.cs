using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace moonNest
{
    public class UIQuestGroup : MonoBehaviour
    {
        public QuestGroupId groupId;
        public GameObject questContainer;
        public UICountDownTime refeshTimeText;
        public bool statusSorting;
        public bool autoConsumeReward = true;

        [Header("Point")]
        public UIQuestPointProgress pointProgress;
        public Image pointImage;
        public AudioSource pointSfx;
        public bool destroyRedundant = false;

        protected UIQuest questClaimed;
        protected UIListContainer<Quest, UIQuest> listContainer = new UIListContainer<Quest, UIQuest>();

        private TickInterval _tickInterval;
        public TickInterval TickInterval { get { if (_tickInterval == null) _tickInterval = gameObject.AddComponent<TickInterval>(); return _tickInterval; } }
        public QuestGroup QuestGroup { get; private set; }

        public Action<UIQuest> onQuestClaimed = delegate { };

        protected virtual void OnEnable()
        {
            if (groupId != -1)
            {
                QuestGroup = UserQuest.Ins.FindGroup(groupId);
                QuestGroup.onPointUpdated = OnPointUpdated;
                UserQuest.Ins.Subscribe("Refresh" + groupId.ToString(), OnGroupRefeshed);
                UserQuest.Ins.Subscribe(groupId.ToString(), OnGroupUpdated);
            }
        }

        protected virtual void OnDisable()
        {
            if (groupId != -1)
            {
                UserQuest.Ins.Unsubscribe("Refresh" + groupId.ToString(), OnGroupRefeshed);
                UserQuest.Ins.Unsubscribe(groupId.ToString(), OnGroupUpdated);
            }
        }

        protected virtual void OnGroupRefeshed(BaseUserData obj)
        {
            if (questContainer)
            {
                listContainer.destroyRedundant = destroyRedundant;
                listContainer.SetList(questContainer.transform, UserQuest.Ins.FindByGroup(groupId), ui =>
                {
                    ui.AutoConsumeReward = autoConsumeReward;
                    ui.onQuestClaimed = OnQuestClaimed;
                });

                if (refeshTimeText)
                {
                    refeshTimeText.gameObject.SetActive(QuestGroup.IsRefreshOnTime);
                    refeshTimeText.StartWithDuration((float)QuestGroup.LastSeconds);
                }
            }
        }

        protected virtual void OnGroupUpdated(BaseUserData obj)
        {
            if (pointProgress)
            {
                pointProgress.SetQuestGroup(QuestGroup);
                pointProgress.SetValue(QuestGroup.Point);
            }

            if (statusSorting)
            {
                listContainer.UIList.SortDesc((quest) => (float)((float)quest.Quest.Progress.value / (float)quest.Quest.RequireValue));
                listContainer.UIList.Sort((uiQuestA, uiQuestB) =>
                {
                    int A = uiQuestA.Quest.CanClaim ? (uiQuestA.Quest.Claimed ? 1 : -1) : 0;
                    int B = uiQuestB.Quest.CanClaim ? (uiQuestB.Quest.Claimed ? 1 : -1) : 0;
                    return A - B;
                });

                listContainer.UIList.ForEach((uiQuest, i) => uiQuest.transform.SetSiblingIndex(i));
            }
        }

        protected virtual void OnQuestClaimed(UIQuest uiQuest)
        {
            onQuestClaimed(uiQuest);
            questClaimed = uiQuest;
        }

        protected virtual void OnPointUpdated(int lastPoint, int currentPoint)
        {
            int value = currentPoint - lastPoint;
            if (value <= 0) return;

            if (pointProgress && pointImage && questClaimed && questClaimed.pointRewardText)
            {
                Vector3 target = pointImage.transform.position;
                float delay = 0;
                for (int i = 0; i < value; i++)
                {
                    Image key = PoolSystem.RequireObject(pointImage);
                    key.transform.SetParent(transform);
                    key.transform.localScale = pointImage.transform.localScale * 0.5f;
                    key.transform.position = questClaimed.pointRewardText.transform.position;
                    key.transform.DOMove(target, 0.6f)
                        .SetDelay(delay)
                        .OnComplete(() =>
                        {
                            key.gameObject.SetActive(false);
                            if (pointSfx && pointSfx.isActiveAndEnabled)
                                pointSfx.Play();
                        });
                    delay += 0.1f;
                }
            }
        }
    }
}