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
        EventManager.AddHandler(EVENT.ONJOYSTICK,   OnJoyStick);
    }

    public override void Update()
    {

    }

    public override void Exit()
    {
        EventManager.DelHandler(EVENT.ONJOYSTICK,   OnJoyStick);
    }

    //拖拽摇杆
    private void OnJoyStick(GameEvent @event)
    {
        Vector2 input = (Vector2)@event.GetParam(0);

        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;

        Field.Instance.Player.Move(angle);
    }
}
