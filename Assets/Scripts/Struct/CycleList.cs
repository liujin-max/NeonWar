using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleList<T> where T : IDisposable, ICustomUpdate, IFinished
{
    public List<T> List {get {return m_List;}}
    private List<T> m_List = new List<T>();
    private List<T> m_Removes = new List<T>();


    private Action<T> m_RemoveCallback;

    public CycleList(Action<T> remove_callback = null)
    {
        m_RemoveCallback = remove_callback;
    }

    public void Add(T t)
    {
        m_List.Add(t);
    }

    public void CustomUpdate(float deltaTime)
    {

        foreach (var a in m_List) {
            a.CustomUpdate(deltaTime);

            if (a.IsFinished()) m_Removes.Add(a);
        }

        if (m_Removes.Count > 0) {
            foreach (var a in m_Removes) {
                a.Dispose();
                m_List.Remove(a);

                if (m_RemoveCallback != null) m_RemoveCallback(a);
            }
            m_Removes.Clear();
        }
    }


    public void Clear()
    {
        m_List.ForEach(a => a.Dispose());
        m_List.Clear();
    }

    public void Dispose()
    {
        Clear();
    }


}
