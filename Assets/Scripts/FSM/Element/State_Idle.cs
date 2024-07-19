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
        EventManager.AddHandler(EVENT.ONJOYSTICK_DRAG,      OnJoyStickDrag);


        var game_window = GameFacade.Instance.UIManager.GetWindow("GameWindow").GetComponent<GameWindow>();
        game_window.ShowJoyStick(true);
    }

    public override void Update()
    {

    }

    public override void Exit()
    {
        Debug.Log("离开 待机阶段");

        EventManager.DelHandler(EVENT.ONJOYSTICK_DRAG,      OnJoyStickDrag);
    }

    //拖拽摇杆
    private void OnJoyStickDrag(GameEvent @event)
    {
        m_FSM.Transist(_C.FSMSTATE.PLAY);
    }
}
