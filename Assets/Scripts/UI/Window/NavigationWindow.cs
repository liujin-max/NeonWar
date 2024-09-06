using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationWindow : MonoBehaviour
{
    [SerializeField] NavigationButton m_BtnBackpack;
    [SerializeField] NavigationButton m_BtnLeague;
    [SerializeField] NavigationButton m_BtnBattle;
    [SerializeField] NavigationButton m_BtnSetting;


    private NavigationButton m_NAVBTN;

    

    void Awake()
    {
        //背包
        m_BtnBackpack.Init(
            ()=>{
                return NavigationController.GotoBackpack(null);
            },
            ()=>{
                GameFacade.Instance.UIManager.UnloadWindow("BackpackWindow");
                return true;
            },
            ()=>{
                if (m_NAVBTN != null) m_NAVBTN.Revoke();
                
                m_NAVBTN = m_BtnBackpack;
                m_NAVBTN.Execute();
            }
        );

        //武器
        m_BtnLeague.Init(
            ()=>{
                return false;
            },
            ()=>{
                return false;
            },
            ()=>{
                if (m_NAVBTN != null) m_NAVBTN.Revoke();
                
                m_NAVBTN = m_BtnLeague;
                m_NAVBTN.Execute();
            }
        );

        //设置
        m_BtnSetting.Init(
            ()=>{
                Time.timeScale = 0;
                GameFacade.Instance.UIManager.LoadWindowAsync("SettingWindow", UIManager.BOARD);

                return true;
            },
            ()=>{
                Time.timeScale = 1;
                GameFacade.Instance.UIManager.UnloadWindow("SettingWindow");
                return true;
            },
            ()=>{
                if (m_NAVBTN != null) m_NAVBTN.Revoke();
                
                m_NAVBTN = m_BtnSetting;
                m_NAVBTN.Execute();
            }
        );

    }
}
