using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    //加载UI特效
    public GameObject LoadUIEffect(string path, Vector3 world_pos)
    {
        var e = GameFacade.Instance.EffectManager.Load(path, world_pos, UIManager.EFFECT.gameObject);
        e.transform.position = world_pos;

        return e;
    }


    //加载特效
    public GameObject Load(string path, Vector3 pos, GameObject parent = null)
    {
        var e = GameFacade.Instance.PoolManager.AllocateEffect(path, pos); //Instantiate(Resources.Load<GameObject>(path), pos, Quaternion.identity);

        if (parent != null) {
            e.transform.SetParent(parent.transform);
            e.transform.localPosition = pos;
        }

        return e;
    }
}
