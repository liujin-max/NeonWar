using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Field : MonoBehaviour
{
    private static Field m_Instance;
    public static Field Instance{ get { return m_Instance;}}

    private FSM<Field> m_FSM;
    public _C.GAME_STATE STATE = _C.GAME_STATE.PAUSE;

    private Land Land;




    void Awake()
    {
        m_Instance = this;
    }

    void OnDestroy()
    {
        
    }

    void Start()
    {
        m_FSM = new FSM<Field>(this,  new State<Field>[] {
            new State_Idle<Field>(_C.FSMSTATE.IDLE),

        });

    
    }

    public void Enter(int stage)
    {
        STATE   = _C.GAME_STATE.PLAY;

        Land    = new Land();


        m_FSM.Transist(_C.FSMSTATE.IDLE);
    }

    public void Pause()
    {
        STATE   = _C.GAME_STATE.PAUSE;
    }

    public void Resume()
    {
        STATE   = _C.GAME_STATE.PLAY;
    }

    public void Transist(_C.FSMSTATE state, params object[] values)
    {
        m_FSM.Transist(state, values);
    }

    public _C.FSMSTATE GetCurrentFSMState()
    {
        return m_FSM.CurrentState.ID;
    }

    void Update()
    {
        if (this.STATE != _C.GAME_STATE.PLAY) return;

        if (m_FSM != null) m_FSM.Update();
    }

    void FixedUpdate()
    {
        
    }

    public _C.RESULT CheckResult()
    {
        return _C.RESULT.NONE;
    }



}
