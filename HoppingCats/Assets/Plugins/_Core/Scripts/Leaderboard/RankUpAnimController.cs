using DG.Tweening;
using Doozy.Engine.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace moonNest
{
    public class RankUpAnimController : MonoBehaviour
    {
        [SerializeField] protected string leaderboardId;
        [SerializeField] protected LeaderboardTimespan timeSpan;
        [SerializeField] protected UILeaderboardRankUpOSA rankUpOSA;

        [Header("My Score")]
        [SerializeField] protected UILeaderboardRecord myScore;
        [SerializeField] protected DOTweenAnimation myScoreAnimation;

        [Header("Animation Config")]
        [SerializeField] protected bool manualPlay = false;
        [SerializeField] protected float startDelay = 1f;
        [SerializeField] protected float moveDuration = 1f;
        [SerializeField] protected float scrollDuration = 2f;
        [SerializeField] protected int maxVisibleRow = 4;
        [SerializeField] protected string animationStartKey = "start";
        [SerializeField] protected string animationEndKey = "end";

        [Header("Sfx")]
        [SerializeField] protected SoundyPlayer startSfx;
        [SerializeField] protected SoundyPlayer rankStepSfx;
        [SerializeField] protected SoundyPlayer endSfx;
        [SerializeField] protected SoundyPlayer scrollEndSfx;

        [Header("Vfx")]
        [SerializeField] protected ParticleSystem scaleInVfx;
        [SerializeField] protected ParticleSystem scaleOutVfx;

        [Header("Test Mode")]
        [SerializeField][LabelOverride("Enabled")] bool testModeEnabled = false;
        [SerializeField] int fakeRank = 1;
        [SerializeField] int offset = 20;

        public LeaderboardScore MyScoreData { get; private set; }

        public bool HaveRankUp { get; private set; }

        public event Action OnStart = delegate { };
        public event Action OnRankStep = delegate { };
        public event Action OnEnded = delegate { };
        public event Action OnReadyDone = delegate { };

        Action RankUpAnimationCallback;

        const int kMaxScores = 200;
        bool enableLog = true;
        int padTop = 3;
        int minFakeRow = 20;

        protected virtual void Reset()
        {
            if (!rankUpOSA)
            {
                rankUpOSA = GetComponentInChildren<UILeaderboardRankUpOSA>();
            }

            if (!myScore)
            {
                myScore = GetComponentInChildren<UIMyLeaderboardRecord>();
            }
        }

        protected virtual void OnValidate()
        {
            if (rankUpOSA)
            {
                rankUpOSA.leaderboardId = leaderboardId;
                rankUpOSA.timeSpan = timeSpan;
            }
        }

        protected virtual void Awake()
        {
            rankUpOSA.gameObject.SetActive(false);
        }

        protected virtual void Start()
        {
            rankUpOSA.Parameters.SetDragEnable(false);
            rankUpOSA.OnReady += OnScrollReady;
            rankUpOSA.gameObject.SetActive(true);
        }

        protected virtual void OnDestroy()
        {
            rankUpOSA.OnReady -= OnScrollReady;
        }

        protected virtual void OnEnable()
        {
            rankUpOSA.Parameters.SetScrollEnable(true);
            myScore.gameObject.SetActive(false);
            myScore.RectTransform.anchoredPosition = Vector2.zero;
        }

        int Invert(int maxVisibleRow, int index)
        {
            return maxVisibleRow - index - 1;
        }

        //List<LeaderboardScore> scores;
        //int lastRank;
        //int myScoreIndex;
        //int myLastScoreIndex;
        async void OnScrollReady()
        {
            HaveRankUp = false;
            rankUpOSA.ResetItems(0);

            int lastRank = Leaderboard.GetLastRank(leaderboardId, timeSpan);
            if (lastRank == -1)
            {
                LeaderboardData leaderboard = await Leaderboard.GetLeaderboardAsync(leaderboardId, timeSpan);
                lastRank = leaderboard.MaxRange;
            }
            MyScoreData = await Leaderboard.LoadMyScore(leaderboardId, timeSpan);
            if (!testModeEnabled && (MyScoreData == null || MyScoreData.Rank >= lastRank))
            {
                gameObject.SetActive(false);
                OnReadyDone();
                return;
            }

            if (testModeEnabled)
            {
                MyScoreData.UpdateRank(fakeRank);
                lastRank = MyScoreData.Rank + offset;
            }

            if (enableLog)
                Debug.Log($"MyScoreData.Rank: {MyScoreData.Rank} - lastRank: {lastRank}");

            int fromIndex = Math.Max(MyScoreData.Rank - padTop, 0);
            int limit = Mathf.Clamp(lastRank - MyScoreData.Rank + padTop, minFakeRow, kMaxScores);
            List<LeaderboardScore> scores = await Leaderboard.LoadScores(leaderboardId, timeSpan, fromIndex, limit);
            if (scores.Count == 0)
            {
                gameObject.SetActive(false);
                OnReadyDone();
                return;
            }

            int diff = scores.Count - padTop;
            lastRank = MyScoreData.Rank + diff;

            if (enableLog)
                Debug.Log($"diff: {diff} - lastRank: {lastRank}");

            int visibleRow = Mathf.Min(scores.Count, maxVisibleRow);
            float contentSpace = rankUpOSA.Parameters.ContentSpacing;
            float rowSize = rankUpOSA.Parameters.DefaultItemSize;
            Vector2 sizeDelta = rankUpOSA.RectTransform.sizeDelta;
            sizeDelta = new Vector2(sizeDelta.x, (rowSize + contentSpace) * visibleRow - contentSpace);
            rankUpOSA.RectTransform.sizeDelta = sizeDelta;

            LeaderboardScore _myScoreData = MyScoreData.Clone();
            _myScoreData.UpdateRank(lastRank);
            myScore.SetData(_myScoreData);

            int myScoreIndex = scores.IndexOf((l) => l.Rank == MyScoreData.Rank);
            int myLastScoreIndex = scores.IndexOf((l) => l.Rank == lastRank);
            rankUpOSA.HideIndex2 = myLastScoreIndex > -1 ? myLastScoreIndex : scores.Count - 1;

            if (enableLog)
                Debug.Log($"myScoreIndex: {myScoreIndex} - myLastScoreIndex: {myLastScoreIndex}");

            HaveRankUp = myLastScoreIndex != -1;
            OnReadyDone();

            RankUpAnimationCallback = () =>
            {
                myScore.gameObject.SetActive(true);

                if (myLastScoreIndex < maxVisibleRow)
                {
                    // PLAY MOVE ANIMATION

                    scores.RemoveAt(myScoreIndex);
                    scores.Insert(myLastScoreIndex, _myScoreData);
                    for (int i = myScoreIndex; i < myLastScoreIndex; i++)
                    {
                        scores[i].UpdateRank(MyScoreData.Rank + i - myScoreIndex);
                    }

                    rankUpOSA.HideIndex1 = -1;
                    rankUpOSA.Parameters.SetScrollEnable(false);
                    rankUpOSA.SetScores(scores, scores.Count);
                    rankUpOSA.ScrollTo(myLastScoreIndex);

                    int maxVisibleItemCount = Math.Min(rankUpOSA.VisibleItemsCount, scores.Count);
                    myScore.RectTransform.anchoredPosition = Invert(maxVisibleItemCount, myLastScoreIndex) * (rowSize + contentSpace) * Vector2.up;

                    static TweenCallback DoChange(UILeaderboardRecord ui, LeaderboardScore score)
                    {
                        return () =>
                        {
                            score.UpdateRank(score.Rank + 1);
                            ui.SetData(score);
                        };
                    }

                    float halfDuration = moveDuration * 0.5f;
                    DOVirtual.DelayedCall(startDelay, () =>
                    {
                        for (int i = 0; i < maxVisibleItemCount; i++)
                        {
                            if (i < myLastScoreIndex && i >= myScoreIndex)
                            {
                                var vh = rankUpOSA.GetItemViewsHolder(i);
                                var root = vh.root;
                                var anchorPosY = -(i + 1) * (rowSize + contentSpace) - (root.pivot.y) * rowSize;
                                root.DOAnchorPosY(anchorPosY, moveDuration);
                                DOVirtual.DelayedCall(halfDuration, DoChange(vh.UI, scores[i].Clone()));
                            }
                        }

                        var myAnchorPosY = Invert(maxVisibleItemCount, myScoreIndex) * (rowSize + contentSpace);
                        myScore.RectTransform.DOAnchorPosY(myAnchorPosY, moveDuration);
                        myScoreAnimation.DORestartAllById(animationStartKey);
                        DOVirtual.DelayedCall(halfDuration, () =>
                        {
                            NotifyOnRankStep();
                            _myScoreData.UpdateRank(MyScoreData.Rank);
                            myScore.SetData(_myScoreData);
                        });
                        DOVirtual.DelayedCall(moveDuration, () =>
                        {
                            myScoreAnimation.DORestartAllById(animationEndKey);
                        });
                    });
                }
                else
                {
                    // PLAY SCROLL ANIMATION

                    rankUpOSA.HideIndex1 = myScoreIndex;
                    rankUpOSA.Parameters.SetScrollEnable(true);
                    rankUpOSA.SetScores(scores, scores.Count);
                    rankUpOSA.ScrollTo(myLastScoreIndex, 1f, 1f);

                    DOVirtual.DelayedCall(startDelay, () =>
                    {
                        NotifyOnStart();
                        rankUpOSA.SmoothScrollTo(0, scrollDuration, 0.5f, 0.5f,
                            (dt) =>
                            {
                                int newRank = MyScoreData.Rank + Mathf.RoundToInt(diff * (1 - dt));
                                if (newRank != _myScoreData.Rank)
                                    NotifyOnRankStep();

                                _myScoreData.UpdateRank(newRank);
                                myScore.SetData(_myScoreData);
                                if (dt >= 1) { EndScroll(); return false; }
                                else return true;
                            }, null, true);

                        var myAnchorPosY = (maxVisibleRow - 1 - myScoreIndex) * (rowSize + contentSpace);
                        myScore.RectTransform.DOAnchorPosY(myAnchorPosY, moveDuration);

                        myScoreAnimation.DORestartAllById(animationStartKey);
                    });
                }
            };

            if (!manualPlay)
            {
                RankUpAnimationCallback();
                RankUpAnimationCallback = null;
            }
        }

        void EndScroll()
        {
            if (scrollEndSfx) scrollEndSfx.Play();
            myScoreAnimation.DORestartAllById(animationEndKey);
        }

        public void PlayRankUpAnimation()
        {
            if (manualPlay && RankUpAnimationCallback != null)
            {
                RankUpAnimationCallback();
                RankUpAnimationCallback = null;
            }
        }

        public void NotifyOnRankStep()
        {
            if (rankStepSfx) rankStepSfx.Play();
            OnRankStep();
        }

        public void NotifyOnStart()
        {
            if (startSfx) startSfx.Play();
            OnStart();
        }

        public void NotifyOnEnded()
        {
            if (endSfx) endSfx.Play();
            OnEnded();
        }

        public void NotifyOnScaleIn()
        {
            if (scaleInVfx) scaleInVfx.Play();
        }

        public void NotifyOnScaleOut()
        {
            if (scaleOutVfx) scaleOutVfx.Play();
        }
    }
}