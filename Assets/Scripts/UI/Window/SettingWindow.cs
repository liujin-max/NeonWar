using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingWindow : BaseWindow
{
    [SerializeField] Button m_BtnClose;

    [SerializeField] Transform m_Music;
    [SerializeField] Transform m_Sound;


    private Action m_Callback;

    void Awake()
    {
        m_BtnClose.onClick.AddListener(()=>{
            if (m_Callback != null) m_Callback();
            
            UICtrl_SettingWindow.Instance.Exit();
        });

        m_Music.Find("Open").GetComponent<Button>().onClick.AddListener(()=>{
            GameFacade.Instance.SystemManager.MusicVolume = 0;

            FlushUI();
        });

        m_Music.Find("Close").GetComponent<Button>().onClick.AddListener(()=>{
            GameFacade.Instance.SystemManager.MusicVolume = 1;

            FlushUI();
        });

        //
        m_Sound.Find("Open").GetComponent<Button>().onClick.AddListener(()=>{
            GameFacade.Instance.SystemManager.SoundVolume = 0;

            FlushUI();
        });

        m_Sound.Find("Close").GetComponent<Button>().onClick.AddListener(()=>{
            GameFacade.Instance.SystemManager.SoundVolume = 1;

            FlushUI();
        });
    }

    void Start()
    {
        FlushUI();
    }

    void FlushUI()
    {
        m_Music.Find("Open").gameObject.SetActive(GameFacade.Instance.SystemManager.MusicVolume > 0);
        m_Music.Find("Close").gameObject.SetActive(GameFacade.Instance.SystemManager.MusicVolume == 0);

        //
        m_Sound.Find("Open").gameObject.SetActive(GameFacade.Instance.SystemManager.SoundVolume > 0);
        m_Sound.Find("Close").gameObject.SetActive(GameFacade.Instance.SystemManager.SoundVolume == 0);
    }

    public void SetCallback(Action callback)
    {
        m_Callback = callback;
    }

    public void ShowClose(bool flag)
    {
        m_BtnClose.gameObject.SetActive(flag);
    }
}
