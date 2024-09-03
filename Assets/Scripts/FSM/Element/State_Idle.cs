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
        int level_id = DataCenter.Instance.User.Level + 1;

        if (GameFacade.Instance.TestMode == true)
        {
            level_id    = GameFacade.Instance.TestStage;
        }

        //判断是不是通关了
        if (DataCenter.Instance.Levels.LoadLevelJSON(level_id) == null) {
            EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "未完待续..."));
            return;
        }

        
        Field.Instance.Play(level_id);
    }
}
