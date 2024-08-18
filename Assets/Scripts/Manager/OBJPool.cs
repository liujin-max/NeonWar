using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OBJPool<T> where T : MonoBehaviour
{
    private readonly List<OBJEntity<T>> m_Caches = new List<OBJEntity<T>>();
    private readonly Dictionary<T, OBJEntity<T>> m_KeyPairs = new Dictionary<T, OBJEntity<T>>();
    private readonly List<OBJEntity<T>> m_Removes = new List<OBJEntity<T>>();

    public bool Has(T value)
    {
        for (int i = 0; i < m_Caches.Count; i++)
        {
            var obj_entity = m_Caches[i];
            if (obj_entity.Entity.Equals(value))
                return true;
        }

        return false;
    }

    public T Get(GameObject template)
    {
        if (m_Caches.Count > 0)
        {
            T obj = m_Caches[0].Entity;
            m_Caches.RemoveAt(0); // 移除队列中的第一个对象
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

    public void Recyle(T value)
    {
        var obj_entity = m_KeyPairs[value];
        obj_entity.Reset();

        m_Caches.Add(obj_entity);
    }

    public void CustomUpdate(float dt)
    {
        m_Caches.ForEach(obj_entity => {
            obj_entity.CustomUpdate(dt);
            if (obj_entity.IsUnUsed()) m_Removes.Add(obj_entity);
        });

        m_Removes.ForEach(obj_entity => {
            m_KeyPairs.Remove(obj_entity.Entity);
            m_Caches.Remove(obj_entity);

            obj_entity.Dispose();
        });
        m_Removes.Clear();
    }
}
