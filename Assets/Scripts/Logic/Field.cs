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
    public Level Level {get {return m_Level;}}

    private List<BuffBubble> m_BuffBubbles = new List<BuffBubble>();
    private List<Area> m_Areas = new List<Area>();
    
    //累计获得的碎片
    private int m_Glass;

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
        m_Level.Init();
        m_Spawn.Init(m_Level.LevelJSON);

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

        m_BuffBubbles.ForEach(b => b.Dispose());
        m_BuffBubbles.Clear();

        m_Areas.ForEach(a => a.Dispose());
        m_Areas.Clear();


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
        BlinkTimer.Update(deltaTime);

        m_Player.CustomUpdate(deltaTime);
        m_Spawn.CustomUpdate(deltaTime);
        

        //区域
        List<Area> _Removes = new List<Area>();
        m_Areas.ForEach(a => {
            a.CustomUpdate(deltaTime);

            if (a.IsFinished()) _Removes.Add(a);
        });

        _Removes.ForEach(a => {
            Field.Instance.RemoveArea(a);
        });

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
    }

    
    //根据当前的击杀进度计算出可以获得多少碎片奖励
    public void ReceiveRewards()
    {
        float kill_rate = m_Spawn.KillProgress;

        int glass = Mathf.FloorToInt(m_Level.LevelJSON.Glass * kill_rate) + m_Glass;

        //额外倍率
        glass   = Mathf.CeilToInt(glass * m_Player.GlassRate.ToNumber());

        GameFacade.Instance.DataCenter.User.UpdateGlass(glass);
    }

    public void InitPlayer()
    {
        if (m_Player != null) return;

        int id  = GameFacade.Instance.DataCenter.User.CurrentPlayer.ID;
        m_Player= GameFacade.Instance.PrefabManager.Load("Prefab/Player/Player_" + id, Vector2.zero, Land.ENTITY_ROOT).GetComponent<Player>();
        m_Player.Init(id, 270);
    }

    public void RemovePlayer()
    {
        if (m_Player != null) {
            m_Player.Dispose();
            m_Player = null;
        }
    }

    //在场地上生成可拾取的Buff
    public void PushBuffBubble(int id, int value)
    {
        var point   = ToolUtility.FindPointOnCircle(Vector2.zero, _C.DEFAULT_RADIUS, RandomUtility.Random(0, 360));

        GameFacade.Instance.PrefabManager.AsyncLoad("Prefab/Element/BuffBubble", point, Land.ELEMENT_ROOT, (obj)=>{
            var bubble = obj.GetComponent<BuffBubble>();
            bubble.Init(id, value);

            m_BuffBubbles.Add(bubble);
        });
    }

    public void RemoveBuffBubble(BuffBubble b)
    {
        b.Dispose();

        m_BuffBubbles.Remove(b);
    }

    //在场地上生成区域
    public void PushArea(Unit caster, string area_path, Vector2 point, float time)
    {
        GameFacade.Instance.PrefabManager.AsyncLoad(area_path, point, Land.ELEMENT_ROOT, (obj)=>{
            var area = obj.GetComponent<Area>();
            area.Init(caster, time);

            m_Areas.Add(area);
        });
    }

    public void RemoveArea(Area area)
    {
        area.Dispose();
        m_Areas.Remove(area);
    }


    //击中目标
    public bool SettleHit(Hit hit, Unit unit)
    {
        if (this.STATE != _C.GAME_STATE.PLAY) return false;
        //无敌了
        if (unit.IsInvincible() == true) return false;
        if (unit.IsDead()) return false;
        if (hit.IgnoreUnits.Contains(unit)) return false;
        

        float demage = Mathf.RoundToInt(hit.ATK.ToNumber() * hit.ATK_INC.ToNumber() * unit.ATT.VUN_INC.ToNumber());

        //护盾
        if (unit.GetBuff((int)_C.BUFF.SHIELD) != null)
        {
            demage = 0;
        }
        else if (RandomUtility.IsHit((int)unit.ATT.DODGE.ToNumber(), 1000) == true) //闪避了
        {
            //闪避特效
            GameFacade.Instance.EffectManager.Load(EFFECT.MISS, hit.Position, Land.ELEMENT_ROOT.gameObject);

            demage = 0;

            EventManager.SendEvent(new GameEvent(EVENT.ONDODGE, hit, unit));
        }
        else
        {
            if (RandomUtility.IsHit(hit.KillRate) == true && (unit.Side == _C.SIDE.ENEMY && unit.GetComponent<Enemy>().TYPE != _C.ENEMY_TYPE.BOSS))   //一击必杀
            {
                demage = unit.ATT.HPMAX;
            }
            else
            {
                if (unit.Side == _C.SIDE.ENEMY)
                {
                    if (unit.GetComponent<Enemy>().TYPE == _C.ENEMY_TYPE.BOSS)
                    {
                        demage *= hit.BOSS_INC.ToNumber();
                    }
                }

                //是否暴击
                if (RandomUtility.IsHit((int)hit.CP.ToNumber(), 1000) == true)
                {
                    demage  = demage * hit.CT.ToNumber() / 1000.0f;

                    //暴击特效
                    GameFacade.Instance.EffectManager.Load(EFFECT.CRIT, hit.Position, Land.ELEMENT_ROOT.gameObject);
                }
            }
        }

        demage  = Mathf.FloorToInt(demage);

        unit.UpdateHP(-(int)demage); 

        EventManager.SendEvent(new GameEvent(EVENT.ONHPUPDATE));

        //受击表现
        if (unit.IsDead() == true)
        {
            unit.Dead(hit);
        }
        else
        {
            if (demage > 0) {
                unit.Affected(hit);
            }
        }

        return true;
    }

    //敌人碰撞玩家
    public void Crash(Enemy enemy, Player player)
    {
        //无敌了
        if (player.IsInvincible() == true) return;

        //闪避了
        if (RandomUtility.IsHit((int)player.ATT.DODGE.ToNumber(), 1000) == true) {
            //闪避特效
            return;
        }


        //撞击伤害
        int demage = enemy.TYPE == _C.ENEMY_TYPE.BOSS ? 3 : 1;
        player.UpdateHP(-demage);

        EventManager.SendEvent(new GameEvent(EVENT.ONHPUPDATE));

        //受击表现
        if (player.IsDead() == true)
        {
            player.Dead(null);
        }
        else
        {
            player.Affected(null);
        }
    }

    #endregion
}
