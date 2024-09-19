using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICtrl_BackpackWindow : UICtrl<UICtrl_BackpackWindow, BackpackWindow>
{
    protected override void RegisterHandlers()
    {
        EventManager.AddHandler(EVENT.UI_PEARCOMPOSE,   OnPearComposeStart);
        EventManager.AddHandler(EVENT.UI_COMPOSECHANGE, OnComposeChange);
        EventManager.AddHandler(EVENT.UI_SELECTPEAR,    OnSelectPear);
        
    }

    protected override void RemoveHandlers()
    {
        EventManager.DelHandler(EVENT.UI_PEARCOMPOSE,   OnPearComposeStart);
        EventManager.DelHandler(EVENT.UI_COMPOSECHANGE, OnComposeChange);
        EventManager.DelHandler(EVENT.UI_SELECTPEAR,    OnSelectPear);
    }


    protected override void OpenWindow(Action<BackpackWindow> action = null)
    {
        GameFacade.Instance.UIManager.LoadWindowAsync(UI.BACKPACKWINDOW, UIManager.MAJOR, (obj)=>{
            m_Window = obj.GetComponent<BackpackWindow>();
            m_Window.Init(null);
        });
    }

    protected override void CloseWindow()
    {
        m_Window?.OnExit();
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

    void OnSelectPear(GameEvent @event)
    {
        Pear pear = @event.GetParam(0) as Pear;

        m_Window?.ShowDetail(pear);
    }

    #endregion
}
