using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//游戏进行中
public class State_Play<T> : State<Field>
{
    public State_Play(CONST.FSMSTATE id) : base(id){}

    public override void Enter(params object[] values)
    {
        EventManager.AddHandler(EVENT.ONJOYSTICK_PRESS,     OnJoyStickPress);
        EventManager.AddHandler(EVENT.ONJOYSTICK_DOUBLE,    OnJoyStickDouble);

        Platform.Instance.UPDATETARGETFRAME(60);
    
    }

    public override void Update()
    {
        Field.Instance.CustomUpdate(Time.deltaTime);

        var result = Field.Instance.CheckResult();
        if (result != CONST.RESULT.NONE) {
            if (result == CONST.RESULT.UPGRADE) 
            {
                m_FSM.Transist(CONST.FSMSTATE.UPGRADE);
            }
            else
            {
                m_FSM.Transist(CONST.FSMSTATE.RESULT, result);
            }
        }
    }

    public override void Exit()
    {
        Platform.Instance.UPDATETARGETFRAME(30);

        Field.Instance.ClearFieldWhenWaveFinished();

        EventManager.DelHandler(EVENT.ONJOYSTICK_PRESS,     OnJoyStickPress);
        EventManager.DelHandler(EVENT.ONJOYSTICK_DOUBLE,    OnJoyStickDouble);
    }


    #region 监听事件
    //按下、抬起摇杆
    private void OnJoyStickPress(GameEvent @event)
    {
        float direction = (float)@event.GetParam(0);

        Field.Instance.STATE   = CONST.GAME_STATE.PLAY;

        Field.Instance.Player.Move(direction);
    }

    private void OnJoyStickDouble(GameEvent @event)
    {
        // Field.Instance.Player.Blink();
    }
    #endregion
}
