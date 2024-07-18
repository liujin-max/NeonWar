using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingWindow : MonoBehaviour
{
    [SerializeField] Button m_BtnClose;
    [SerializeField] Slider m_MusicSlider;
    [SerializeField] Slider m_SoundSlider;
    [SerializeField] Toggle m_VibrateToggle;

    [SerializeField] GameObject m_ButtonPivot;
    [SerializeField] Button m_BtnReturn;
    [SerializeField] Button m_BtnContinue;
    [SerializeField] Button m_BtnShare;

    private Action m_Callback;

    void Awake()
    {
        m_BtnClose.onClick.AddListener(()=>{
            if (m_Callback != null) m_Callback();
            
            GameFacade.Instance.UIManager.UnloadWindow(gameObject);
        });

        m_MusicSlider.onValueChanged.AddListener((float value)=>{
            GameFacade.Instance.SystemManager.MusicVolume = value;
        });

        m_SoundSlider.onValueChanged.AddListener((float value)=>{
            GameFacade.Instance.SystemManager.SoundVolume = value;
        });

        m_VibrateToggle.onValueChanged.AddListener((flag)=>{
            GameFacade.Instance.SystemManager.VibrateFlag = flag;
        });


        //继续游戏
        m_BtnContinue.onClick.AddListener(()=>{
            if (m_Callback != null) m_Callback();
            
            GameFacade.Instance.UIManager.UnloadWindow(gameObject);
        });
    }


    void OnDestroy()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        // Platform.Instance.INTER_VIDEOAD("adunit-eeebbaf0e72027e2");


        m_MusicSlider.value     = GameFacade.Instance.SystemManager.MusicVolume;
        m_SoundSlider.value     = GameFacade.Instance.SystemManager.SoundVolume;

        m_VibrateToggle.isOn    = GameFacade.Instance.SystemManager.VibrateFlag;
    }

    public void SetCallback(Action callback)
    {
        m_Callback = callback;
    }

    public void ShowButton(bool flag)
    {
        m_ButtonPivot.SetActive(flag);
        m_BtnShare.gameObject.SetActive(flag);
    }
}
