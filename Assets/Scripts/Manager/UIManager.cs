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

        GameObject obj = Instantiate<GameObject>(Resources.Load<GameObject>(path), parent);
        WindowCaches[obj.name.Replace("(Clone)", "")] = obj;

        return obj;
    }

    public void UnloadWindow(GameObject window)
    {
        WindowCaches[window.name] = null;

        Destroy(window);
    }

    public GameObject GetWindow(string name)
    {
        GameObject window;
        if (WindowCaches.TryGetValue(name, out window)) {
            return window;
        }
        return null;
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
