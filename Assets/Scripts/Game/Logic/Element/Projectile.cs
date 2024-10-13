using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //运动轨迹
    private Trace m_Trace;
    [HideInInspector] Unit Caster;

    private Action m_Callback;
    
    
    public void Init(TRACE trace_type, Unit caster, Action callback)
    {
        Caster  = caster;
        m_Trace = Trace.Create(trace_type, transform);

        m_Callback  = callback;
    }

    public void GO(Vector2 to_pos, float time)
    {
        m_Trace.GO(to_pos, time);
    }

    void Dispose()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        m_Trace.CustomUpdate(Time.deltaTime);

        if (!m_Trace.IsReach()) return;

        if (m_Callback != null) {
            m_Callback();
        }

        Dispose();
    }
}
