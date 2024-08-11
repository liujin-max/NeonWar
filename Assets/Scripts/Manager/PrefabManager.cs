using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//预制体加载管理
public class PrefabManager : MonoBehaviour
{
    public GameObject Load(string path, Vector3 position, Transform parent)
    {
        var obj = Instantiate(Resources.Load<GameObject>(path), position, Quaternion.identity, parent);
        obj.transform.localEulerAngles = Vector3.zero;
        obj.transform.localPosition = position;

        return obj;
    }

    public void AsyncLoad(string path, Vector3 position, Transform parent, Action<GameObject> callback)
    {
        StartCoroutine(AsyncLoadPrefab(path, position, parent, callback));
    }

    IEnumerator AsyncLoadPrefab(string path, Vector3 position, Transform parent, Action<GameObject> callback)
    {
        ResourceRequest request = Resources.LoadAsync<GameObject>(path);
        yield return request;

        if (request.asset != null)
        {
            GameObject prefab = request.asset as GameObject;

            var obj = Instantiate(prefab, position, Quaternion.identity, parent);
            obj.transform.localEulerAngles = Vector3.zero;
            obj.transform.localPosition = position;

            if (callback != null) callback(obj);
        }
        else
        {
            Debug.LogError("Failed to load prefab from path: " + path);
        }
    }
}
