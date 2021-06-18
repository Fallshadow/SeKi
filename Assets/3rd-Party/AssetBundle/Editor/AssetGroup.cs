using System.Collections.Generic;

public class AssetItem
{
    const int DEFAULT_IMPORTANCE = 4;

    public List<string> subLevelNames;
    public string assetPath;
    public int importance = DEFAULT_IMPORTANCE;
    public bool bAtlas;
}

public class AssetGroup
{
    public int Importance;
    public bool IsCommon; // 供优化使用
    public bool IsSVC = false; // 是否SVC标签
    public int InstanceId;
    public List<AssetItem> Assets = new List<AssetItem>();

    private string groupName;

    public string GroupName
    {
        get
        {
            return groupName;
        }
        set
        {
            groupName = value;
            if (groupName.IndexOf('&') != -1)
            {
                IsCommon = true;
            }
        }
    }
    
    public bool IsValid { get { return Assets.Count != 0; } }

    public int AssetCount { get { return Assets.Count; } }
}