using UnityEngine;
using moonNest;

public class FeatureTab : TabContent
{
    readonly TabContainer tabContainer;
    readonly TabItem featureUnlockTab;
    readonly TabItem templateTab;

    public FeatureTab()
    {
        featureUnlockTab = new TabItem("Features", new LockFeatureTab());
        templateTab = new TabItem("Templates", new QuestTemplateTab());

        tabContainer = new TabContainer();
        tabContainer.AddTab("Quest", new QuestAssetTab());
        tabContainer.AddTab("Online Reward", new OnlineRewardTab());
        tabContainer.AddTab("Achievement", new AchievementTab());
        tabContainer.AddTab("Spin", new SpinTab());
        tabContainer.AddTab("Lucky Box", new LuckyBoxTab());
        tabContainer.AddTab(featureUnlockTab);
        tabContainer.AddTab(templateTab);
        tabContainer.HeaderType = HeaderType.Vertical;
        tabContainer.TabWidth = 150;
        tabContainer.DrawShortHeader = false;
    }

    public override void DoDraw()
    {
        tabContainer.DoDraw();
    }

    public override bool DoDrawWindow()
    {
        if (!base.DoDrawWindow())
        {
            return tabContainer.DoDrawWindow();
        }
        return true;   
    }

}