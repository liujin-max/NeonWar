using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class NavigationWindow : BaseWindow
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
        #region 道具
        m_BtnBackpack.Init(
            ()=>{
                return NavigationController.GotoBackpack();
            },
            ()=>{
                UICtrl_BackpackWindow.Instance.Exit();
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
                return NavigationController.GotoSkillTree();
            },
            ()=>{
                UICtrl_SkillTreeWindow.Instance.Exit();
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
                EventManager.SendEvent(new GameEvent(EVENT.UI_SHOWFIGHT, true));
                return true;
            },
            ()=>{
                EventManager.SendEvent(new GameEvent(EVENT.UI_SHOWFIGHT, false));
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
                UICtrl_SettingWindow.Instance.Enter((window)=>{
                    window.ShowClose(false);
                    window.ShowMask(false);
                });

                return true;
            },
            ()=>{
                UICtrl_SettingWindow.Instance.Exit();
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



    //战斗开始
    public void OnBattleStart()
    {
        m_NavPivot.DOLocalMoveY(-350, 0.5f).SetEase(Ease.OutCubic);
    }

    //战斗结束
    public void OnBattleEnd()
    {
        m_NavPivot.DOLocalMoveY(0, 0.5f).SetEase(Ease.InCubic);

        FlushUI();
    }
}
