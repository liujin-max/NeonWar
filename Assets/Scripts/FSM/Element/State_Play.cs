using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//游戏进行中
public class State_Play<T> : State<Field>
{
    public State_Play(FSMSTATE id) : base(id){}

    public override void Enter(params object[] values)
    {
        Event_JoystickPress.OnEvent += OnJoyStickPress;

        Platform.Instance.UPDATETARGETFRAME(60);
    }

    public override void Update()
    {
        Field.Instance.CustomUpdate(Time.deltaTime);

        var result = Field.Instance.CheckResult();
        if (result != RESULT.NONE) {
            if (result == RESULT.UPGRADE) 
            {
                m_FSM.Transist(FSMSTATE.UPGRADE);
            }
            else
            {
                m_FSM.Transist(FSMSTATE.RESULT, result);
            }
        }
    }

    public override void Exit()
    {
        Platform.Instance.UPDATETARGETFRAME(30);

        // Field.Instance.ClearFieldWhenWaveFinished();

        Event_JoystickPress.OnEvent -= OnJoyStickPress;
    }


    #region 监听事件
    //按下、抬起摇杆
    private void OnJoyStickPress(Event_JoystickPress e)
    {
        Field.Instance.STATE   = GAME_STATE.PLAY;

        Field.Instance.Player.Move(e.Direction);
    }
    #endregion
}
