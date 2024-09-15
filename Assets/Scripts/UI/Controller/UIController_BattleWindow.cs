using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController_BattleWindow : UIController<UIController_BattleWindow, BattleWindow>
{
    protected override void AddHandlers()
    {
        EventManager.AddHandler(EVENT.ONHPUPDATE,       OnUpdateHP);
        EventManager.AddHandler(EVENT.ONNEXTWAVE,       OnNextWave);
        
        EventManager.AddHandler(EVENT.UI_ENEMYDEAD,     OnEnemyDead);
    }

    protected override void DelHandlers()
    {
        EventManager.DelHandler(EVENT.ONHPUPDATE,       OnUpdateHP);
        EventManager.DelHandler(EVENT.ONNEXTWAVE,       OnNextWave);

        EventManager.DelHandler(EVENT.UI_ENEMYDEAD,     OnEnemyDead);
    }

    protected override void OpenWindow(Action<BattleWindow> action = null)
    {
        GameFacade.Instance.UIManager.LoadWindowAsync(UI.BATTLEWINDOW, UIManager.BOTTOM, (obj)=>{
            m_Window = obj.GetComponent<BattleWindow>();
            m_Window.Init();
        });
    }


    #region 监听事件
    private void OnUpdateHP(GameEvent @event)
    {
        m_Window?.OnUpdateHP();
    }

    private void OnNextWave(GameEvent @event)
    {
        m_Window?.OnNextWave();
    }

    private void OnEnemyDead(GameEvent @event)
    {
        int count = (int)@event.GetParam(0);

        m_Window?.OnEnemyDead(count);
    }

    #endregion
}
