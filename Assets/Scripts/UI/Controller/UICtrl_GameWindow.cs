using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICtrl_GameWindow : UICtrl<UICtrl_GameWindow, GameWindow>
{
    protected override void RegisterHandlers()
    {
        Event_BattleStart.OnEvent   += OnBattleStart;
        Event_BattleEnd.OnEvent     += OnBattleEnd;
        Event_UpdateGlass.OnEvent   += OnUpdateGlass;
        Event_FightShow.OnEvent     += OnShowFight;
    }

    protected override void RemoveHandlers()
    {
        Event_BattleStart.OnEvent   -= OnBattleStart;
        Event_BattleEnd.OnEvent     -= OnBattleEnd;
        Event_UpdateGlass.OnEvent   -= OnUpdateGlass;
        Event_FightShow.OnEvent     -= OnShowFight;
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
    private void OnBattleStart(Event_BattleStart e)
    {
        m_Window?.OnBattleStart();
    }

    //战斗结束
    private void OnBattleEnd(Event_BattleEnd e)
    {
        m_Window?.OnBattleEnd();
    }


    private void OnUpdateGlass(Event_UpdateGlass e)
    {
        m_Window?.OnUpdateGlass();
    }

    private void OnShowFight(Event_FightShow e)
    {
        m_Window?.OnShowFight(e.Flag);
    }

    #endregion
}
