using System;

namespace moonNest
{
    [Serializable]
    public class LuckyBoxesDetail : BaseData
    {
        public PriceConfig cost = new PriceConfig();
        public LuckyBoxConfig[] boxes = new LuckyBoxConfig[9];

        public GatchaDrawMethod drawMethod;

        public LuckyBoxesDetail(string name) : base(name)
        {
            for(int i = 0; i < 9; i++)
            {
                boxes[i] = new LuckyBoxConfig("Box " + (i + 1));
            }
        }
    }

    [Serializable]
    public class LuckyBoxConfig : BaseGatchaItemDetail
    {
        public LuckyBoxConfig(string name) : base(name) { }
    }
}