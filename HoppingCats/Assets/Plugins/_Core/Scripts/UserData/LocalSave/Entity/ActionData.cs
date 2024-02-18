namespace moonNest
{
    public struct ActionData
    {
        public int id;
        public int arg1;
        public int arg2;
        public int arg3;

        public ActionData(int actionId, int arg1 = -1, int arg2 = -1, int arg3 = -1)
        {
            this.id = actionId;
            this.arg1 = arg1;
            this.arg2 = arg2;
            this.arg3 = arg3;
        }
    }
}