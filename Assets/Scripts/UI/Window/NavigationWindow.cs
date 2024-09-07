using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class NavigationWindow : MonoBehaviour
{
    [SerializeField] Transform m_NavPivot;
    [SerializeField] TextMeshProUGUI m_Level;

    [Header("按钮")]
    [SerializeField] NavigationButton m_BtnBackpack;
    [SerializeField] NavigationButton m_BtnLeague;
    [SerializeField] NavigationButton m_BtnFight;
    [SerializeField] NavigationButton m_BtnSetting;


    private NavigationButton m_NAVBTN;

    

    void Awake()
    {
        EventManager.AddHandler(EVENT.ONBATTLESTART,    OnBattleStart);
        EventManager.AddHandler(EVENT.ONBATTLEEND,      OnBattleEnd);

        EventManager.AddHandler(EVENT.UI_BACKPACKOPEN,  OnBackpackOpen);


        #region 宝珠
        m_BtnBackpack.Init(
            ()=>{
                return NavigationController.GotoBackpack(null);
            },
            ()=>{
                GameFacade.Instance.UIManager.UnloadWindow("BackpackWindow");
            },
            ()=>{
                if (m_NAVBTN == m_BtnBackpack) return;
                if (m_BtnBackpack.Execute())
                {
                    m_NAVBTN.Revoke();
                    m_NAVBTN = m_BtnBackpack; 
                }
            }
        );
        #endregion



        #region 武器
        m_BtnLeague.Init(
            ()=>{
                return NavigationController.GotoWeapon();
            },
            ()=>{
                GameFacade.Instance.UIManager.UnloadWindow("WeaponWindow");
            },
            ()=>{
                if (m_NAVBTN == m_BtnLeague) return;
                if (m_BtnLeague.Execute())
                {
                    m_NAVBTN.Revoke();
                    m_NAVBTN = m_BtnLeague; 
                }
            }
        );
        #endregion



        #region 关卡
        m_BtnFight.Init(
            ()=>{
                return true;
            },
            ()=>{

            },
            ()=>{
                if (m_NAVBTN == m_BtnFight) return;
                if (m_BtnFight.Execute())
                {
                    m_NAVBTN.Revoke();
                    m_NAVBTN = m_BtnFight; 
                }
            }
        );
        #endregion



        #region 设置
        m_BtnSetting.Init(
            ()=>{
                GameFacade.Instance.UIManager.LoadWindowAsync("SettingWindow", UIManager.BOTTOM, (obj)=>{
                    var window = obj.GetComponent<SettingWindow>();
                    window.ShowClose(false);
                    window.SetCallback(()=>{

                    });
                });

                return true;
            },
            ()=>{
                GameFacade.Instance.UIManager.UnloadWindow("SettingWindow");
            },
            ()=>{
                if (m_NAVBTN == m_BtnSetting) return;
                if (m_BtnSetting.Execute())
                {
                    m_NAVBTN.Revoke();
                    m_NAVBTN = m_BtnSetting; 
                }
            }
        );
        #endregion


    }


    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.ONBATTLESTART,    OnBattleStart);
        EventManager.DelHandler(EVENT.ONBATTLEEND,      OnBattleEnd);

        EventManager.DelHandler(EVENT.UI_BACKPACKOPEN,  OnBackpackOpen);
    }


    void Start()
    {
        m_NAVBTN = m_BtnFight;
        m_NAVBTN.Execute(); 

        FlushUI();
    }

    void FlushUI()
    {
        m_Level.text    = (DataCenter.Instance.User.Level + 1).ToString();
    }


    #region 监听事件
    //战斗开始
    private void OnBattleStart(GameEvent @event)
    {
        m_NavPivot.DOLocalMoveY(-350, 0.5f).SetEase(Ease.OutCubic);
    }

    //战斗结束
    private void OnBattleEnd(GameEvent @event)
    {
        m_NavPivot.DOLocalMoveY(0, 0.5f).SetEase(Ease.InCubic);

        FlushUI();
    }

    //关闭背包
    private void OnBackpackOpen(GameEvent @event)
    {
        bool flag = (bool)@event.GetParam(0);

        if (flag == false)
        {
            if (m_NAVBTN == m_BtnBackpack)
            {
                m_BtnFight.Execute();
                
                m_NAVBTN.Revoke();
                m_NAVBTN = m_BtnFight; 
            }
        }
    }
    #endregion
}
