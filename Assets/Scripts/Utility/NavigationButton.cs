using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NavigationButton : MonoBehaviour
{
    public Button m_Button;
    public GameObject m_Light;



    private Func<bool> m_ExecuteCallback;
    private Func<bool> m_RevokeCallback;


    void Start()
    {
        if (m_Light != null) m_Light.SetActive(false);
    }

    public void Init(Func<bool> execute, Func<bool> revoke, UnityAction click_action)
    {
        m_ExecuteCallback   = execute;
        m_RevokeCallback    = revoke;

        m_Button.onClick.RemoveAllListeners();
        m_Button.onClick.AddListener(click_action);
    }


    //执行
    public void Execute()
    {
        if (m_ExecuteCallback())
        {
            if (m_Light != null) m_Light.SetActive(true);
        }
    }

    //撤销
    public void Revoke()
    {
        if (m_RevokeCallback())
        {
            if (m_Light != null) m_Light.SetActive(false);
        }
    }
}
