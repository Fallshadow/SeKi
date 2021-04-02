namespace Framework.AnimGraphs
{
    public class AssetHelper
    {
        public static int AssetPathToGUID(string assetPath)
        {
            return act.Resource.ResourceUtility.GetHashCodeByAssetPath(assetPath);
        }

    }
}

