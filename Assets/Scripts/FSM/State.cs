using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态基类
/// </summary>
public class State<T> where T : class
{
    public _C.FSMSTATE ID;
    public FSM<T> m_FSM;

    // public virtual void Enter() {}   //状态委托：开始
    public virtual void Enter(params object[] values) {}

    public virtual void Update() {}  //状态委托：刷新
    public virtual void Exit() {}    //状态委托：退出



    public State(_C.FSMSTATE id)
    {
        ID      = id;
    }

    public void SetFSM(FSM<T> fsm)
    {
        m_FSM = fsm;
    }
}