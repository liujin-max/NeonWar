using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICtrl_SkillWindow : UICtrl<UICtrl_SkillWindow, SkillWindow>
{
    protected override void AddHandlers()
    {
        EventManager.AddHandler(EVENT.UI_SKILLUPGRADE,  OnSkillUpgrade);
    }

    protected override void DelHandlers()
    {
        EventManager.DelHandler(EVENT.UI_SKILLUPGRADE,  OnSkillUpgrade);
    }

    protected override void OpenWindow(Action<SkillWindow> action)
    {
        GameFacade.Instance.UIManager.LoadWindowAsync(UI.SKILLWINDOW, UIManager.BOARD, (obj)=>{
            m_Window = obj.GetComponent<SkillWindow>();
            if (action != null) action(m_Window);
        });
    }


    #region 监听事件
    private void OnSkillUpgrade(GameEvent @event)
    {
        m_Window?.OnSkillUpgrade();
    }
    #endregion
}
