using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.UI;

// 存储 AssetBundle 信息的内部类
public class ABInfo
{
    public AssetBundle AssetBundle;
    public List<GameObject>  ReferenceCounts;

    public ABInfo(AssetBundle assetBundle)
    {
        AssetBundle     = assetBundle;
        ReferenceCounts = new List<GameObject>();
    }
}

public static class AssetsManager
{
    private static Dictionary<string, ABInfo> m_AssetBundles = new Dictionary<string, ABInfo>();

    //GameObject和AssetBundle的对应关系
    private static Dictionary<GameObject, ABInfo> m_Obj2AssetBundles = new Dictionary<GameObject, ABInfo>();

    //卸载队列
    private static Dictionary<ABInfo, CDTimer> m_ABInfoRemoves = new Dictionary<ABInfo, CDTimer>();
    private static List<ABInfo> m_Removes = new List<ABInfo>();



    //获取AssetBundle
    static ABInfo GetAssetBundleInfo(string dictionary)
    {
        StringBuilder path_sb   = new StringBuilder(Application.streamingAssetsPath);
        path_sb.Append("/AssetBundles/");
        path_sb.Append(dictionary.ToLower());

        var res_path = path_sb.ToString();

        if (!m_AssetBundles.ContainsKey(dictionary))
        {
            m_AssetBundles[dictionary] = new ABInfo(AssetBundle.LoadFromFile(res_path));
        }

        return m_AssetBundles[dictionary];
    }


    /// <summary>
    /// 加载Sprite
    /// </summary>
    /// <param name="dictionary">目录名称, 和AssetBundle的标签是一致的</param>
    /// <param name="name">资源名称,不包含后缀</param>
    /// <param name="obj">使用Sprite的GameObject</param>
    /// <returns></returns>
    public static Sprite LoadSprite(string dictionary, string name, GameObject obj)
    {
        //标签就是目录名称的小写
        dictionary = dictionary.ToLower();

        //如果当前obj正在使用其他的AB包中的资源，则先卸载计数
        AssetsManager.Unload(obj);

        ABInfo ab_info  = GetAssetBundleInfo(dictionary);
        Sprite sprite   = ab_info.AssetBundle.LoadAsset<Sprite>(name);

        //记录计数
        m_Obj2AssetBundles.Add(obj, ab_info);
        ab_info.ReferenceCounts.Add(obj);

        return sprite;
    }

    //卸载引用计数
    public static void Unload(GameObject obj)
    {
        if (!m_Obj2AssetBundles.ContainsKey(obj)) return;

        var ab_info = m_Obj2AssetBundles[obj];
        ab_info.ReferenceCounts.Remove(obj);
        m_Obj2AssetBundles.Remove(obj);

        if (ab_info.ReferenceCounts.Count <= 0)
        {
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
