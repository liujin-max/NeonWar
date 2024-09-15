using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICtrl_BackpackWindow : UICtrl<UICtrl_BackpackWindow, BackpackWindow>
{
    protected override void AddHandlers()
    {
        EventManager.AddHandler(EVENT.UI_PEARCOMPOSE,   OnPearComposeStart);
        EventManager.AddHandler(EVENT.UI_COMPOSECHANGE, OnComposeChange);
    }

    protected override void DelHandlers()
    {
        EventManager.DelHandler(EVENT.UI_PEARCOMPOSE,   OnPearComposeStart);
        EventManager.DelHandler(EVENT.UI_COMPOSECHANGE, OnComposeChange);
    }

    protected override void OpenWindow(Action<BackpackWindow> action = null)
    {
        GameFacade.Instance.UIManager.LoadWindowAsync(UI.BACKPACKWINDOW, UIManager.BOARD, (obj)=>{
            m_Window = obj.GetComponent<BackpackWindow>();
            m_Window.Init(null);
        });
    }



    #region 监听事件
    void OnPearComposeStart(GameEvent @event)
    {
        Pear pear = @event.GetParam(0) as Pear;

        m_Window?.OnPearComposeStart(pear);
    }

    void OnComposeChange(GameEvent @event)
    {
        bool flag = (bool)@event.GetParam(0);
        Pear pear = @event.GetParam(1) as Pear;

        m_Window?.OnComposeChange(flag, pear);
    }
    #endregion
}
