using UnityEditor;
using UnityEngine;

public class AssetBundleBuildTest : MonoBehaviour
{
    [MenuItem("UnityStudy/Asset/BuildAssetBundle/WithNone")]
    public static void WithNone()
    {
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }

    [MenuItem("UnityStudy/Asset/BuildAssetBundle/WithChunkLZ4")]
    public static void WithChunkLZ4()
    {
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows);
    }

    [MenuItem("UnityStudy/Asset/BuildAssetBundle/WithOutTypeTreeWithChunkLZ4")]
    public static void WithOutTypeTreeWithChunkLZ4()
    {
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DisableWriteTypeTree, BuildTarget.StandaloneWindows);
    }

    [MenuItem("UnityStudy/Asset/BuildAssetBundle/WithOutNameHashWithChunkLZ4")]
    public static void WithOutNameHashWithChunkLZ4()
    {
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DisableLoadAssetByFileName | BuildAssetBundleOptions.DisableLoadAssetByFileNameWithExtension, BuildTarget.StandaloneWindows);
    }
}
