namespace moonNest
{
    public class UpdateUserLogin : BootStep
    {
        public override string ToString() => "Update User Login";

        public async override void OnStep()
        {
            CoreHandler.Ins.Init();
            await CoreHandler.Ins.DoLogin();
            Complete();
        }
    }
}