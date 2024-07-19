using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//结算
public class State_Result<T> : State<Field>
{
    public State_Result(_C.FSMSTATE id) : base(id){}

    public override void Enter(params object[] values)
    {
        Debug.Log("进入 结算阶段");

        //结算奖励
        GameFacade.Instance.DataCenter.User.UpdateGlass(Field.Instance.Glass);


        GameFacade.Instance.UIManager.LoadWindow("ResultWindow", UIManager.BOARD).GetComponent<ResultWindow>();
    }

    public override void Update()
    {

    }

    public override void Exit()
    {
        GameFacade.Instance.UIManager.UnloadWindow("ResultWindow");
    }
}
