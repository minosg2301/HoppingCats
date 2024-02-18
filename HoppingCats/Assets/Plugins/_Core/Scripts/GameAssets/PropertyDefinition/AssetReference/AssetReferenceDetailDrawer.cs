#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

public class AssetReferenceDetailDrawer
{
    public AssetReference assetReference;
    public string newGuid = "";
    public Rect assetDropDownRect;
    public string assetName;
    public SubassetDetailPopup subassetPopup;
    
    Object subAsset;

    public AssetReferenceDetailDrawer(AssetReference assetReference)
    {
        this.assetReference = assetReference;
    }

    public void Draw(float maxWidth)
    {
        string guid = assetReference.AssetGUID;
        AddressableAssetSettings aaSettings = AddressableAssetSettingsDefaultObject.Settings;
        string checkToForceAddressable = string.Empty;
        if(!string.IsNullOrEmpty(newGuid))
        {
            if(newGuid == "None (AddressableAsset)")
            {
                SetObject(null, out guid);
                newGuid = string.Empty;
            }
            else
            {
                bool success = SetObject(AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(newGuid)), out guid);
                if(success) checkToForceAddressable = newGuid;
                newGuid = string.Empty;
            }
        }
        bool isNotAddressable = false;
        assetName = "None (AddressableAsset)";
        if(aaSettings != null && !string.IsNullOrEmpty(guid))
        {
            AddressableAssetEntry entry = aaSettings.FindAssetEntry(guid);
            if(entry != null)
            {
                assetName = entry.address;
            }
            else
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if(!string.IsNullOrEmpty(path))
                {
                    if(!IsAssetPathInAddressableDirectory(aaSettings, path, out assetName))
                    {
                        assetName = path;
                        if(!string.IsNullOrEmpty(checkToForceAddressable))
                        {
                            AddressableAssetEntry newEntry = aaSettings.CreateOrMoveEntry(guid, aaSettings.DefaultGroup, false, true);
                        }
                        else
                        {
                            if(!File.Exists(path)) assetName = "Missing File!";
                            else isNotAddressable = true;
                        }
                    }
                }
                else
                {
                    assetName = "Missing File!";
                }
            }
        }

        assetDropDownRect = EditorGUILayout.GetControlRect(GUILayout.MaxWidth(maxWidth));
        string nameToUse = GetNameForAsset(isNotAddressable);
        if(assetReference.editorAsset)
        {
            List<Object> subAssets = GetSubAssetsList();
            if(subAssets.Count > 1) assetDropDownRect = DrawSubAssetsControl(subAssets);
        }

        DrawControl(nameToUse, isNotAddressable, guid);
    }

    public void DrawSubSprite(bool autoSize, float maxWidth, float maxHeight)
    {
        if (assetReference.editorAsset && subAsset == null)
        {
            List<Object> subAssets = GetSubAssetsList();
            GetSelectedSubassetIndex(subAssets, out int subIndex, out string[] objNames);
            if(subIndex > 0)
            {
                SetSubAssets(subAssets[subIndex]);
            }
        }

        if (subAsset == null || subAsset.GetType() != typeof(Sprite)) return;

        var sprite = subAsset as Sprite;

        if (autoSize)
        {
            maxWidth = Mathf.Min(sprite ? sprite.rect.width : 80, maxWidth);
            maxHeight = Mathf.Min(sprite ? sprite.rect.height : 80, maxHeight);
        }

        var rect = EditorGUILayout.GetControlRect(GUILayout.Width(maxWidth), GUILayout.Height(maxHeight));
        var a = EditorGUI.ObjectField(rect, sprite, typeof(Sprite), false) as Sprite;
    }

    public bool SetSubAssets(Object subAsset)
    {
        this.subAsset = subAsset;
        return assetReference.SetEditorSubObject(subAsset);
    }

    private Rect DrawSubAssetsControl(List<Object> subAssets)
    {
        assetDropDownRect = new Rect(assetDropDownRect.position, new Vector2(assetDropDownRect.width / 2f, assetDropDownRect.height));
        Rect objRect = new Rect(assetDropDownRect.xMax, assetDropDownRect.y, assetDropDownRect.width, assetDropDownRect.height);

        GetSelectedSubassetIndex(subAssets, out int selIndex, out string[] objNames);

        bool isFieldPressed = Event.current.type == EventType.MouseDown && Event.current.button == 0 && objRect.Contains(Event.current.mousePosition);
        if(isFieldPressed)
        {
            subassetPopup = new SubassetDetailPopup(this, selIndex, objNames, subAssets);
            PopupWindow.Show(objRect, subassetPopup);
        }
        if(subassetPopup != null && subassetPopup.SelectionChanged)
        {
            subassetPopup.UpdateSubAssets();
        }
        GUIContent nameSelected = new GUIContent(objNames[selIndex]);
        GUI.Box(objRect, nameSelected, EditorStyles.textField);
        return assetDropDownRect;
    }

    private void GetSelectedSubassetIndex(List<Object> subAssets, out int selIndex, out string[] objNames)
    {
        objNames = new string[subAssets.Count];
        selIndex = 0;
        for(int i = 0; i < subAssets.Count; i++)
        {
            Object subAsset = subAssets[i];
            string objName = !subAsset ? "<none>" : subAsset.name;
            if(objName.EndsWith("(Clone)"))
                objName = objName.Replace("(Clone)", "");
            objNames[i] = !subAsset ? objName : string.Format("{0}:{1}", objName, subAsset.GetType());
            if(!subAsset || assetReference.SubObjectName == objName)
                selIndex = i;
        }
    }

    private bool SetObject(Object target, out string guid)
    {
        guid = null;
        try
        {
            if(assetReference == null) return false;

            if(target)
            {
                Object subObject = null;
                if(target.GetType() == typeof(Sprite))
                {
                    List<AddressableAssetEntry> atlasEntries = new List<AddressableAssetEntry>();
                    AddressableAssetSettingsDefaultObject.Settings.GetAllAssets(atlasEntries, false, null, e => AssetDatabase.GetMainAssetTypeAtPath(e.AssetPath) == typeof(SpriteAtlas));

                    string spriteName = target.name;
                    if(spriteName.EndsWith("(Clone)"))
                        spriteName = spriteName.Replace("(Clone)", "");

                    foreach(AddressableAssetEntry a in atlasEntries)
                    {
                        SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(a.AssetPath);
                        if(atlas)
                        {
                            Sprite s = atlas.GetSprite(spriteName);
                            if(s)
                            {
                                subObject = target;
                                target = atlas;
                                break;
                            }
                        }
                    }
                }
                guid = SetSingleAsset(target, subObject);
            }
            else
                guid = SetSingleAsset(null, null);
        }
        catch(Exception e)
        {
            Debug.LogException(e);
            return false;

        }

        return true;
    }

    private string SetSingleAsset(Object asset, Object subObject)
    {
        string guid = null;
        string result;
        if(!asset)
        {
            assetReference.SetEditorAsset(null);
            result = guid;
        }
        else
        {
            if(assetReference.SetEditorAsset(asset))
            {
                if(subObject)
                    assetReference.SetEditorSubObject(subObject);
                else
                    assetReference.SubObjectName = null;

                guid = assetReference.AssetGUID;
            }
            result = guid;
        }
        return result;
    }

    private string GetNameForAsset(bool isNotAddressable)
    {
        string nameToUse = assetName;
        if(isNotAddressable)
        {
            nameToUse = "Not Addressable - " + nameToUse;
        }
        return nameToUse;
    }

    private List<Object> GetSubAssetsList()
    {
        List<Object> subAssets = new List<Object>() { null };
        string assetPath = AssetDatabase.GUIDToAssetPath(assetReference.AssetGUID);
        subAssets.AddRange(AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath));
        Type mainType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
        if(mainType == typeof(SpriteAtlas))
        {
            SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(assetPath);
            Sprite[] sprites = new Sprite[atlas.spriteCount];
            atlas.GetSprites(sprites);
            subAssets.AddRange(sprites);
        }
        return subAssets;
    }

    private bool IsAssetPathInAddressableDirectory(AddressableAssetSettings aasettings, string assetPath, out string assetName)
    {
        bool flag = !string.IsNullOrEmpty(assetPath);
        if(flag)
        {
            string dir = Path.GetDirectoryName(assetPath);
            while(!string.IsNullOrEmpty(dir))
            {
                AddressableAssetEntry dirEntry = aasettings.FindAssetEntry(AssetDatabase.AssetPathToGUID(dir));
                if(dirEntry != null)
                {
                    assetName = dirEntry.address + assetPath.Remove(0, dir.Length);
                    return true;
                }
                dir = Path.GetDirectoryName(dir);
            }
        }
        assetName = "";
        return false;
    }

    private void DrawControl(string nameToUse, bool isNotAddressable, string guid)
    {
        bool isFieldPressed = Event.current.type == EventType.MouseDown && Event.current.button == 0 && assetDropDownRect.Contains(Event.current.mousePosition);
        bool isEnterKeyPressed = Event.current.type == EventType.KeyDown && Event.current.isKey && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return);
        if(isFieldPressed || isEnterKeyPressed)
            Event.current.Use();

        Object asset = assetReference?.editorAsset;
        if(asset)
        {
            GUI.Box(assetDropDownRect, new GUIContent(nameToUse), EditorStyles.textField);
            if(Event.current.clickCount == 2)
            {
                AssetDatabase.OpenAsset(asset);
                GUIUtility.ExitGUI();
            }
        }
        else
        {
            GUI.Box(assetDropDownRect, new GUIContent(nameToUse), EditorStyles.textField);
        }
        if(isFieldPressed)
        {
            string nonAddressedOption = isNotAddressable ? assetName : string.Empty;
            PopupWindow.Show(assetDropDownRect, new AssetReferenceDetailPopup(this, nonAddressedOption));
        }
    }
}

public class AssetReferenceDetailPopup : PopupWindowContent
{
    private AssetReferenceTreeView m_Tree;

    private TreeViewState m_TreeState;
    private bool shouldClose;
    private string m_CurrentName = string.Empty;
    private AssetReferenceDetailDrawer drawer;
    private string nonAddressedAsset;
    private SearchField searchField;

    private void ForceClose()
    {
        shouldClose = true;
    }

    public AssetReferenceDetailPopup(AssetReferenceDetailDrawer drawer, string nonAddressedAsset)
    {
        this.drawer = drawer;
        this.nonAddressedAsset = nonAddressedAsset;
        searchField = new SearchField();
        shouldClose = false;
    }

    public override void OnOpen()
    {
        searchField.SetFocus();
        base.OnOpen();
    }

    public override Vector2 GetWindowSize()
    {
        Vector2 result = base.GetWindowSize();
        result.x = drawer.assetDropDownRect.width;
        return result;
    }

    public override void OnGUI(Rect rect)
    {
        int border = 4;
        int topPadding = 12;
        int searchHeight = 20;
        Rect searchRect = new Rect(border, topPadding, rect.width - (border * 2), searchHeight);
        int remainTop = topPadding + searchHeight + border;
        Rect remainingRect = new Rect(border, (topPadding + searchHeight + border), rect.width - (border * 2), rect.height - remainTop - border);
        m_CurrentName = searchField.OnGUI(searchRect, m_CurrentName);
        bool flag = m_Tree == null;
        if(flag)
        {
            bool flag2 = m_TreeState == null;
            if(flag2)
            {
                m_TreeState = new TreeViewState();
            }
            m_Tree = new AssetReferenceTreeView(drawer, m_TreeState, this, nonAddressedAsset);
            m_Tree.Reload();
        }
        m_Tree.searchString = m_CurrentName;
        m_Tree.OnGUI(remainingRect);
        if(shouldClose)
        {
            GUIUtility.hotControl = 0;
            editorWindow.Close();
        }
    }

    private sealed class AssetRefTreeViewItem : TreeViewItem
    {
        public string AssetPath;

        private string guid;
        public string Guid { get { if(string.IsNullOrEmpty(guid)) guid = AssetDatabase.AssetPathToGUID(AssetPath); return guid; } }

        public AssetRefTreeViewItem(int id, int depth, string displayName, string path) : base(id, depth, displayName)
        {
            AssetPath = path;
            icon = (AssetDatabase.GetCachedIcon(path) as Texture2D);
        }
    }

    internal class AssetReferenceTreeView : TreeView
    {
        private AssetReferenceDetailDrawer drawer;
        private AssetReferenceDetailPopup popup;
        private string nonAddressedAsset;
        private Texture2D warningIcon;

        public AssetReferenceTreeView(AssetReferenceDetailDrawer drawer, TreeViewState state, AssetReferenceDetailPopup popup, string nonAddressedAsset) : base(state)
        {
            this.drawer = drawer;
            this.popup = popup;
            this.nonAddressedAsset = nonAddressedAsset;
            showBorder = true;
            showAlternatingRowBackgrounds = true;
            warningIcon = EditorGUIUtility.FindTexture("console.warnicon");
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return false;
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            bool flag = selectedIds != null && selectedIds.Count == 1;
            if(flag)
            {
                AssetRefTreeViewItem assetRefItem = FindItem(selectedIds[0], rootItem) as AssetRefTreeViewItem;
                if(assetRefItem != null && !string.IsNullOrEmpty(assetRefItem.AssetPath))
                    drawer.newGuid = assetRefItem.Guid;
                else
                    drawer.newGuid = "None (AddressableAsset)";
                popup.ForceClose();
            }
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            bool flag = string.IsNullOrEmpty(base.searchString);
            IList<TreeViewItem> result;
            if(flag)
            {
                result = base.BuildRows(root);
            }
            else
            {
                List<TreeViewItem> rows = new List<TreeViewItem>();
                foreach(TreeViewItem child in base.rootItem.children)
                {
                    bool flag2 = child.displayName.IndexOf(base.searchString, StringComparison.OrdinalIgnoreCase) >= 0;
                    if(flag2)
                    {
                        rows.Add(child);
                    }
                }
                result = rows;
            }
            return result;
        }

        protected override TreeViewItem BuildRoot()
        {
            TreeViewItem root = new TreeViewItem(-1, -1);
            AddressableAssetSettings aaSettings = AddressableAssetSettingsDefaultObject.Settings;
            bool flag = aaSettings == null;
            if(flag)
            {
                string message = "Use 'Window->Addressables' to initialize.";
                root.AddChild(new AssetRefTreeViewItem(message.GetHashCode(), 0, message, string.Empty));
            }
            else
            {
                if(!string.IsNullOrEmpty(nonAddressedAsset))
                {
                    root.AddChild(new AssetRefTreeViewItem(nonAddressedAsset.GetHashCode(), 0, "Make Addressable - " + nonAddressedAsset, string.Empty)
                    {
                        icon = warningIcon
                    });
                }
                root.AddChild(new AssetRefTreeViewItem("None (AddressableAsset)".GetHashCode(), 0, "None (AddressableAsset)", string.Empty));
                List<ReferenceEntryData> allAssets = new List<ReferenceEntryData>();
                aaSettings.GatherAllAssetReferenceDrawableEntries(allAssets);
                foreach(ReferenceEntryData entry in allAssets)
                {
                    if(!entry.IsInResources)
                    {
                        AssetRefTreeViewItem child = new AssetRefTreeViewItem(entry.AssetPath.GetHashCode(), 0, entry.Address, entry.AssetPath);
                        root.AddChild(child);
                    }
                }
            }
            return root;
        }
    }
}

public static class AddressableAssetExtension
{
    internal static void GatherAllAssetReferenceDrawableEntries(this AddressableAssetSettings setting, List<ReferenceEntryData> assets)
    {
        foreach(AddressableAssetGroup g in setting.groups)
        {
            if(g) g.GatherAllAssetReferenceDrawableEntries(assets);
        }
    }

    internal static void GatherAllAssetReferenceDrawableEntries(this AddressableAssetGroup group, List<ReferenceEntryData> results)
    {
        foreach(AddressableAssetEntry e in group.entries)
        {
            if(e.MainAsset && e.TargetAsset)
            {
                ReferenceEntryData reference = new ReferenceEntryData { Address = e.address, AssetPath = e.AssetPath, IsInResources = e.IsInResources };
                results.Add(reference);
            }
        }
    }
}

public class ReferenceEntryData
{
    public string AssetPath { get; set; }
    public string Address { get; set; }
    public bool IsInResources { get; set; }
}

public class SubassetDetailPopup : PopupWindowContent
{
    private readonly AssetReferenceDetailDrawer drawer;

    public int SelectedIndex { get; set; }
    public bool SelectionChanged { get; set; }
    readonly string[] objNames;
    readonly List<Object> subAssets;
    Vector2 scrollPosition;


    public SubassetDetailPopup(AssetReferenceDetailDrawer drawer, int selIndex, string[] objNames, List<Object> subAssets)
    {
        this.drawer = drawer;
        SelectedIndex = selIndex;
        this.objNames = objNames;
        this.subAssets = subAssets;
    }

    public void UpdateSubAssets()
    {
        if(SelectionChanged)
        {
            if(!drawer.SetSubAssets(this.subAssets[this.SelectedIndex]))
            {
                Debug.LogError("Unable to set all of the objects selected subassets");
            }
            SelectionChanged = false;
        }
    }

    public override void OnGUI(Rect rect)
    {
        GUIStyle buttonStyle = new GUIStyle
        {
            fontStyle = 0,
            fontSize = 12,
            contentOffset = new Vector2(10f, 0f)
        };
        buttonStyle.normal.textColor = Color.white;
        EditorGUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
        scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition, new GUILayoutOption[] { GUILayout.Width(rect.width), GUILayout.Height(rect.height) });
        for(int i = 0; i < this.objNames.Length; i++)
        {
            if(GUILayout.Button(this.objNames[i], buttonStyle, Array.Empty<GUILayoutOption>()))
            {
                if(SelectedIndex != i)
                {
                    SelectedIndex = i;
                    SelectionChanged = true;
                }
                EditorWindow.focusedWindow.Close();
                break;
            }
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
}
#endif