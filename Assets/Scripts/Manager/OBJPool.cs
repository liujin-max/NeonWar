using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBJPool<T>
{
    private readonly List<T> m_Caches = new List<T>();


    private GameObject m_Template = default;
    private string m_ResPath = default;


    // 构造函数接受一个模板对象
    public OBJPool(GameObject template)
    {
        m_Template = template;
    }

    public OBJPool(string res_path)
    {
        m_ResPath = res_path;
    }


    public void Add(T value)
    {
        m_Caches.Add(value);
    }

    public T Get()
    {
        if (m_Caches.Count > 0)
        {
            T item = m_Caches[0];
            m_Caches.RemoveAt(0); // 移除队列中的第一个对象
            return item;
        }
        else
        {
            // 如果缓存池没有对象，则创建一个新的对象
            // return GameObject.Instantiate(m_Template);
        }
    }

    public void Remove(T value)
    {
        m_Caches.Remove(value);
    }
}
