using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICtrl_GameWindow : UICtrl<UICtrl_GameWindow, GameWindow>
{
    protected override void RegisterHandlers()
    {
        EventManager.AddHandler(EVENT.ONBATTLESTART,    OnBattleStart);
        EventManager.AddHandler(EVENT.ONBATTLEEND,      OnBattleEnd);
        EventManager.AddHandler(EVENT.ONUPDATEGLASS,    OnUpdateGlass);

        EventManager.AddHandler(EVENT.UI_SHOWFIGHT,     OnShowFight);
    }

    protected override void RemoveHandlers()
    {
        EventManager.DelHandler(EVENT.ONBATTLESTART,    OnBattleStart);
        EventManager.DelHandler(EVENT.ONBATTLEEND,      OnBattleEnd);
        EventManager.DelHandler(EVENT.ONUPDATEGLASS,    OnUpdateGlass);

        EventManager.DelHandler(EVENT.UI_SHOWFIGHT,     OnShowFight);
    }


    protected override void OpenWindow(Action<GameWindow> action)
    {
        GameFacade.Instance.UIManager.LoadWindowAsync(UI.GAMEWINDOW, UIManager.BOTTOM, (obj)=>{
            m_Window = obj.GetComponent<GameWindow>();
            m_Window.Init();
        });
    }

    





    #region 监听事件
    //战斗开始
    private void OnBattleStart(GameEvent @event)
    {
        m_Window?.OnBattleStart();
    }

    //战斗结束
    private void OnBattleEnd(GameEvent @event)
    {
        m_Window?.OnBattleEnd();
    }


    private void OnUpdateGlass(GameEvent @event)
    {
        m_Window?.OnUpdateGlass();
    }

    private void OnShowFight(GameEvent @event)
    {
        m_Window?.OnShowFight((bool)@event.GetParam(0));
    }

    #endregion
}
