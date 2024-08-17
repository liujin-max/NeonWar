using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OBJManager: MonoBehaviour
{
    private Transform PoolLayer;

    private Dictionary<string, List<Bullet>> m_BulletPool = new Dictionary<string, List<Bullet>>();

    //特效池
    private Dictionary<string, List<GameObject>> m_EffectPool = new Dictionary<string, List<GameObject>>();
    private int m_EffectMax = 3;    //同种特效最多存3个


    void Awake()
    {
        PoolLayer = GameObject.Find("POOL").transform;
    }



    //加载子弹
    public Bullet AllocateBullet(GameObject bullet_template, Vector3 pos)
    {
        string name     = bullet_template.name;
        Bullet bullet   = null;

        if (m_BulletPool.ContainsKey(name)) {
            List<Bullet> bullets = m_BulletPool[name];

            if (bullets.Count > 0) {
                bullet  = bullets.First();
                bullets.Remove(bullet);
            }
        }

        if (bullet == null)
        {
            bullet      = Instantiate(bullet_template, pos, Quaternion.identity).GetComponent<Bullet>();
            bullet.Name = name;
        }

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

        if (!m_BulletPool.ContainsKey(name)) {
            m_BulletPool[name] = new List<Bullet>();
        }

        if (m_BulletPool[name].Contains(bullet)) return;

        m_BulletPool[name].Add(bullet);

        bullet.transform.SetParent(PoolLayer);
        bullet.gameObject.SetActive(false);
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
            effect = Instantiate(AssetsManager.LoadPrefab("Effect", effect_path), pos, Quaternion.identity);
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
