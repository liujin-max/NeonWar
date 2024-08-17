using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundleBuilder
{
    static string AssetBundleDirectory = "Assets/StreamingAssets/AssetBundles";

    [MenuItem("AssetBundle/打包 AssetBundles")]
    static void BuildAllAssetBundles()
    {
        if (System.IO.Directory.Exists(AssetBundleDirectory))
        {
            Directory.Delete(AssetBundleDirectory, true);
        }
        
        System.IO.Directory.CreateDirectory(AssetBundleDirectory);

        BuildPipeline.BuildAssetBundles(AssetBundleDirectory,
            BuildAssetBundleOptions.ChunkBasedCompression,
            BuildTarget.WeixinMiniGame);


        EditorUtility.DisplayDialog("打包完成", "所有 AssetBundles 已成功构建！", "确定");
    }
}
