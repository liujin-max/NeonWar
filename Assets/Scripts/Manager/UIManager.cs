using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static Transform BOTTOM;
    public static Transform MAJOR;
    public static Transform TIP;
    public static Transform BOARD;
    public static Transform EFFECT;
    public static Transform GUIDE;



    private Dictionary<string, GameObject> WindowCaches = new Dictionary<string, GameObject>();
    private HashSet<string> WindowsHash = new HashSet<string>();

    void Awake()
    {
        BOTTOM  = GameObject.Find("Canvas/BOTTOM").transform;
        MAJOR   = GameObject.Find("Canvas/MAJOR").transform;
        BOARD   = GameObject.Find("Canvas/BOARD").transform;
        EFFECT  = GameObject.Find("Canvas/EFFECT").transform;
        GUIDE   = GameObject.Find("Canvas/GUIDE").transform;
        TIP     = GameObject.Find("Canvas/TIP").transform;
    }

    // public GameObject LoadWindow(string path, Transform parent)
    // {
    //     path    = "Prefab/UI/Window/" + path;

    //     GameObject obj = Instantiate<GameObject>(Resources.Load<GameObject>(path), parent);
    //     WindowCaches[obj.name.Replace("(Clone)", "")] = obj;

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


        WindowsHash.Add(path);

        GameFacade.Instance.AssetManager.AsyncLoadPrefab("Prefab/UI/Window/" + path, Vector3.zero, parent, (obj)=>{
            obj.name = path;
            if (callback != null) callback(obj);
            WindowCaches[path] = obj;
        });
    }

    public void ShowWindow(string name)
    {
        var window = GetWindow(name);
        if (window == null) {
            return;
        }

        window.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    public void HideWindow(string name)
    {
        var window = GetWindow(name);
        if (window == null) {
            return;
        }

        window.GetComponent<RectTransform>().anchoredPosition = new Vector2(9999, 9999);
    }

    public void UnloadWindow(GameObject window)
    {
        WindowCaches.Remove(window.name);
        WindowsHash.Remove(window.name);

        Destroy(window);
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

    public GameObject LoadItem(string path, Transform parent)
    {
        path    = "Prefab/UI/Item/" + path;

        var obj = Instantiate<GameObject>(Resources.Load<GameObject>(path), parent);
        obj.transform.localPosition = Vector3.zero;

        return obj;
    }
}
