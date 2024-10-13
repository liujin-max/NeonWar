using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICtrl_BattleWindow : UICtrl<UICtrl_BattleWindow, BattleWindow>
{
    protected override void RegisterHandlers()
    {
        Event_UpdateHP.OnEvent      += OnUpdateHP;
        Event_EnemyProgress.OnEvent += OnEnemyDead;
    }

    protected override void RemoveHandlers()
    {
        Event_UpdateHP.OnEvent      -= OnUpdateHP;
        Event_EnemyProgress.OnEvent -= OnEnemyDead;
    }

    protected override void OpenWindow(Action<BattleWindow> action = null)
    {
        GameFacade.Instance.UIManager.LoadWindowAsync(UI.BATTLEWINDOW, UIManager.BOTTOM, (obj)=>{
            m_Window = obj.GetComponent<BattleWindow>();
            m_Window.Init();
        });
    }


    #region 监听事件
    private void OnUpdateHP(Event_UpdateHP e)
    {
        m_Window?.OnUpdateHP();
    }

    // private void OnNextWave(GameEvent @event)
    // {
    //     m_Window?.OnNextWave();
    // }

    private void OnEnemyDead(Event_EnemyProgress e)
    {
        m_Window?.OnEnemyDead(e.Count);
    }

    #endregion
}
