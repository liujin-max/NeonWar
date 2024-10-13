using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICtrl_NavWindow : UICtrl<UICtrl_NavWindow, NavigationWindow>
{
    protected override void RegisterHandlers()
    {
        Event_BattleStart.OnEvent   += OnBattleStart;
        Event_BattleEnd.OnEvent     += OnBattleEnd;

        Event_BackpackOpen.OnEvent += OnBackpackOpen;
    }

    protected override void RemoveHandlers()
    {
        Event_BattleStart.OnEvent   -= OnBattleStart;
        Event_BattleEnd.OnEvent     -= OnBattleEnd;

        Event_BackpackOpen.OnEvent  -= OnBackpackOpen;
    }
    
    protected override void OpenWindow(Action<NavigationWindow> action)
    {
        GameFacade.Instance.UIManager.LoadWindowAsync(UI.NAVWINDOW, UIManager.NAV, (obj)=>{
            m_Window = obj.GetComponent<NavigationWindow>();
        });
    }


    #region 监听事件
    //战斗开始
    private void OnBattleStart(Event_BattleStart e)
    {
        m_Window?.OnBattleStart();
    }

    //战斗结束
    private void OnBattleEnd(Event_BattleEnd e)
    {
        m_Window?.OnBattleEnd();
    }

    //关闭背包
    private void OnBackpackOpen(Event_BackpackOpen e)
    {

    }
    #endregion
}
