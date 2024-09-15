using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//结算
public class State_Result<T> : State<Field>
{
    private CONST.RESULT m_Result;
    private CDTimer m_DelayTimer = new CDTimer(1);

    private ResultWindow m_ResultWindow;


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

            //计算宝珠奖励
            Dictionary<int, int> pear_dic = new Dictionary<int, int>();
            
            if (m_Result == CONST.RESULT.VICTORY)
            {
                Field.Instance.GeneratePearRewards(out pear_dic);

                if (pear_dic.Count > 0) {
                    foreach (var pear_keypairs in pear_dic) {
                        DataCenter.Instance.Backpack.PushPear(pear_keypairs.Key, pear_keypairs.Value);
                    }
                }
            }
            

            GameFacade.Instance.UIManager.LoadWindowAsync(UI.RESULTWINDOW, UIManager.BOARD, (obj)=>{
                m_ResultWindow = obj.GetComponent<ResultWindow>();
                m_ResultWindow.Init(m_Result, base_glass, worth_glass, (rate)=>{
                    DataCenter.Instance.User.UpdateGlass(glass_total * rate);

                    EventManager.SendEvent(new GameEvent(EVENT.ONUPDATEGLASS));
                });
                m_ResultWindow.InitPears(pear_dic);
            });

            
            
            // Field.Instance.End();
        }
    }

    public override void Exit()
    {
        GameFacade.Instance.UIManager.UnloadWindow(m_ResultWindow.gameObject);
    }
}
