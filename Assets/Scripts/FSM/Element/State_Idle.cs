using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//可以进行局外系统的操作
public class State_Idle<T> : State<Field>
{
    public State_Idle(_C.FSMSTATE id) : base(id){}

    public override void Enter(params object[] values)
    {
        EventManager.AddHandler(EVENT.ONJOYSTICK_PRESS,     OnJoyStickPress);

        EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_SHOW, true));

        Field.Instance.RemovePlayer();
        Field.Instance.InitPlayer();
    }

    public override void Update()
    {

    }

    public override void Exit()
    {
        EventManager.DelHandler(EVENT.ONJOYSTICK_PRESS,     OnJoyStickPress);
    }

    //按下按钮
    private void OnJoyStickPress(GameEvent @event)
    {
        //判断是不是通关了
        if (GameFacade.Instance.DataCenter.Levels.LoadLevelJSON(GameFacade.Instance.DataCenter.User.Level + 1) == null) {
            EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "未完待续..."));
            return;
        }

        m_FSM.Transist(_C.FSMSTATE.PLAY);
    }
}
