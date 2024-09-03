using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Upgrade<T> : State<Field>
{
    public State_Upgrade(_C.FSMSTATE id) : base(id) {}

    public override void Enter(params object[] values)
    { 
        Debug.Log("进入升级阶段");

        GameFacade.Instance.UIManager.LoadWindowAsync("SkillWindow", UIManager.BOARD, (obj)=>{
            var window = obj.GetComponent<SkillWindow>();
            window.Init();
        });
    }

    public override void Update()
    {

    }

    public override void Exit()
    {

    }
}
