using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 状态机
/// </summary>
public class FSM<T> where T : class
{
    public Dictionary<CONST.FSMSTATE, State<T>> States = new Dictionary<CONST.FSMSTATE, State<T>>();

    public State<T> CurrentState;//当前状态
    public T Owner;
 
    public FSM(T owner, State<T>[] states)
    {
        Owner = owner;

        foreach (var state in states)
        {
            state.SetFSM(this);
            States.Add(state.ID, state);
        }
    }

    public State<T> GetState(CONST.FSMSTATE id)
    {
        return States[id];
    }

    public void Transist(CONST.FSMSTATE id, params object[] values)
    {
        if (CurrentState != null)
        {
            CurrentState.Exit();
        }

        State<T> state = GetState(id);
        if (state != null)
        {
            CurrentState = state;
            CurrentState.Enter(values);
        }
    }
 

    public void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.Update();
        }
    }

    public void Dispose()
    {
        if (CurrentState != null)
        {
            CurrentState.Exit();
        }
        CurrentState = null;


        States.Clear();
    }
}