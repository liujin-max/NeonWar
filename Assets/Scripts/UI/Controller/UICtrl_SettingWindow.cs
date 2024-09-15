using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICtrl_SettingWindow : UICtrl<UICtrl_SettingWindow, SettingWindow>
{
    protected override void RegisterHandlers()
    {
        
    }

    protected override void RemoveHandlers()
    {
        
    }

    protected override void OpenWindow(Action<SettingWindow> action = null)
    {
        GameFacade.Instance.UIManager.LoadWindowAsync(UI.SETTINGWINDOW, UIManager.MAJOR, (obj)=>{
            m_Window = obj.GetComponent<SettingWindow>();
            if (action != null) action(m_Window);
        });
    }
}
