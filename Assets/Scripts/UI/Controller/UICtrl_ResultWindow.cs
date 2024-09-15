using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICtrl_ResultWindow : UICtrl<UICtrl_ResultWindow, ResultWindow>
{
    protected override void RegisterHandlers()
    {
        
    }

    protected override void RemoveHandlers()
    {
        
    }

    protected override void OpenWindow(Action<ResultWindow> action = null)
    {
        GameFacade.Instance.UIManager.LoadWindowAsync(UI.RESULTWINDOW, UIManager.BOARD, (obj)=>{
            m_Window = obj.GetComponent<ResultWindow>();
            if (action != null) action(m_Window);
        });
    }
}
