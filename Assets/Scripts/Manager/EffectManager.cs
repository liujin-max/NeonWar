using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    //加载UI特效
    public Effect LoadUIEffect(string path, Vector3 world_pos)
    {
        var e = GameFacade.Instance.EffectManager.Load(path, world_pos, UIManager.EFFECT.gameObject);
        e.transform.position = world_pos;
        e.transform.localScale = new Vector3(100, 100, 100);

        return e;
    }


    //加载特效
    public Effect Load(string path, Vector3 pos, GameObject parent = null)
    {
        var e = GameFacade.Instance.PoolManager.AllocateEffect(path, pos);

        if (parent != null) {
            e.transform.SetParent(parent.transform);
            e.transform.localPosition = pos;
        }

        return e.GetComponent<Effect>();
    }

    //销毁特效
    public void RemoveEffect(Effect e)
    {
        if (string.IsNullOrEmpty(e.ResPath) || !e.IsRecycle) {
            Destroy(e.gameObject);
            return;
        } 
        
        GameFacade.Instance.PoolManager.RecycleEffect(e);
    }
}
