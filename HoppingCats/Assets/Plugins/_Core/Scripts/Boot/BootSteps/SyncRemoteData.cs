using DG.Tweening;
using moonNest.remotedata;

namespace moonNest
{
    public class SyncRemoteData : BootStep
    {
        public override string ToString() => "Sync Remote Data";

        public override void OnStep()
        {
            if(!GlobalConfig.Ins.StoreRemoteData)
            {
                Complete();
                return;
            }
            UserCurrency.Ins.CreateRemoteUpdater();
            UserData.Ins.CreateRemoteUpdater();
            UserProgress.Ins.CreateRemoteUpdater();
            UserInventory.Ins.CreateRemoteUpdater();
            UserQuest.Ins.CreateRemoteUpdater();
            UserShop.Ins.CreateRemoteUpdater();
            UserOnlineReward.Ins.CreateRemoteUpdater();
            UserArena.Ins.CreateRemoteUpdater();
            UserStore.Ins.CreateRemoteUpdater();
            UserTutorial.Ins.CreateRemoteUpdater();

            DOVirtual.DelayedCall(0.2f, DoSyncRemoteData);
        }

        async void DoSyncRemoteData()
        {
            await RemoteDataManager.GetOrCreate<FirestoreUserData>("Users", UserData.UserId);

            Complete();
        }
    }
}