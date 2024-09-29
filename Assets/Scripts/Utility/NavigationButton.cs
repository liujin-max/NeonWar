using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NavigationButton : MonoBehaviour
{
    public Button m_Button;
    public GameObject m_UnlockPivot;
    public GameObject m_LockPivot;
    public GameObject m_Light;
    public IconAnim m_Anim;



    private Func<bool> m_ExecuteCallback;
    private Action m_RevokeCallback;
    private Func<bool> m_UnlockCallback;


    void Awake()
    {
        ShowLight(false);
    }

    public void Init(Func<bool> execute, Action revoke, UnityAction click_action, Func<bool> unlock_action)
    {
        m_ExecuteCallback   = execute;
        m_RevokeCallback    = revoke;
        m_UnlockCallback    = unlock_action;

        m_Button.onClick.RemoveAllListeners();
        m_Button.onClick.AddListener(click_action);

        FlushLock();
    }

    public void FlushLock()
    {
        bool flag = m_UnlockCallback();
        m_UnlockPivot.SetActive(flag);
        m_LockPivot.SetActive(!flag);
    }

    void ShowLight(bool flag)
    {
        if (m_Light != null) 
        {
            m_Light.SetActive(flag);
        }
    }

    void Play(bool flag)
    {
        if (m_Anim == null) return;

        if (flag) m_Anim.Play();
        else m_Anim.Stop();
    }

    //执行
    public bool Execute()
    {
        if (m_ExecuteCallback())
        {
            Play(true);
            ShowLight(true);
            return true;
        }
        return false;
    }

    //撤销
    public void Revoke()
    {
        m_RevokeCallback();

        Play(false);
        ShowLight(false);
    }
}
