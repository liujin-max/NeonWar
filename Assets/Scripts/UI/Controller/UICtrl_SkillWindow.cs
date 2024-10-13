using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICtrl_SkillWindow : UICtrl<UICtrl_SkillWindow, SkillWindow>
{
    protected override void RegisterHandlers()
    {
        Event_SkillUpgrade.OnEvent += OnSkillUpgrade;
    }

    protected override void RemoveHandlers()
    {
        Event_SkillUpgrade.OnEvent -= OnSkillUpgrade;
    }

    protected override void OpenWindow(Action<SkillWindow> action)
    {
        GameFacade.Instance.UIManager.LoadWindowAsync(UI.SKILLWINDOW, UIManager.BOARD, (obj)=>{
            m_Window = obj.GetComponent<SkillWindow>();
            if (action != null) action(m_Window);
        });
    }


    #region 监听事件
    private void OnSkillUpgrade(Event_SkillUpgrade e)
    {
        m_Window?.OnSkillUpgrade();
    }
    #endregion
}
