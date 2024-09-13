using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OBJPool<T> where T : MonoBehaviour
{
    private int m_NumberMax = -1;    //最多存#个

    //池子中可用的对象
    private List<OBJEntity<T>> m_Caches = new List<OBJEntity<T>>();
    private HashSet<T> m_CacheDic = new HashSet<T>();

    //创建出的对象，把该对象和对应的OBJEntity做成映射关系并记录下来
    private Dictionary<T, OBJEntity<T>> m_KeyPairs = new Dictionary<T, OBJEntity<T>>();

    private List<OBJEntity<T>> m_Removes = new List<OBJEntity<T>>();

    public OBJPool(int value = -1)
    {
        m_NumberMax = value;
    }

    public bool Has(T value)
    {
        return m_CacheDic.Contains(value);
    }

    public T Get(GameObject template)
    {
        if (m_Caches.Count > 0)
        {
            T obj = m_Caches[m_Caches.Count - 1].Entity;
            m_Caches.RemoveAt(m_Caches.Count - 1); // 移除队列中的第一个对象
            m_CacheDic.Remove(obj);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            // 如果缓存池没有对象，则创建一个新的对象
            OBJEntity<T> e = new OBJEntity<T>(GameObject.Instantiate(template).GetComponent<T>());
            m_KeyPairs[e.Entity] = e;
            return e.Entity;
        }
    }

    public T Get(string res_path)
    {
        if (m_Caches.Count > 0)
        {
            T obj = m_Caches[m_Caches.Count - 1].Entity;
            m_Caches.RemoveAt(m_Caches.Count - 1); // 移除队列中的第一个对象
            m_CacheDic.Remove(obj);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            // 如果缓存池没有对象，则创建一个新的对象
            OBJEntity<T> e = new OBJEntity<T>(GameObject.Instantiate(Resources.Load<GameObject>(res_path)).GetComponent<T>());
            m_KeyPairs[e.Entity] = e;
            return e.Entity;
        }
    }

    public void Recyle(T value)
    {
        var obj_entity = m_KeyPairs[value];

        if (m_NumberMax != -1 && m_Caches.Count >= m_NumberMax)
        {
            m_KeyPairs.Remove(value);
            obj_entity.Dispose();
            return;
        }

        value.transform.SetParent(GameFacade.Instance.PoolManager.PoolLayer);
        value.transform.localPosition = Vector3.zero;
        value.gameObject.SetActive(false);

        obj_entity.Reset();

        m_Caches.Add(obj_entity);
        m_CacheDic.Add(value);
    }

    public void CustomUpdate(float dt)
    {
        foreach (var obj_entity in m_Caches) {
            obj_entity.CustomUpdate(dt);
            if (obj_entity.IsUnUsed()) m_Removes.Add(obj_entity);
        }

        foreach (var obj_entity in m_Removes) {
            m_KeyPairs.Remove(obj_entity.Entity);
            m_Caches.Remove(obj_entity);
            m_CacheDic.Remove(obj_entity.Entity);

            obj_entity.Dispose();
        }
        m_Removes.Clear();
    }
}
