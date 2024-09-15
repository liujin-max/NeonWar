using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController_GameWindow : UIController<UIController_GameWindow, GameWindow>
{
    protected override void OpenWindow()
    {
        GameFacade.Instance.UIManager.LoadWindowAsync(UI.GAMEWINDOW, UIManager.BOTTOM, (obj)=>{
            m_Window = obj.GetComponent<GameWindow>();
            m_Window.Init();
        });
    }

    protected override void AddHandlers()
    {
        EventManager.AddHandler(EVENT.ONBATTLESTART,    OnBattleStart);
        EventManager.AddHandler(EVENT.ONBATTLEEND,      OnBattleEnd);
        EventManager.AddHandler(EVENT.ONUPDATEGLASS,    OnUpdateGlass);
    }

    protected override void DelHandlers()
    {
        EventManager.DelHandler(EVENT.ONBATTLESTART,    OnBattleStart);
        EventManager.DelHandler(EVENT.ONBATTLEEND,      OnBattleEnd);
        EventManager.DelHandler(EVENT.ONUPDATEGLASS,    OnUpdateGlass);
    }





    #region 监听事件
    //战斗开始
    private void OnBattleStart(GameEvent @event)
    {
        if (m_Window == null) return;

        m_Window.OnBattleStart();
    }

    //战斗结束
    private void OnBattleEnd(GameEvent @event)
    {
        if (m_Window == null) return;

        m_Window.OnBattleEnd();
    }


    private void OnUpdateGlass(GameEvent @event)
    {
        if (m_Window == null) return;

        m_Window.OnUpdateGlass();
    }


    #endregion
}
