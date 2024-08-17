using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

// 存储 AssetBundle 信息的内部类
public class ABInfo
{
    public AssetBundle AssetBundle;
    public int  ReferenceCounts;

    public ABInfo(AssetBundle assetBundle)
    {
        AssetBundle     = assetBundle;
    }
}

public static class AssetsManager
{
    private static string m_AssetBundlePath = Application.streamingAssetsPath + "/AssetBundles/";
    private static AssetBundleManifest m_Manifest = null;

    private static Dictionary<string, ABInfo> m_AssetBundles = new Dictionary<string, ABInfo>();

    //记录GameObject是否加载过Sprite
    private static Dictionary<GameObject, ABInfo> m_SpriteRecords = new Dictionary<GameObject, ABInfo>();

    
    //卸载队列
    private static Dictionary<ABInfo, CDTimer> m_ABInfoRemoves = new Dictionary<ABInfo, CDTimer>();
    private static List<ABInfo> m_Removes = new List<ABInfo>();


    static AssetBundleManifest GetManifest()
    {
        if (m_Manifest == null)
        {
            // 假设你的AssetBundle目录是StreamingAssets/AssetBundles/
            // 先加载主Manifest文件，它会包含所有的依赖信息
            AssetBundle manifestBundle = AssetBundle.LoadFromFile(m_AssetBundlePath + "AssetBundles");
            if (manifestBundle == null)
            {
                Debug.LogError("manifestBundle is null");
                return null;
            }

            m_Manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        return m_Manifest;
    }

    //加载AssetBundle及依赖项
    static ABInfo LoadAssetBundleWithDependencies(string bundleName)
    {
        if (m_AssetBundles.ContainsKey(bundleName))
        {
            Debug.Log(bundleName + " is already loaded.");
            return m_AssetBundles[bundleName];
        }


        // 获取依赖项
        string[] dependencies = GetManifest().GetAllDependencies(bundleName);
        // 先加载所有依赖项
        foreach (string dependency in dependencies)
        {
            if (!m_AssetBundles.ContainsKey(dependency))
            {
                string dependencyPath = m_AssetBundlePath + dependency;
                AssetBundle dependencyBundle = AssetBundle.LoadFromFile(dependencyPath);
                if (dependencyBundle != null) {
                    m_AssetBundles[dependency] = new ABInfo(dependencyBundle);
                    m_AssetBundles[dependency].ReferenceCounts++;
                }
            }
        }

        // 最后加载主AssetBundle
        StringBuilder path_sb   = new StringBuilder(m_AssetBundlePath);
        path_sb.Append(bundleName.ToLower());

        m_AssetBundles[bundleName] = new ABInfo(AssetBundle.LoadFromFile(path_sb.ToString()));

        return m_AssetBundles[bundleName];
    }


    /// <summary>
    /// 加载Sprite
    /// </summary>
    /// <param name="bundlename">目录名称, 和AssetBundle的标签是一致的</param>
    /// <param name="name">资源名称,不包含后缀</param>
    /// <param name="obj">使用Sprite的GameObject</param>
    /// <returns></returns>
    public static Sprite LoadSprite(string bundlename, string name, GameObject obj)
    {
        //标签就是目录名称的小写
        bundlename = bundlename.ToLower();

        //如果当前obj正在使用其他的AB包中的资源，则先卸载原AssetBundle的计数
        if (m_SpriteRecords.ContainsKey(obj)) m_SpriteRecords[obj].ReferenceCounts--;

        ABInfo ab_info  = LoadAssetBundleWithDependencies(bundlename);
        Sprite sprite   = ab_info.AssetBundle.LoadAsset<Sprite>(name);

        //记录计数
        ab_info.ReferenceCounts++;
        m_SpriteRecords[obj] = ab_info;

        return sprite;
    }

    //加载Prefab
    public static GameObject LoadPrefab(string bundlename, string name)
    {
        //标签就是目录名称的小写
        bundlename = bundlename.ToLower();

        ABInfo ab_info  = LoadAssetBundleWithDependencies(bundlename);
        GameObject obj  = ab_info.AssetBundle.LoadAsset<GameObject>(name);

        ab_info.ReferenceCounts++;

        return obj;
    }

    //卸载引用计数
    public static void Unload(string bundleName)
    {
        if (!m_AssetBundles.ContainsKey(bundleName)) return;

        //先减少主AssetBundle的引用计数
        var ab_info = m_AssetBundles[bundleName];
        ab_info.ReferenceCounts--;


        // 再处理依赖项
        string[] dependencies = GetManifest().GetAllDependencies(bundleName);
        foreach (string dependency in dependencies)
        {
            if (m_AssetBundles.ContainsKey(dependency)) m_AssetBundles[dependency].ReferenceCounts--;
        }
    }

    //回收卸载未时候的AB
    public static void Recyle()
    {
        foreach (var item in m_AssetBundles)
        {
            var ab_info = item.Value;

            if (ab_info.ReferenceCounts <= 0 && !m_ABInfoRemoves.ContainsKey(ab_info))
                m_ABInfoRemoves.Add(ab_info, new CDTimer(1.0f));
        }
    }


    public static void CustomUpdate(float dt)
    {
        foreach (var item in m_ABInfoRemoves)
        {
            ABInfo key = item.Key;
            m_ABInfoRemoves[key].Update(dt);

            if (m_ABInfoRemoves[key].IsFinished())
            {
                m_Removes.Add(key);  
            }
        }

        m_Removes.ForEach(ab_info => {
            Debug.LogError("卸载AssetBundle ：" + ab_info.AssetBundle.name);

            m_AssetBundles.Remove(ab_info.AssetBundle.name);
            m_ABInfoRemoves.Remove(ab_info);

            ab_info.AssetBundle.Unload(true); 
        });
        m_Removes.Clear();
    }
}
