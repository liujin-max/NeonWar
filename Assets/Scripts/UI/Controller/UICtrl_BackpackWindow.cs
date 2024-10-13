using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICtrl_BackpackWindow : UICtrl<UICtrl_BackpackWindow, BackpackWindow>
{
    protected override void RegisterHandlers()
    {
        Event_PearCompose.OnEvent       += OnPearComposeStart;
        Event_PearComposeChange.OnEvent += OnComposeChange;
        Event_SelectPear.OnEvent        += OnSelectPear;
    }

    protected override void RemoveHandlers()
    {
        Event_PearCompose.OnEvent       -= OnPearComposeStart;
        Event_PearComposeChange.OnEvent -= OnComposeChange;
        Event_SelectPear.OnEvent        -= OnSelectPear;

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
    void OnPearComposeStart(Event_PearCompose e)
    {
        m_Window?.OnPearComposeStart(e.Pear);
    }

    void OnComposeChange(Event_PearComposeChange e)
    {
        m_Window?.OnComposeChange(e.IsPush, e.Pear);
    }

    void OnSelectPear(Event_SelectPear e)
    {
        m_Window?.ShowDetail(e.Pear);
    }

    #endregion
}
