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
        Spawn   = new Spawn();

        GameFacade.Instance.UIManager.LoadWindow("GameWindow", UIManager.BOTTOM).GetComponent<GameWindow>().Init();


        m_FSM.Transist(_C.FSMSTATE.IDLE);
    }

    //开始游玩
    public void Play()
    {
        STATE   = _C.GAME_STATE.PLAY;

        BlinkTimer.Reset(_C.DEFAULT_BLINKCD);
        BlinkTimer.Full();

        m_Level = GameFacade.Instance.DataCenter.Levels.GetLevel(GameFacade.Instance.DataCenter.User.Level + 1);
        Spawn.Init(Field.Instance.FML_EnemyCount(m_Level.ID));

        Debug.Log("========  开始关卡：" + m_Level.ID + "  ========");

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
        Spawn.Dispose();


        EventManager.SendEvent(new GameEvent(EVENT.ONBATTLEEND));
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
        BlinkTimer.Update(deltaTime);
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


    #region 数值公式
    //敌人数量和关卡的公式
    public int FML_EnemyCount(int stage_level)
    {
        //第一关敌人的数量
        int e1  = 5; 
        //线性增长系数
        float m = 2f;
        //后期增长系数
        float a = 0.01f;
        //增长指数
        float b = 2f;

        return Mathf.FloorToInt(e1 + m * (stage_level - 1) + a * Mathf.Pow(stage_level - 1, b));
    }

    

    //敌人血量和关卡的关系公式
    public int FML_EnemyHP(int stage_level)
    {
        //第一关敌人的基础血量
        int hp_base = 2;  
        //血量增长率，设定为0.1（每关增加10%） 
        float pr    = 0.1f;

        int hp_now  = Mathf.FloorToInt(hp_base * Mathf.Pow(1 + pr, stage_level - 1));

        //上下浮动
        int hp_min  = Mathf.FloorToInt(hp_now * 0.8f);
        int hp_max  = Mathf.CeilToInt(hp_now * 1.2f);

        return RandomUtility.Random(hp_min, hp_max);
    }

    //敌人死亡时掉落的碎片和敌人血量的公式
    public int FML_HP2Glass(int hp)
    {
        // k 是比例系数(0.5， 表示每2滴血掉落1颗碎片)
        float k = 0.5f;

        return Mathf.FloorToInt(k * hp);
    }

    //升级攻击力消耗的碎片数量和等级的公式
    public int FML_ATKCost(int atk_level)
    {
        //第一次升级消耗的数量
        int cost_base = 5;

        //增长指数
        float cost_pa = 1.5f;

        return Mathf.FloorToInt(cost_base * Mathf.Pow(atk_level, cost_pa));
    }

    //升级攻速消耗的碎片数量和等级的公式
    public int FML_ASPCost(int asp_level)
    {
        //第一次升级消耗的数量
        int cost_base = 5;

        //增长指数
        float cost_pa = 1.2f;

        return Mathf.FloorToInt(cost_base * Mathf.Pow(asp_level, cost_pa));
    }

    #endregion


    #region 逻辑处理
    public void UpdateGlass(int value)
    {
        m_Glass += value;

        EventManager.SendEvent(new GameEvent(EVENT.ONUPDATEGLASS, m_Glass));
    }

    public void InitPlayer()
    {
        if (m_Player != null) return;

        m_Player = GameFacade.Instance.UIManager.LoadPrefab("Prefab/Element/Player", Vector2.zero, Land.ENTITY_ROOT).GetComponent<Player>();
        m_Player.Init(270);
        m_Player.Sync();
    }

    public void RemovePlayer()
    {
        m_Player.Dispose();
        m_Player = null;
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
