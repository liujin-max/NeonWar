using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static Transform BOTTOM;
    public static Transform MASK;
    public static Transform MAJOR;
    public static Transform NAV;
    public static Transform BOARD;
    public static Transform EFFECT;
    public static Transform GUIDE;
    public static Transform TIP;

    


    private Dictionary<string, GameObject> WindowCaches = new Dictionary<string, GameObject>();
    private HashSet<string> WindowsHash = new HashSet<string>();


    private int m_MajorCount = 0;

    void Awake()
    {
        BOTTOM  = GameObject.Find("Canvas/BOTTOM").transform;
        MASK    = GameObject.Find("Canvas/MASK").transform;
        MAJOR   = GameObject.Find("Canvas/MAJOR").transform;
        NAV     = GameObject.Find("Canvas/NAV").transform;
        BOARD   = GameObject.Find("Canvas/BOARD").transform;
        EFFECT  = GameObject.Find("Canvas/EFFECT").transform;
        GUIDE   = GameObject.Find("Canvas/GUIDE").transform;
        TIP     = GameObject.Find("Canvas/TIP").transform;

        MASK.localPosition = new Vector3(9999, 9999, 0);
    }

    // public GameObject LoadWindow(string path, Transform parent)
    // {
    //     GameObject obj = Instantiate<GameObject>(Resources.Load<GameObject>("Prefab/UI/Window/" + path), parent);
    //     WindowCaches[path] = obj;
    //     WindowsHash.Add(path);

    //     return obj;
    // }

    //异步加载UI
    public void LoadWindowAsync(string path, Transform parent , Action<GameObject> callback = null)
    {
        //如果存在这个界面
        if (WindowsHash.Contains(path)) {
            if (WindowCaches.ContainsKey(path)) {
                callback(WindowCaches[path]);
            }
            return;
        }

        if (parent == MAJOR) UpdateMaskVisible(1);

        WindowsHash.Add(path);

        GameFacade.Instance.AssetManager.AsyncLoadPrefab("Prefab/UI/Window/" + path, Vector3.zero, parent, (obj)=>{
            obj.name = path;
            if (callback != null) callback(obj);
            WindowCaches[path] = obj;
        });
    }

    public void UnloadWindow(GameObject window)
    {
        WindowCaches.Remove(window.name);
        WindowsHash.Remove(window.name);

        if (window.transform.parent == MAJOR) UpdateMaskVisible(-1);

        DestroyImmediate(window);
    }

    public void UnloadWindow(string name)
    {
        var window = GetWindow(name);
        if (window == null) {
            WindowsHash.Remove(name);
            return;
        }

        UnloadWindow(window);
    }

    public GameObject GetWindow(string name)
    {
        return WindowCaches.TryGetValue(name, out var window) ? window : null;
    }

    void UpdateMaskVisible(int value)
    {
        m_MajorCount += value;
        if (m_MajorCount > 0) MASK.localPosition = Vector3.zero;
        else MASK.localPosition = new Vector3(9999, 9999, 0);
    }

    public GameObject LoadItem(string path, Transform parent)
    {
        path    = "Prefab/UI/Item/" + path;

        var obj = Instantiate<GameObject>(Resources.Load<GameObject>(path), parent);
        obj.transform.localPosition = Vector3.zero;

        return obj;
    }


}
