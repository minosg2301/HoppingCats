using System;
using UnityEngine;

namespace moonNest
{
    public class UserGatcha : BaseUserData
    {
        public static UserGatcha Ins => LocalData.Get<UserGatcha>();

        [SerializeField] private int type;
        [SerializeField] private Spin spin;
        [SerializeField] private LuckyBoxes luckyBoxes;
        [SerializeField] private DateTime endTime;
        [SerializeField] private DateTime luckyBoxTime;

        public Action<Spin, int> onPointRewardClaimed;

        public LuckyBoxes LuckyBoxes => luckyBoxes;
        public Spin Spin => spin;
        public float LastSeconds => (float)endTime.Subtract(DateTime.Now).TotalSeconds;

        protected internal override void OnInit()
        {
            base.OnInit();
            endTime = DateTime.Today;
            type = 0;
        }

        protected internal override void OnLoad()
        {
            base.OnLoad();

            if(luckyBoxTime == null)
            {
                luckyBoxTime = DateTime.Now;
            }
        }

        public void UpdateNewDayLogin()
        {
            GatchaAsset instance = GatchaAsset.Ins;
            if (endTime <= DateTime.Now)
            {
                type = (type + 1) % 2;
                spin = null;
                luckyBoxes = null;

                if (instance.spins.Count > 0)
                {
                    spin = new Spin(instance.spins.Count == 1 ? instance.spins[0] : instance.spins.Random(spin?.Detail));
                    endTime = DateTime.Today.AddDays(instance.spinDuration);
                }
            }
            else if (spin != null)
            {
                spin.FreeSpin = spin.Detail.freePerDay;
            }

            if (luckyBoxTime <= DateTime.Now)
            {
                type = (type + 1) % 2;
                spin = null;
                luckyBoxes = null;
                if (instance.luckyBoxes.Count > 0)
                {
                    luckyBoxes = new LuckyBoxes(instance.luckyBoxes.Count == 1 ? instance.luckyBoxes[0] : instance.luckyBoxes.Random(luckyBoxes?.Detail));
                    endTime = DateTime.Today.AddDays(instance.luckyBoxDuration);
                }
            }

            dirty = true;
        }
    }
}