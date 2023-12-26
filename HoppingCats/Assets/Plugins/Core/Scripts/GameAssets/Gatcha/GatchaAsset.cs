using System.Collections.Generic;

namespace moonNest
{
    public class GatchaAsset : SingletonScriptObject<GatchaAsset>
    {
        public int spinDuration = 4;
        public List<SpinDetail> spins = new List<SpinDetail>();
        public int luckyBoxDuration = 4;
        public List<LuckyBoxesDetail> luckyBoxes = new List<LuckyBoxesDetail>();

        public SpinDetail FindSpin(int spinId) => spins.Find(_ => _.id == spinId);
        public LuckyBoxesDetail FindNineBoxes(int boxesId) => luckyBoxes.Find(_ => _.id == boxesId);
    }

    public enum GatchaDrawMethod { Table, Reward }
}