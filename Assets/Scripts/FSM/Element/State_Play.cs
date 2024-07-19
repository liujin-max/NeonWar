using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//游戏进行中
public class State_Play<T> : State<Field>
{
    public State_Play(_C.FSMSTATE id) : base(id){}

    public override void Enter(params object[] values)
    {
        Debug.Log("进入 游玩阶段");

        EventManager.AddHandler(EVENT.ONJOYSTICK_PRESS,     OnJoyStickPress);
        EventManager.AddHandler(EVENT.ONJOYSTICK_DOUBLE,    OnJoyStickDouble);

        var game_window = GameFacade.Instance.UIManager.GetWindow("GameWindow").GetComponent<GameWindow>();
        game_window.ShowJoyStick(true);

        Field.Instance.Resume();
    }

    public override void Update()
    {
        var result = Field.Instance.CheckResult();
        if (result != _C.RESULT.NONE) {
            m_FSM.Transist(_C.FSMSTATE.RESULT, result);
        }
    }

    public override void Exit()
    {
        Debug.Log("离开 游玩阶段");

        var game_window = GameFacade.Instance.UIManager.GetWindow("GameWindow").GetComponent<GameWindow>();
        game_window.ShowJoyStick(false);

        Field.Instance.Pause();
        Field.Instance.Spawn.Dispose();

        EventManager.DelHandler(EVENT.ONJOYSTICK_PRESS,     OnJoyStickPress);
        EventManager.DelHandler(EVENT.ONJOYSTICK_DOUBLE,    OnJoyStickDouble);
    }


    #region 监听事件
    //按下、抬起摇杆
    private void OnJoyStickPress(GameEvent @event)
    {
        float direction = (float)@event.GetParam(0);

        Field.Instance.Player.Move(direction);

    }

    private void OnJoyStickDouble(GameEvent @event)
    {
        Field.Instance.Player.Blink();
    }
    #endregion
}
