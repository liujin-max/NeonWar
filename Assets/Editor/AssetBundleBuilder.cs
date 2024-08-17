using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundleBuilder
{
    static string AssetBundleDirectory = "Assets/StreamingAssets/AssetBundles";

    [MenuItem("AssetBundle/打包 编辑器AssetBundles")]
    static void BuildWindowsAssetBundles()
    {
        if (System.IO.Directory.Exists(AssetBundleDirectory))
        {
            Directory.Delete(AssetBundleDirectory, true);
        }
        
        System.IO.Directory.CreateDirectory(AssetBundleDirectory);

        BuildPipeline.BuildAssetBundles(AssetBundleDirectory,
            BuildAssetBundleOptions.ChunkBasedCompression,
            BuildTarget.StandaloneWindows);


        EditorUtility.DisplayDialog("打包完成", "所有 AssetBundles 已成功构建！", "确定");
    }

    [MenuItem("AssetBundle/打包 WX AssetBundles")]
    static void BuildWXAssetBundles()
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
