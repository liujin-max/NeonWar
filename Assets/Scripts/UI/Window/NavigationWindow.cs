using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class NavigationWindow : MonoBehaviour
{
    [SerializeField] Transform m_NavPivot;

    [Header("按钮")]
    [SerializeField] NavigationButton m_BtnBackpack;
    [SerializeField] NavigationButton m_BtnLeague;
    [SerializeField] NavigationButton m_BtnBattle;
    [SerializeField] NavigationButton m_BtnSetting;


    private NavigationButton m_NAVBTN;

    

    void Awake()
    {
        EventManager.AddHandler(EVENT.ONBATTLESTART,    OnBattleStart);
        EventManager.AddHandler(EVENT.ONBATTLEEND,      OnBattleEnd);


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
                GameFacade.Instance.UIManager.LoadWindowAsync("SettingWindow", UIManager.BOARD, (obj)=>{
                    obj.GetComponent<SettingWindow>().SetCallback(()=>{
                        Time.timeScale = 1;
                    });
                });

                return true;
            },
            ()=>{
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


    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.ONBATTLESTART,    OnBattleStart);
        EventManager.DelHandler(EVENT.ONBATTLEEND,      OnBattleEnd);
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
    }
    #endregion
}
