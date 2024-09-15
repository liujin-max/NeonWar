using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICtrl_SkillTreeWindow : UICtrl<UICtrl_SkillTreeWindow, SkillTreeWindow>
{
    protected override void RegisterHandlers()
    {
        EventManager.AddHandler(EVENT.ONUPDATEGLASS,    OnUpdateGlass);
    }

    protected override void RemoveHandlers()
    {
        EventManager.DelHandler(EVENT.ONUPDATEGLASS,    OnUpdateGlass);
    }

    protected override void OpenWindow(Action<SkillTreeWindow> action)
    {
        GameFacade.Instance.UIManager.LoadWindowAsync(UI.SKILLTREEWINDOW, UIManager.MAJOR, (obj)=>{
            m_Window = obj.GetComponent<SkillTreeWindow>();
            m_Window.Init();
        });
    }


    #region 监听事件
    private void OnUpdateGlass(GameEvent @event)
    {
        m_Window?.FlushUI();
    }

    #endregion
}
