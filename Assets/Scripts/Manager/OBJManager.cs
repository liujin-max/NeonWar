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
    private Transform PoolLayer;

    private Dictionary<string, OBJPool<Bullet>> m_BulletPools = new Dictionary<string, OBJPool<Bullet>>();

    //特效池
    private Dictionary<string, List<GameObject>> m_EffectPool = new Dictionary<string, List<GameObject>>();
    private int m_EffectMax = 3;    //同种特效最多存3个


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
        bullet.gameObject.SetActive(true);
        bullet.transform.SetParent(Field.Instance.Land.ELEMENT_ROOT);
        bullet.transform.localPosition = pos;
        bullet.transform.localEulerAngles = Vector3.zero;

        return bullet;
    }

    //回收子弹
    public void RecycleBullet(Bullet bullet)
    {
        string name = bullet.Name;

        bullet.transform.SetParent(PoolLayer);
        bullet.gameObject.SetActive(false);

        // 正常不会有这种情况
        // if (!m_BulletPools.ContainsKey(name)) {
        //     m_BulletPools.Add(name, new OBJPool<Bullet>());
        // }

        if (m_BulletPools[name].Has(bullet)) return;

        m_BulletPools[name].Recyle(bullet);
    }


    public GameObject AllocateEffect(string effect_path, Vector3 pos)
    {
        GameObject effect = null;

        List<GameObject> effect_list;
        if (m_EffectPool.TryGetValue(effect_path, out effect_list) != true)
        {
            effect_list = new List<GameObject>();
            m_EffectPool.Add(effect_path, effect_list);
        }


        if (effect_list.Count > 0) {
            effect = effect_list[0];
            effect.transform.SetParent(SceneManager.GetActiveScene().GetRootGameObjects()[0].transform);
            effect.transform.localPosition = pos;
            effect_list.RemoveAt(0);
        } else {
            effect = Instantiate(Resources.Load<GameObject>(effect_path), pos, Quaternion.identity);
        }


        Effect effect_cs    = effect.GetComponent<Effect>();
        effect_cs.ResPath   = effect_path;
        effect_cs.Restart();
        effect.SetActive(true);


        return effect;
    }

    public void RecycleEffect(Effect effect)
    {
        List<GameObject> list = m_EffectPool[effect.ResPath];
        if (list.Count >= m_EffectMax) {  //只存3个
            Destroy(effect.gameObject);
            return;
        }
        list.Add(effect.gameObject);

        effect.transform.SetParent(PoolLayer);
        effect.transform.localPosition = Vector3.zero;
        effect.gameObject.SetActive(false);
    }
}
