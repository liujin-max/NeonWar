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
        Debug.Log("进入 待机阶段");
        EventManager.AddHandler(EVENT.ONJOYSTICK_PRESS,     OnJoyStickPress);

        var game_window = GameFacade.Instance.UIManager.GetWindow("GameWindow").GetComponent<GameWindow>();
        game_window.ShowJoyStick(true);


        Field.Instance.InitPlayer();
    }

    public override void Update()
    {

    }

    public override void Exit()
    {
        Debug.Log("离开 待机阶段");

        EventManager.DelHandler(EVENT.ONJOYSTICK_PRESS,     OnJoyStickPress);
    }

    //按下按钮
    private void OnJoyStickPress(GameEvent @event)
    {
        m_FSM.Transist(_C.FSMSTATE.PLAY);
    }
}
