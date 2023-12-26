using UnityEditor;
using UnityEngine;
using moonNest;

internal class ProfileConfigTab : TabContent
{
    readonly ProfileListDrawer profileListDrawer;

    public ProfileConfigTab()
    {
        profileListDrawer = new ProfileListDrawer();
    }

    public override void DoDraw()
    {
        Undo.RecordObject(Profiles.Ins, "Profiles");
        profileListDrawer.DoDraw(Profiles.Ins.profiles);
        if (GUI.changed) Draw.SetDirty(Profiles.Ins);
    }

    public override bool DoDrawWindow() => profileListDrawer.DoDrawWindow();

    internal class ProfileListDrawer : ListTabDrawer<ProfileConfig>
    {
        protected override string GetTabLabel(ProfileConfig element) => element.name;

        protected override ProfileConfig CreateNewElement() => new ProfileConfig("New Profile");

        protected override void DoDrawContent(ProfileConfig data)
        {
            data.name = Draw.TextField("Name", data.name, 300);

            Draw.BeginHorizontal();

            Draw.BeginVertical();
            DrawNetworking(data);
            Draw.EndVertical();

            Draw.Space(40);
            Draw.BeginVertical();
            DrawCheat(data);
            DrawLog(data);
            Draw.EndVertical();

            Draw.FlexibleSpace();

            Draw.EndHorizontal();
        }

        private void DrawLog(ProfileConfig profile)
        {
            if (profile != null)
            {
                Draw.SpaceAndLabelBoldBox("Logs", Color.yellow);
                profile.logRestApi = Draw.ToggleField("Log RestAPI", profile.logRestApi, 50);
                profile.verboseLog = Draw.ToggleField("Verbose Log", profile.verboseLog, 100);
                profile.debugLog = Draw.ToggleField("Debug Log", profile.debugLog, 100);
            }
        }

        private void DrawCheat(ProfileConfig profile)
        {
            Draw.SpaceAndLabelBoldBox("Cheat", Color.yellow);
            profile.cheatNewDay = Draw.ToggleField("Cheat New Day", profile.cheatNewDay, 100);
            profile.cheatEnabled = Draw.ToggleField("Enable Cheat", profile.cheatEnabled, 100);
        }

        private void DrawNetworking(ProfileConfig profile)
        {
            Draw.SpaceAndLabelBoldBox("Networking", Color.yellow);
            profile.environment = Draw.EnumField("Environment", profile.environment, 100);
            profile.usePhoton = Draw.ToggleField("Use Photon", profile.usePhoton, 100);

            Draw.SpaceAndLabelBoldBox("Remote Services", Color.yellow);
            profile.storeRemoteData = Draw.ToggleField("Store Remote Data", profile.storeRemoteData, 40);
            profile.syncDate = Draw.ToggleField("Sync Date", profile.syncDate, 40);
            profile.syncArena = Draw.ToggleField("Sync Arena", profile.syncArena, 40);
            if (profile.storeRemoteData)
                profile.fakeUserId = Draw.TextField("Fake Id", profile.fakeUserId, 120);


            Draw.SpaceAndLabelBoldBox("Custom Services", Color.yellow);
            profile.leaderboardUrl = Draw.TextField("Leaderboard Url", profile.leaderboardUrl, 250);
        }
    }
}