using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//结算
public class State_Result<T> : State<Field>
{
    private CONST.RESULT m_Result;
    private CDTimer m_DelayTimer = new CDTimer(1);



    public State_Result(CONST.FSMSTATE id) : base(id){}

    public override void Enter(params object[] values)
    {
        m_Result = (CONST.RESULT) values[0];
        m_DelayTimer.ForceReset();

        GameFacade.Instance.TimeManager.BulletTime();
    }

    public override void Update()
    {
        m_DelayTimer.Update(Time.deltaTime);
        if (m_DelayTimer.IsFinished() == true) {
            Field.Instance.STATE = CONST.GAME_STATE.PAUSE;

            //胜利
            if (m_Result == CONST.RESULT.VICTORY)
            {   
                //记录通关
                DataCenter.Instance.User.SetLevel(Field.Instance.Level.ID);
            }


            //计算碎片奖励
            Field.Instance.GenerateGlassRewards(out int base_glass, out int worth_glass);

            int glass_total = base_glass + worth_glass;
            DataCenter.Instance.User.UpdateGlass(glass_total);
            EventManager.SendEvent(new GameEvent(EVENT.ONUPDATEGLASS));

            //计算道具奖励
            List<Pear> pears = Field.Instance.GeneratePearRewards(m_Result);
            
            UICtrl_ResultWindow.Instance.Enter((window)=>{
                window.Init(m_Result, base_glass, worth_glass, (rate)=>{
                    DataCenter.Instance.User.UpdateGlass(glass_total * rate);

                    EventManager.SendEvent(new GameEvent(EVENT.ONUPDATEGLASS));
                });
                window.InitPears(pears);
            });
        }
    }

    public override void Exit()
    {
        UICtrl_ResultWindow.Instance.Exit();
    }
}
