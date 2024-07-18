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

    void Awake()
    {
        BOTTOM  = GameObject.Find("Canvas/BOTTOM").transform;
        MAJOR   = GameObject.Find("Canvas/MAJOR").transform;
        BOARD   = GameObject.Find("Canvas/BOARD").transform;
        EFFECT  = GameObject.Find("Canvas/EFFECT").transform;
        GUIDE   = GameObject.Find("Canvas/GUIDE").transform;
        TIP     = GameObject.Find("Canvas/TIP").transform;
    }

    public GameObject LoadWindow(string path, Transform parent)
    {
        path    = "Prefab/UI/Window/" + path;

        return Instantiate<GameObject>(Resources.Load<GameObject>(path), parent);
    }

    public void UnloadWindow(GameObject window)
    {
        Destroy(window);
    }

    //显示界面
    public GameObject ShowWindow(string name)
    {
        GameObject cache;
        if (WindowCaches.TryGetValue(name, out cache)) {
            // cache.SetActive(true);
            cache.transform.localPosition = Vector3.zero;
            WindowCaches.Remove(name);
            return cache;
        }
        return null;
    }

    //隐藏界面(不销毁)
    public void HideWindow(string name, GameObject obj)
    {
        GameObject cache;
        if (WindowCaches.TryGetValue(name, out cache)) {
            cache.SetActive(false);
            return;
        }

        obj.transform.localPosition = new Vector3(9999, 9999, 0);
        WindowCaches.Add(name, obj);
    }

    public GameObject LoadItem(string path, Transform parent)
    {
        path    = "Prefab/UI/Item/" + path;

        var obj = Instantiate<GameObject>(Resources.Load<GameObject>(path), parent);
        obj.transform.localPosition = Vector3.zero;

        return obj;
    }

    public GameObject LoadPrefab(string path, Vector3 position, Transform parent)
    {
        var obj = Instantiate(Resources.Load<GameObject>(path), position, Quaternion.identity, parent);
        obj.transform.localEulerAngles = Vector3.zero;
        obj.transform.localPosition = position;

        return obj;
    }

    public bool HasBoard()
    {
        return BOARD.childCount > 0;
    }
}
