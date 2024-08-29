using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


//负责管理单个对象的生命周期
//如果长时间未被使用，则从池子里移除
public class OBJEntity<T> where T : MonoBehaviour
{
    public T Entity;
    public float m_Timer;


    public OBJEntity(T e)
    {
        Entity = e;

        Reset();
    }

    public void CustomUpdate(float dt)
    {
        m_Timer -= dt;
    }

    public void Reset() {m_Timer = 60;}
    //长时间未使用
    public bool IsUnUsed() { return m_Timer <= 0;}

    public void Dispose()
    {
        if (Entity != null) GameObject.Destroy(Entity.gameObject);
    }
}


public class OBJManager: MonoBehaviour
{
    public Transform PoolLayer;

    //子弹池
    private Dictionary<string, OBJPool<Bullet>> m_BulletPools = new Dictionary<string, OBJPool<Bullet>>();
    //特效池
    private Dictionary<string, OBJPool<Effect>> m_EffectPools = new Dictionary<string, OBJPool<Effect>>();
    //血条池
    OBJPool<CircleHP> m_HPPools = new OBJPool<CircleHP>();



    void Awake()
    {
        PoolLayer = GameObject.Find("POOL").transform;
    }

    //控制缓存池的生命周期、收缩策略
    void Update()
    {
        float dt = Time.deltaTime;
        foreach (var item in m_BulletPools) {
            item.Value.CustomUpdate(dt);
        }
    }

    //加载子弹
    public Bullet AllocateBullet(GameObject bullet_template, Vector3 pos)
    {
        string name     = bullet_template.name;

        if (!m_BulletPools.ContainsKey(name)) {
            m_BulletPools.Add(name, new OBJPool<Bullet>());
        }

        Bullet bullet = m_BulletPools[name].Get(bullet_template);
        bullet.Name = name;
        bullet.transform.SetParent(Field.Instance.Land.ELEMENT_ROOT);
        bullet.transform.localPosition = pos;
        bullet.transform.localEulerAngles = Vector3.zero;

        return bullet;
    }

    //回收子弹
    public void RecycleBullet(Bullet bullet)
    {
        string name = bullet.Name;

        if (m_BulletPools[name].Has(bullet)) {
            Destroy(bullet.gameObject);
            return;
        }

        m_BulletPools[name].Recyle(bullet);
    }


    public Effect AllocateEffect(string effect_path, Vector3 pos)
    {
        if (!m_EffectPools.ContainsKey(effect_path)) {
            m_EffectPools.Add(effect_path, new OBJPool<Effect>(5));
        }


        Effect effect   = m_EffectPools[effect_path].Get(effect_path);
        effect.transform.SetParent(Field.Instance.Land.ELEMENT_ROOT.transform);
        effect.transform.localPosition = pos;
        effect.ResPath   = effect_path;
        effect.Restart();


        return effect;
    }

    public void RecycleEffect(Effect effect)
    {
        if (m_EffectPools[effect.ResPath].Has(effect)) {
            Destroy(effect.gameObject);
            return;
        }

        m_EffectPools[effect.ResPath].Recyle(effect);
    }


    //加载血条
    public CircleHP AllocateHP()
    {
        return m_HPPools.Get("Prefab/Element/CircleHP");
    }

    public void RecycleHP(CircleHP hp)
    {
        m_HPPools.Recyle(hp);
    }
}
