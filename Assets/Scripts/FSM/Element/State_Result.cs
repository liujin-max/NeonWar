using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//结算
public class State_Result<T> : State<Field>
{
    private _C.RESULT m_Result;
    private CDTimer m_DelayTimer = new CDTimer(1);


    public State_Result(_C.FSMSTATE id) : base(id){}

    public override void Enter(params object[] values)
    {
        m_Result = (_C.RESULT) values[0];
        m_DelayTimer.ForceReset();


        GameFacade.Instance.TimeManager.BulletTime();
    }

    public override void Update()
    {
        if (m_DelayTimer == null) return;

        m_DelayTimer.Update(Time.deltaTime);
        if (m_DelayTimer.IsFinished() == true) {
            //胜利
            if (m_Result == _C.RESULT.VICTORY)
            {   
                //记录通关
                GameFacade.Instance.DataCenter.User.SetLevel(Field.Instance.Level.ID);
            }

            var window = GameFacade.Instance.UIManager.LoadWindow("ResultWindow", UIManager.BOARD).GetComponent<ResultWindow>();
            window.Init(m_Result);

            //结算奖励
            Field.Instance.ReceiveRewards();
            
            
            Field.Instance.End();
        }
    }

    public override void Exit()
    {
        GameFacade.Instance.UIManager.UnloadWindow("ResultWindow");
    }
}
