using System.Collections.Generic;
using UnityEngine;
using moonNest;

public class IAPGroupDrawer : ListTabDrawer<IAPPackageGroup>
{
    readonly IAPPackageTabDrawer iapPackageDrawer;

    public IAPGroupDrawer()
    {
        onElementAdded = (group) => IAPPackageAsset.Ins.AddGroup(group);
        onElementRemoved = (group) => IAPPackageAsset.Ins.RemoveGroup(group.id);
        onElementCloned = OnElementCloned;

        iapPackageDrawer = new IAPPackageTabDrawer();
    }

    void OnElementCloned(IAPPackageGroup newGroup, IAPPackageGroup origin)
    {
        newGroup.layerEnabled = false;
        var packages = IAPPackageAsset.Ins.FindByGroup(origin.id);
        foreach(var package in packages)
        {
            var newPackage = package.Clone() as IAPPackage;
            newPackage.groupId = newGroup.id;
            IAPPackageAsset.Ins.Add(newPackage);
        }
    }

    protected override IAPPackageGroup CreateNewElement() => new IAPPackageGroup("New Group");

    protected override string GetTabLabel(IAPPackageGroup group) => group.name;

    protected override void DoDrawContent(IAPPackageGroup group)
    {
        Draw.BeginHorizontal();
        {
            Draw.BeginVertical();
            {
                group.name = Draw.TextField("Name", group.name, 200);
                DrawLayerEnabled(group);
                Draw.BeginDisabledGroup(group.refreshConfig.enabled);
                group.sync = Draw.ToggleField("Sync", group.sync, 50);
                Draw.EndDisabledGroup();
                group.x2FirstBuy = Draw.ToggleField("X2 First Buy", group.x2FirstBuy, 50);
            }
            Draw.EndVertical();

            Draw.Space(30);
            Draw.BeginVertical();
            {
                Draw.BeginChangeCheck();
                group.refreshConfig.enabled = Draw.ToggleField("Refresh", group.refreshConfig.enabled, 150);
                Draw.BeginDisabledGroup(!group.refreshConfig.enabled);
                Draw.LimitField("Limit", group.refreshConfig.limit, 150);
                Draw.PeriodField("Period", group.refreshConfig.period, 150);
                Draw.EndDisabledGroup();
                if(Draw.EndChangeCheck())
                {
                    if(group.refreshConfig.enabled) group.sync = true;
                }
            }
            Draw.EndVertical();

            Draw.FlexibleSpace();
        }
        Draw.EndHorizontal();

        iapPackageDrawer.group = group;
        iapPackageDrawer.DoDraw(IAPPackageAsset.Ins.FindByGroup(group.id));
    }

    private void DrawLayerEnabled(IAPPackageGroup group)
    {
        var instance = IAPPackageAsset.Ins;
        Draw.BeginChangeCheck();
        Draw.BeginDisabledGroup(!instance.layerEnabled);
        group.layerEnabled = Draw.ToggleField(TextPack.Layer, group.layerEnabled, 120);
        if(group.layerEnabled)
        {
            LayerGroup layerGroup = LayerAsset.Ins.FindGroup(instance.layerGroupId);
            if(layerGroup)
                Draw.LabelBoldBox($"Rewards is overrided by Layer '{layerGroup.name}'", Color.blue);
            else
                Draw.LabelBoldBox("Select which Layer Group in LAYER TAB", Color.red);
        }
        Draw.EndDisabledGroup();
        if(Draw.EndChangeCheck())
        {
            LayerGroup layerGroup = LayerAsset.Ins.FindGroup(instance.layerGroupId);
            List<LayerDetail> layers = LayerAsset.Ins.FindByGroup(instance.layerGroupId);
            if(group.layerEnabled)
            {
                layerGroup.packageGroupIds.Add(group.id);
                List<IAPPackage> packages = instance.FindByGroup(group.id);
                layers.ForEach(layer => layer.iapPackageGroups.Add(new IAPPackageGroupLayer(group, packages)));
            }
            else
            {
                layerGroup.packageGroupIds.Remove(group.id);
                layers.ForEach(layer => layer.iapPackageGroups.Remove(c => c.groupId == group.id));
            }
        }
    }

    public override bool DoDrawWindow()
    {
        bool ret = base.DoDrawWindow();
        return !ret ? iapPackageDrawer.DoDrawWindow() : ret;
    }
}