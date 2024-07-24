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
    private Spawn m_Spawn; //怪物工厂
    public Spawn Spawn {get {return m_Spawn;}}

    private Player m_Player;
    public Player Player{ get { return m_Player;}}

    private Level m_Level;
    public Level Level{ get { return m_Level;}}
    
    //累计获得的碎片
    private int m_Glass;
    public int Glass{get {return m_Glass;}}

    public bool LeftBtnPressFlag    = false;
    public bool RightBtnPressFlag   = false;

    //闪现冷却
    public CDTimer BlinkTimer = new CDTimer(0);

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
        STATE   = _C.GAME_STATE.PAUSE;

        Land    = new Land();
        m_Spawn = new Spawn();

        GameFacade.Instance.UIManager.LoadWindow("GameWindow", UIManager.BOTTOM).GetComponent<GameWindow>().Init();


        m_FSM.Transist(_C.FSMSTATE.IDLE);
    }

    //开始游玩
    public void Play(int level_id)
    {
        STATE   = _C.GAME_STATE.PLAY;

        BlinkTimer.Reset(_C.DEFAULT_BLINKCD);
        BlinkTimer.Full();

        m_Level = GameFacade.Instance.DataCenter.Levels.GetLevel(level_id);
        m_Spawn.Init(level_id);

        Debug.Log("========  开始关卡：" + level_id + "  ========");

        //将最新的加成等级应用到Player身上
        m_Player.Sync();

        EventManager.SendEvent(new GameEvent(EVENT.ONBATTLESTART));
    }

    //结束游玩
    public void End()
    {
        STATE   = _C.GAME_STATE.PAUSE;

        m_Glass = 0;

        BlinkTimer.Full();

        Land.Dispose();
        m_Spawn.Dispose();


        EventManager.SendEvent(new GameEvent(EVENT.ONBATTLEEND));
    }


    public void Pause()
    {
        STATE   = _C.GAME_STATE.PAUSE;

        m_Spawn.Pause();
    }

    public void Resume()
    {
        STATE   = _C.GAME_STATE.PLAY;

        m_Spawn.Resume();
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

        m_Spawn.Update(deltaTime);
        BlinkTimer.Update(deltaTime);
    }

    public _C.RESULT CheckResult()
    {
        if (m_Spawn.IsClear() == true) return _C.RESULT.VICTORY;
        if (m_Player.IsDead() == true) return _C.RESULT.LOSE;

        return _C.RESULT.NONE;
    }

    public bool IsPressing()
    {
        return LeftBtnPressFlag || RightBtnPressFlag;
    }


    #region 逻辑处理
    public void UpdateGlass(int value)
    {
        m_Glass += value;

        EventManager.SendEvent(new GameEvent(EVENT.ONUPDATEGLASS, m_Glass));
    }

    public void InitPlayer()
    {
        if (m_Player != null) return;

        m_Player = GameFacade.Instance.UIManager.LoadPrefab("Prefab/Player/Player_10000", Vector2.zero, Land.ENTITY_ROOT).GetComponent<Player>();
        m_Player.Init(270);
    }

    public void RemovePlayer()
    {
        if (m_Player != null) {
            m_Player.Dispose();
            m_Player = null;
        }
    }


    //子弹击中敌人
    public void Hit(Bullet bullet, Unit unit)
    {
        if (bullet.Caster.IsDead()) return;

        unit.UpdateHP(-bullet.Caster.ATT.ATK);


        EventManager.SendEvent(new GameEvent(EVENT.ONBULLETHIT, bullet, unit));

        if (unit.IsDead() == true)
        {
            if (unit.Side == _C.SIDE.PLAYER) 
            {
                Land.DoShake();
            }

            //怪物死亡
            if (unit.Side == _C.SIDE.ENEMY)
            {
                Land.DoSmallShake();

                var e = GameFacade.Instance.EffectManager.Load(EFFECT.BROKEN, unit.transform.localPosition, Land.ELEMENT_ROOT.gameObject);
                e.transform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(bullet.Velocity.y, bullet.Velocity.x) * Mathf.Rad2Deg);
            }
        }
        else
        {
            if (unit.Side == _C.SIDE.PLAYER) 
            {
                Land.DoShake();
                GameFacade.Instance.EffectManager.Load(EFFECT.CRASH, Vector3.zero, Field.Instance.Land.ELEMENT_ROOT.gameObject);
            }
        }
    }

    //敌人碰撞玩家
    public void Crash(Enemy enemy, Player player)
    {
        //无敌了
        if (player.IsInvincible() == true) return;

        player.UpdateHP(-enemy.ATT.ATK);
        player.InvincibleTimer.ForceReset();

        Land.DoShake();
        //特效处理
        if (player.IsDead() == true) {}
        else GameFacade.Instance.EffectManager.Load(EFFECT.CRASH, Vector3.zero, Field.Instance.Land.ELEMENT_ROOT.gameObject);
    }

    #endregion
}
