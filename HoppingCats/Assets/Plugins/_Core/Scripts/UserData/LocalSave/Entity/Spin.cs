using System;
using System.Collections.Generic;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class Spin
    {
        [SerializeField] private int id;
        [SerializeField] private bool freeSpin = false;
        [SerializeField] private int point = 0;
        [SerializeField] private List<int> claimedPoints = new List<int>();

        public PointUpdateEvent onPointUpdated;

        public int Id => id;
        public bool FreeSpin { get { return freeSpin; } set { freeSpin = value; } }
        public int Point => point;

        private SpinDetail _detail;
        public SpinDetail Detail { get { if(!_detail) _detail = GatchaAsset.Ins.FindSpin(id); return _detail; } }

        public Spin(SpinDetail spinDetail)
        {
            id = spinDetail.id;
        }

        public bool CanClaim(int point) => !claimedPoints.Contains(point);

        public void DoClaim(int point)
        {
            if(!claimedPoints.Contains(point))
            {
                claimedPoints.Add(point);
                UserGatcha.Ins.dirty = true;
                UserGatcha.Ins.onPointRewardClaimed?.Invoke(this, point);
            }
        }

        public void AddPoint(int value)
        {
            int lastPoint = point;
            point = Math.Max(0, point + value);
            UserGatcha.Ins.dirty = true;
            UserGatcha.Ins.Notify("Spin");
            onPointUpdated?.Invoke(lastPoint, point);
        }

        /// <summary>
        /// Only for override purpose
        /// </summary>
        /// <param name="value"></param>
        public void SetPointClaimeds(int point, List<int> claimedPoints)
        {
            this.point = point;
            this.claimedPoints = claimedPoints;
        }
    }
}