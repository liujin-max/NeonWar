using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class AssetManager : MonoBehaviour
{
    Dictionary<string, SpriteAtlas> m_SpriteAtlas = new Dictionary<string, SpriteAtlas>();


    //从SpriteAtlas中读取
    public Sprite LoadSprite(string atlas_path, string sprite_name)
    {
        atlas_path  = "Atlas/Dynamic/" + atlas_path;
        if (!m_SpriteAtlas.ContainsKey(atlas_path)) 
        {
            m_SpriteAtlas[atlas_path] = Resources.Load<SpriteAtlas>(atlas_path);  //图集名称
        }

        Sprite spr = m_SpriteAtlas[atlas_path].GetSprite(sprite_name);

        return spr;
    }

    public GameObject LoadPrefab(string path, Vector3 position, Transform parent)
    {
        var obj = Instantiate(Resources.Load<GameObject>(path), position, Quaternion.identity, parent);
        obj.transform.localEulerAngles = Vector3.zero;
        obj.transform.localPosition = position;

        return obj;
    }

    public void AsyncLoadPrefab(string path, Vector3 position, Transform parent = null, Action<GameObject> callback = null)
    {
        StartCoroutine(AsyncLoad(path, position, parent, callback));
    }

    IEnumerator AsyncLoad(string path, Vector3 position, Transform parent, Action<GameObject> callback)
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
