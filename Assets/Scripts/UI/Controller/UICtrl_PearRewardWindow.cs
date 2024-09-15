using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICtrl_PearRewardWindow : UICtrl<UICtrl_PearRewardWindow, PearRewardWindow>
{
    protected override void RegisterHandlers()
    {

    }

    protected override void RemoveHandlers()
    {

    }
    
    protected override void OpenWindow(Action<PearRewardWindow> action)
    {
        GameFacade.Instance.UIManager.LoadWindowAsync(UI.PEARREWARDWINDOW, UIManager.BOARD, (obj)=>{
            m_Window = obj.GetComponent<PearRewardWindow>();
            if (action != null) action(m_Window);
        });
    }


    #region 监听事件

    #endregion
}
