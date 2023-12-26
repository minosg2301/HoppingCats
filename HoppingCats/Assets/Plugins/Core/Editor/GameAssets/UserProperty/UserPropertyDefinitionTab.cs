using UnityEditor;
using UnityEngine;
using moonNest;

public partial class UserPropertyDefinitionTab : TabContent
{
    private readonly UserPropertyDefinitionDrawer userPropertyDefinitionDrawer;

    public UserPropertyDefinitionTab()
    {
        userPropertyDefinitionDrawer = new UserPropertyDefinitionDrawer();
    }

    public override void DoDraw()
    {
        userPropertyDefinitionDrawer.DoDraw(UserPropertyAsset.Ins.properties);

        Draw.Space(12);
        if (Draw.Button("Generate class", Color.magenta, Color.white, 150))
        {
            GenerateClass(UserPropertyAsset.Ins.properties);
            AssetDatabase.Refresh();
        }
    }
}