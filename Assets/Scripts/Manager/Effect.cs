using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public float m_Time = 0;
    private float m_TimeMax = 0;
    public bool m_IsLoop = false;
    public bool m_IsRecycle = true;
    [HideInInspector]public string ResPath;

    private Action m_Callback;

    void Awake()
    {
        m_TimeMax = m_Time;
    }

    public void SetCallback(Action action)
    {
        m_Callback = action;
    }

    public void Restart()
    {
        m_Time = m_TimeMax;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsLoop) return ;
        
        m_Time -= Time.deltaTime;

        if (m_Time <= 0)
        {
            if (m_Callback != null) {
                m_Callback.Invoke();
            }

            if (string.IsNullOrEmpty(ResPath) || !m_IsRecycle) {
                Destroy(gameObject);
            } else {
                GameFacade.Instance.PoolManager.RecycleEffect(this);
            }
        }
    }
}
