using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICtrl_NavWindow : UICtrl<UICtrl_NavWindow, NavigationWindow>
{
    protected override void RegisterHandlers()
    {
        EventManager.AddHandler(EVENT.ONBATTLESTART,    OnBattleStart);
        EventManager.AddHandler(EVENT.ONBATTLEEND,      OnBattleEnd);

        EventManager.AddHandler(EVENT.UI_BACKPACKOPEN,  OnBackpackOpen);
    }

    protected override void RemoveHandlers()
    {
        EventManager.DelHandler(EVENT.ONBATTLESTART,    OnBattleStart);
        EventManager.DelHandler(EVENT.ONBATTLEEND,      OnBattleEnd);

        EventManager.DelHandler(EVENT.UI_BACKPACKOPEN,  OnBackpackOpen);
    }
    
    protected override void OpenWindow(Action<NavigationWindow> action)
    {
        GameFacade.Instance.UIManager.LoadWindowAsync(UI.NAVWINDOW, UIManager.NAV, (obj)=>{
            m_Window = obj.GetComponent<NavigationWindow>();
        });
    }


    #region 监听事件
    //战斗开始
    private void OnBattleStart(GameEvent @event)
    {
        m_Window?.OnBattleStart();
    }

    //战斗结束
    private void OnBattleEnd(GameEvent @event)
    {
        m_Window?.OnBattleEnd();
    }

    //关闭背包
    private void OnBackpackOpen(GameEvent @event)
    {
        bool flag = (bool)@event.GetParam(0);

        // m_Window?.OnBackpackOpen(flag);
    }
    #endregion
}
