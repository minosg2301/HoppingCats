using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace moonNest.editor
{
    internal class GlobalConfigTab : TabContent
    {
        readonly TabContainer tabContainer;
        static GlobalConfig globalConfig;

        public GlobalConfigTab()
        {
            globalConfig = GlobalConfig.Ins;
            if (Profiles.Ins.profiles.Count == 0)
            {
                Profiles.Ins.profiles = new List<ProfileConfig> { new ProfileConfig("Default") };
                globalConfig.selectedProfileId = Profiles.Ins.profiles[0].id;
            }

            tabContainer = new TabContainer();
            tabContainer.AddTab("General", new GeneralTab());
            tabContainer.AddTab("Profiles", new ProfileConfigTab());
            tabContainer.AddTab("Language", new LanguageTab());
            tabContainer.AddTab("Sound", new SoundTab());
            tabContainer.AddTab("Import/Export", new ImportExportTab());
            tabContainer.AddTab("Game ID", new GameIdTab());
            tabContainer.AddTab("Build", new BuildTab());
        }

        public override void DoDraw()
        {
            Undo.RecordObject(GlobalConfig.Ins, "Global Config");
            tabContainer.DoDraw();
            if (GUI.changed) Draw.SetDirty(GlobalConfig.Ins);
        }

        public override bool DoDrawWindow()
        {
            return tabContainer.DoDrawWindow();
        }

        private class GeneralTab : TabContent
        {
            public override void DoDraw()
            {
                Draw.BeginHorizontal();

                Draw.BeginVertical();
                DrawLeft();
                Draw.EndVertical();

                Draw.Space(40);
                Draw.BeginVertical();
                DrawMid();
                Draw.EndVertical();

                Draw.Space(40);
                Draw.BeginVertical();
                //DrawRight();
                Draw.EndVertical();

                Draw.FlexibleSpace();

                Draw.EndHorizontal();
            }

            private void DrawLeft()
            {
                Draw.BeginChangeCheck();
                Draw.LabelBoldBox("Selected Profile", Color.yellow);
                globalConfig.selectedProfileId = Draw.IntPopupField("Profile", globalConfig.selectedProfileId, Profiles.Ins.profiles, "name", "id", 250);
                if (Draw.EndChangeCheck())
                {
                    var profile = Profiles.Ins.profiles.Find(p => p.id == globalConfig.selectedProfileId);
                    if (profile)
                    {
                        if (profile.cheatEnabled) DrawUtilities.AddSymbolsOnAllPlatform("ENABLE_CHEAT");
                        else DrawUtilities.RemoveSymbolsOnAllPlatform("ENABLE_CHEAT");
                    }
                }

                Draw.SpaceAndLabelBoldBox("Custom Services", Color.yellow);
                globalConfig.DRMServer = Draw.TextField("DRM Server", globalConfig.DRMServer, 250);
                globalConfig.trackingServer = Draw.TextField("Tracking Server", globalConfig.trackingServer, 250);

                Draw.SpaceAndLabelBoldBox("Resolution Scaling", Color.yellow);
                globalConfig.resolutionScaling = Draw.ToggleField("Enabled", globalConfig.resolutionScaling, 60);
                if (globalConfig.resolutionScaling)
                    globalConfig.targetDPI = Draw.IntField("Taget DPI", globalConfig.targetDPI, 250);

                Draw.SpaceAndLabelBoldBox("IAP", Color.yellow);
                globalConfig.appStorePromotionPurchase = Draw.ToggleField("AppStore Promotion", globalConfig.appStorePromotionPurchase, 100);
                globalConfig.verifyPurchasingOnline = Draw.ToggleField("Verify Online", globalConfig.verifyPurchasingOnline, 100);
                globalConfig.fakeStoreMode = Draw.EnumField("Fake Mode", globalConfig.fakeStoreMode, 200);

                Draw.SpaceAndLabelBoldBox("Ads", Color.yellow);
                globalConfig.adsEnabled = Draw.ToggleField("Enable Ads", globalConfig.adsEnabled, 100);
            }

            private static void DrawMid()
            {
                Draw.LabelBoldBox("Platform", Color.yellow, 670);
                Draw.BeginHorizontal();
                Draw.BeginVertical();
                Draw.LabelBoldBox("iOS", Color.yellow, 270);
                globalConfig.iosAppId = Draw.TextField("iOS App Id", globalConfig.iosAppId, 130);

                Draw.SpaceAndLabelBold("Capabilities");
                globalConfig.useGameCenter = Draw.ToggleField("Game Center", globalConfig.useGameCenter, 60);
                globalConfig.useSignInWithApple = Draw.ToggleField("Sign In With Apple", globalConfig.useSignInWithApple, 60);
                Draw.EndVertical();

                Draw.Space(40);
                Draw.BeginVertical();
                Draw.LabelBoldBox("Android", Color.yellow);
                globalConfig.googlePlayLicense = Draw.TextField("License Key", globalConfig.googlePlayLicense);
                globalConfig.googleWebClientId = Draw.TextField("Web Client Id", globalConfig.googleWebClientId);

                Draw.SpaceAndLabelBold("Auto Update");
                globalConfig.autoUpdate = Draw.ToggleField("Enabled", globalConfig.autoUpdate);
                globalConfig.updateType = Draw.EnumField("Update Type", globalConfig.updateType);
                globalConfig.stalenessDays = Draw.IntField("Staleness Days", globalConfig.stalenessDays);
                globalConfig.simulateAutoUpdate = Draw.ToggleField("Simulate", globalConfig.simulateAutoUpdate);
                Draw.EndVertical();
                Draw.EndHorizontal();
            }
        }

        private class LanguageTab : TabContent
        {
            private TableDrawer<Language> table;

            public LanguageTab()
            {
                table = new TableDrawer<Language>();
                table.AddCol("Code", 120, ele => ele.code = Draw.Text(ele.code, 120));
                table.AddCol("Text", 240, ele => ele.text = Draw.Text(ele.text, 240));
                table.elementCreator = () => new Language();
            }

            public override void DoDraw()
            {
                if (globalConfig.languages.Count == 0)
                {
                    globalConfig.languages.Add(new Language("English", "TXT_ENGLISH"));
                    globalConfig.languages.Add(new Language("Vietnamese", "TXT_VIETNAMESE"));
                    globalConfig.languages.Add(new Language("French", "TXT_FRENCH"));
                    globalConfig.languages.Add(new Language("German", "TXT_GERMAN"));
                    globalConfig.languages.Add(new Language("Spanish", "TXT_SPANISH"));
                    globalConfig.languages.Add(new Language("Portugese", "TXT_PORTUGUESE"));
                    globalConfig.languages.Add(new Language("Russian", "TXT_RUSSIAN"));
                    globalConfig.languages.Add(new Language("Chinese Simplified", "TXT_CHINESE_SIMPLIFIED"));
                    globalConfig.languages.Add(new Language("Chinese Traditional", "TXT_CHINESE_TRADITIONAL"));
                    globalConfig.languages.Add(new Language("Japanese", "TXT_JAPANESE"));
                    globalConfig.languages.Add(new Language("Korean", "TXT_KOREAN"));
                    globalConfig.languages.Add(new Language("Arabic", "TXT_ARABIC"));
                    globalConfig.languages.Add(new Language("Hindi", "TXT_HINDI"));
                    globalConfig.languages.Add(new Language("Philipine", "TXT_PHILIPPINE"));
                    globalConfig.languages.Add(new Language("Indo", "TXT_INDONESIA"));
                    globalConfig.languages.Add(new Language("Thai", "TXT_THAI"));
                    globalConfig.languages.Add(new Language("Italian", "TXT_ITALIAN"));
                    globalConfig.languages.Add(new Language("Turkis", "TXT_TURKISH"));
                }
                table.DoDraw(globalConfig.languages);
            }
        }

        private class SoundTab : TabContent
        {
            public override void DoDraw()
            {
                globalConfig.mainMixer = Draw.ObjectField("Main Mixer", globalConfig.mainMixer, 120);
                globalConfig.musicParam = Draw.TextField("Music Param", globalConfig.musicParam, 120);
                globalConfig.sfxParam = Draw.TextField("Sfx Param", globalConfig.sfxParam, 120);
                globalConfig.ingameSfxParam = Draw.TextField("Ingame-Sfx Param", globalConfig.ingameSfxParam, 120);
            }
        }
    }

    internal class BuildTab : TabContent
    {
        public override void DoDraw()
        {
            Undo.RecordObject(EditorConfigAsset.Ins, "Global Config");
            EditorConfigAsset.Ins.neverAskUploadSymbol = !Draw.ToggleField("Ask Upload Symbol", !EditorConfigAsset.Ins.neverAskUploadSymbol, 100);
            if (GUI.changed) Draw.SetDirty(EditorConfigAsset.Ins);
        }
    }

    internal class GameIdTab : TabContent
    {
        public override async void DoDraw()
        {
            var globalConfig = GlobalConfig.Ins;
            Draw.BeginDisabledGroup(true);
            globalConfig.gameId = Draw.TextField("Game Id", globalConfig.gameId, 125);
            Draw.EndDisabledGroup();

            Draw.Space();
            if (globalConfig.gameId == "TEMPLATE")
            {
                if (Draw.Button("Create Game ID", Color.blue, 200))
                {
                    globalConfig.gameId = StringExt.RandomAlphabetWithNumber(6);
                    Draw.SetDirty(globalConfig);
                }
            }
            else
            {
                Draw.LabelBoldBox("Selected Profile", Color.yellow, 200);
                globalConfig.selectedProfileId = Draw.IntPopup(globalConfig.selectedProfileId, Profiles.Ins.profiles, "name", "id", 250);
                if (Draw.EndChangeCheck())
                {
                    var profile = Profiles.Ins.profiles.Find(p => p.id == globalConfig.selectedProfileId);
                    if (profile)
                    {
                        if (profile.cheatEnabled) DrawUtilities.AddSymbolsOnAllPlatform("ENABLE_CHEAT");
                        else DrawUtilities.RemoveSymbolsOnAllPlatform("ENABLE_CHEAT");
                    }
                    globalConfig.SelectedProfile = profile;
                    LeaderboardRestAPI.UpdateURL();
                    Draw.SetDirty(globalConfig);
                }

                if (Draw.Button("Check GameID", 120))
                {
                    var gameId = globalConfig.gameId;
                    var ret = await LeaderboardRestAPI.RegisterGameID(globalConfig.gameId, Application.identifier);
                    if (ret != null)
                    {
                        if (ret.code == 0)
                        {
                            Draw.DisplayDialog("MATCHED", $"GameId [{gameId}]\nAppId [{ret.appId}]\nSAME WITH SERVER", "OK");
                        }
                        else if (ret.code == -1)
                        {
                            if (Draw.DisplayDialog("EXISTS", $"GameId {gameId}\nUSED BY {ret.appId}", "CHANGE Game Id", "KEEP Game Id"))
                            {
                                globalConfig.gameId = StringExt.RandomAlphabetWithNumber(6);
                                Draw.SetDirty(globalConfig);
                            }
                        }
                        else if (ret.code == -2)
                        {
                            Draw.DisplayDialog("EMPTY", $"GameId and Application.Identifier must not empty!!", "OK");
                        }
                    }
                }
            }
        }
    }
}