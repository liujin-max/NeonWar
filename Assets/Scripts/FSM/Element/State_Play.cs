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
        EventManager.AddHandler(EVENT.ONJOYSTICK_PRESS,     OnJoyStickPress);
        EventManager.AddHandler(EVENT.ONJOYSTICK_DOUBLE,    OnJoyStickDouble);

        AssetsManager.Recyle(); //卸载无用的AB包
        Platform.Instance.UPDATETARGETFRAME(60);
        
        int level_id = (int)values[0];
        Field.Instance.Play(level_id);
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
        if (!Field.Instance.BlinkTimer.IsFinished()) {
            EventManager.SendEvent(new GameEvent(EVENT.UI_BLINKSHAKE));
            return;
        }

        Field.Instance.BlinkTimer.ForceReset();
        Field.Instance.Player.Blink();
    }
    #endregion
}
