using UnityEngine;

namespace moonNest
{
    public class CollectStatHandler : CollectHandler
    {
        public UIUserStat uiUserStat;

        [Header("Removed in future")]
        public UserStatId statId = -1;

        int currentValue, lastValue, diff;

        void OnValidate()
        {
            if (type != CollectType.Stat) type = CollectType.Stat;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            statId = uiUserStat && uiUserStat.stat != -1 ? uiUserStat.stat : statId;
            if (statId != -1)
            {
                lastValue = currentValue = UserData.Stat(statId);
                UserData.Ins.Subscribe(statId.ToString(), OnStatChanged);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (statId != -1)
            {
                UserData.Ins.Unsubscribe(statId.ToString(), OnStatChanged);
            }
        }

        void OnStatChanged(BaseUserData obj)
        {
            int newValue = UserData.Stat(statId);
            diff = newValue - currentValue;
            lastValue = currentValue;
            currentValue = newValue;
            if (diff <= 0) return;

            // delay call because should call after proccessing reward consuming
            if (animConfig.isActiveAndEnabled && CollectingManager.Ins.FindStat(statId, false) == null)
            {
                if (uiUserStat)
                    uiUserStat.UpdateText(lastValue.ToString());

                executer.DelayExecuteByFrame(1, () =>
                {
                    CollectingManager.Ins.AddRequest(new CollectStat(statId, diff));
                });
            }
        }

        protected override void PlayAnim()
        {
            var request = CollectingManager.Ins.FindStat(statId, true);
            if (request != null)
            {
                DoPlay(request);
            }
        }

        protected override void OnAnimDone(int count, float percent)
        {
            base.OnAnimDone(count, percent);

            if (uiUserStat)
                uiUserStat.UpdateText((lastValue + diff * percent).ToString());
        }
    }
}