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
        _C.RESULT result = (_C.RESULT) values[0];

        //胜利
        if (result == _C.RESULT.VICTORY)
        {   
            //记录通关
            GameFacade.Instance.DataCenter.User.SetLevel(Field.Instance.Level.ID);
        }

        var window = GameFacade.Instance.UIManager.LoadWindow("ResultWindow", UIManager.BOARD).GetComponent<ResultWindow>();
        window.Init(result);

        //结算奖励
        GameFacade.Instance.DataCenter.User.UpdateGlass(Field.Instance.Glass);
        
        
        Field.Instance.End();
    }

    public override void Update()
    {

    }

    public override void Exit()
    {
        GameFacade.Instance.UIManager.UnloadWindow("ResultWindow");
    }
}
