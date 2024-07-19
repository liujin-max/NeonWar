using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Field : MonoBehaviour
{
    public HollowCircle HollowCircle;

    private static Field m_Instance;
    public static Field Instance{ get { return m_Instance;}}

    private FSM<Field> m_FSM;
    public _C.GAME_STATE STATE = _C.GAME_STATE.PAUSE;

    [HideInInspector] public Land Land;
    public Spawn Spawn; //怪物工厂

    private Player m_Player;
    public Player Player{ get { return m_Player;}}
    

    public bool LeftBtnPressFlag    = false;
    public bool RightBtnPressFlag   = false;

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
            new State_Play<Field>(_C.FSMSTATE.PLAY),
            new State_Result<Field>(_C.FSMSTATE.RESULT),
        });
    }

    public void Enter()
    {
        STATE   = _C.GAME_STATE.PLAY;

        Land    = new Land();
        Spawn   = new Spawn();

        InitPlayer();

        GameFacade.Instance.UIManager.LoadWindow("GameWindow", UIManager.BOTTOM).GetComponent<GameWindow>();

        Field.Instance.Pause();

        m_FSM.Transist(_C.FSMSTATE.IDLE);
    }

    public void Pause()
    {
        STATE   = _C.GAME_STATE.PAUSE;

        Spawn.Pause();
    }

    public void Resume()
    {
        STATE   = _C.GAME_STATE.PLAY;

        Spawn.Resume();
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

        float deltaTime = Time.deltaTime;

        if (m_FSM != null) m_FSM.Update();

        Spawn.Update(deltaTime);
    }

    public _C.RESULT CheckResult()
    {
        if (m_Player.IsDead() == true) return _C.RESULT.LOSE;

        
        return _C.RESULT.NONE;
    }

    public bool IsPressing()
    {
        return LeftBtnPressFlag || RightBtnPressFlag;
    }


    #region 逻辑处理
    void InitPlayer()
    {
        m_Player = GameFacade.Instance.UIManager.LoadPrefab("Prefab/Element/Player", Vector2.zero, Land.ENTITY_ROOT).GetComponent<Player>();
        m_Player.Init(270);
    }


    //子弹击中敌人
    public void Hit(Bullet bullet, Enemy enemy)
    {
        if (bullet.Caster.IsDead()) return;

        enemy.UpdateHP(-bullet.Caster.ATT.ATK);
    }

    //敌人碰撞玩家
    public void Crash(Enemy enemy, Player player)
    {
        player.UpdateHP(-enemy.ATT.ATK);
    }

    #endregion
}
