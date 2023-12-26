using DG.Tweening;
using Doozy.Engine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace moonNest
{
    public class UISpin : MonoBehaviour, IObserver
    {
        [Header("References")]
        public Transform cursor;
        public Transform rotator;
        public UISpinPointProgress progress;
        public UICountDownTime countDownTime;
        public UIPrice price;
        public UIButton spinButton;

        [Header("Multi Spin")]
        public float delayPerSpin = 3;
        public TextMeshProUGUI multiSpinText;
        public UIPrice multiSpinPrice;
        public UIButton multiSpinButton;

        [Header("Settings")]
        public float round = 10;
        public float rotateDuration = 5;
        public Ease rotationEase = Ease.InOutQuint;
        public float anglePadding = 15;

        [Header("Sound Fx")]
        public AudioSource spinSfx;
        public AudioSource bigWinSfx;

        [Header("Events")]
        public UnityEvent onSpinStart = new UnityEvent();
        public SpinEvent onSpinStop = new SpinEvent();
        public UnityEvent onMultiSpinStart = new UnityEvent();
        public UnityEvent onMultiSpinStop = new UnityEvent();

        protected UISpinItem[] spinItems;
        protected UISpinItem selectedItem;
        protected Spin spin;
        protected SpinDetail spinDetail;
        protected Tweener dt;
        protected bool spining = false;
        protected bool initialized = false;

        /// <summary>
        /// Keep tracks of spin remaining count
        /// </summary>
        private int multiSpinCount = 0;

        #region Unity Methods
        void Reset()
        {
            gameObject.name = "UISpin";
            if(!rotator) rotator = transform;
        }

        protected void Start()
        {
            if(spinButton) spinButton.OnClick.OnTrigger.Event.AddListener(Spin);
            if(multiSpinButton) multiSpinButton.OnClick.OnTrigger.Event.AddListener(MultiSpin);
            if(countDownTime) countDownTime.StartWithDuration(UserGatcha.Ins.LastSeconds);

            UpdateSpinDetail();
            initialized = true;
        }

        protected void OnEnable()
        {
            if(initialized) UpdateSpinDetail();
        }
        #endregion

        #region public methods
        /// <summary>
        /// Call back when UserGatcha updated
        /// </summary>
        /// <param name="data"></param>
        /// <param name="scopes"></param>
        public void OnNotify(IObservable data, string[] scopes) => UpdateUI();

        /// <summary>
        /// Action callback when user click on spin button
        /// </summary>
        void Spin()
        {
            if(price.UserCanPay)
            {
                if(!spin.FreeSpin) price.Currency.AddValue(-spinDetail.cost.value);
                else spin.FreeSpin = false;

                UserGatcha.Ins.dirty = true;

                DoSpin(spinItem => HandleSpinDone(spinItem));
            }
        }

        /// <summary>
        /// Action callback when user click on multi spin button
        /// </summary>
        void MultiSpin()
        {
            if(multiSpinPrice.UserCanPay)
            {
                multiSpinPrice.Currency.AddValue(-(spinDetail.cost * spinDetail.multiSpin * spinDetail.multiSpinDiscount).value);

                UserGatcha.Ins.dirty = true;
                multiSpinCount = spinDetail.multiSpin;
                onMultiSpinStart.Invoke();
                DoSpin(spinItem => HandleMultiSpinDone(spinItem));
            }
        }
        #endregion

        #region private methods
        /// <summary>
        /// Get Spin in User Gatcha and update ui components if available
        /// </summary>
        private void UpdateSpinDetail()
        {
            if(spin != UserGatcha.Ins.Spin && UserGatcha.Ins.Spin != null)
            {
                spin = UserGatcha.Ins.Spin;
                spinDetail = spin.Detail;
                spinItems = GetComponentsInChildren<UISpinItem>();
                spinItems.ForEach((spinItem, i) => spinItem.SetData(spinDetail.spinItems[i]));
                if(progress) progress.SetSpin(spin);
                UserGatcha.Ins.Subscribe(this, "spin");
            }
        }

        /// <summary>
        /// Update ui components
        /// </summary>
        private void UpdateUI()
        {
            // prevent update ui while spining
            if(spining) return;

            // spin
            if(price) price.SetPrice(spinDetail.cost);
            if(spin.FreeSpin) price.SetPriceValue(0);
            if(spin.Detail.pointEnabled && progress) progress.SetValue(spin.Point);
            if(spinButton) spinButton.Interactable = price.UserCanPay;

            // mutli spin
            if(multiSpinText) multiSpinText.text = "x" + spinDetail.multiSpin;
            if(multiSpinPrice) multiSpinPrice.SetPrice(spinDetail.cost * spinDetail.multiSpin * spinDetail.multiSpinDiscount);
            if(multiSpinButton)
            {
                multiSpinButton.Interactable = multiSpinPrice.UserCanPay;
                multiSpinButton.gameObject.SetActive(!spin.FreeSpin && multiSpinCount == 0);
            }
        }

        /// <summary>
        /// Callback method when a spin completed in multi spin mode
        /// </summary>
        /// <param name="selectedItem"></param>
        private void HandleMultiSpinDone(UISpinItem selectedItem)
        {
            // update remaining spin count
            multiSpinCount--;

            RewardConsumer.ConsumeReward(selectedItem.Config.reward);
            UpdateUI();
            onSpinStop.Invoke(selectedItem);

            // if multi spin count available, do spin after delay
            if(multiSpinCount > 0)
            {
                DOVirtual.DelayedCall(delayPerSpin, () =>
                {
                    DoSpin(spinItem => HandleMultiSpinDone(spinItem));
                });
            }
            else
            {
                onMultiSpinStop.Invoke();
                spinButton.gameObject.SetActive(true);
                multiSpinButton.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Callback method when a spin completed in single spin mode
        /// </summary>
        /// <param name="selectedItem"></param>
        private void HandleSpinDone(UISpinItem selectedItem)
        {
            RewardConsumer.ConsumeReward(selectedItem.Config.reward);
            spinButton.gameObject.SetActive(true);
            multiSpinButton.gameObject.SetActive(true);
            UpdateUI();
            onSpinStop.Invoke(selectedItem);
        }

        /// <summary>
        /// Random a spin item base on probability
        /// </summary>
        /// <returns></returns>
        private UISpinItem CalculateSelectedItem()
        {
            List<UISpinItem> sortedList = spinItems.ToList();
            sortedList.SortAsc(e => e.Config.weight);

            int sum = sortedList.Sum(_ => _.Config.weight);
            float ran = Random.Range(0f, sum);
            float k = 0;
            foreach(UISpinItem item in sortedList)
            {
                k += item.Config.weight;
                if(ran < k) return item;
            }
            return sortedList[0];
        }

        /// <summary>
        /// Perform a spin
        /// </summary>
        /// <param name="onCompleted"></param>
        private void DoSpin(Action<UISpinItem> onCompleted)
        {
            if(rotator)
            {
                if(spinSfx) spinSfx.Play();

                spining = true;
                selectedItem = CalculateSelectedItem();
                spinButton.gameObject.SetActive(false);
                multiSpinButton.gameObject.SetActive(false);

                onSpinStart.Invoke();

                Vector3 eulerAngles = transform.eulerAngles;
                float angle = selectedItem.transform.eulerAngles.z + Random.Range(-anglePadding, anglePadding);
                eulerAngles.z = -(round * 360 + angle - transform.eulerAngles.z);
                dt = rotator
                    .DOLocalRotate(eulerAngles, rotateDuration, RotateMode.FastBeyond360)
                    .SetEase(rotationEase)
                    .OnComplete(() =>
                    {
                        spining = false;
                        if(spinSfx) spinSfx.Stop();
                        if(selectedItem.Config.isBigReward && bigWinSfx) bigWinSfx.Play();
                        if(spin.Detail.pointEnabled) spin.AddPoint(1);

                        onCompleted(selectedItem);
                    });
            }
            else
            {
                Debug.LogError("Rotator can not be null!!!");
            }
        }

        #endregion
    }

    [Serializable]
    public class SpinEvent : UnityEvent<UISpinItem> { }
}