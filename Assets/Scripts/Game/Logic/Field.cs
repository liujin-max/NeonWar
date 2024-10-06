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
    public CONST.GAME_STATE STATE = CONST.GAME_STATE.PAUSE;

    [HideInInspector] public Land Land;
    private Spawn m_Spawn; //怪物工厂
    public Spawn Spawn {get {return m_Spawn;}}

    private Player m_Player;
    public Player Player{ get { return m_Player;}}

    private Level m_Level;
    public Level Level {get {return m_Level;}}

    private List<Bullet> m_Bullets = new List<Bullet>();
    public List<Bullet> Bullets {get {return m_Bullets;}}


    public CycleList<BuffBubble> BuffBubbles = new CycleList<BuffBubble>(null);
    public CycleList<Area> Areas = new CycleList<Area>((a)=>{ EventManager.SendEvent(new GameEvent(EVENT.ONREMOVEAREA, a));});
    


    void Awake()
    {
        m_Instance = this;
    }

    void Start()
    {
        m_FSM = new FSM<Field>(this,  new State<Field>[] {
            new State_Idle<Field>(CONST.FSMSTATE.IDLE),
            new State_Play<Field>(CONST.FSMSTATE.PLAY),
            new State_Upgrade<Field>(CONST.FSMSTATE.UPGRADE),
            new State_Result<Field>(CONST.FSMSTATE.RESULT),
        });
    }

    public void Enter()
    {
        STATE   = CONST.GAME_STATE.PAUSE;

        Land    = new Land();
        m_Spawn = new Spawn();


        m_FSM.Transist(CONST.FSMSTATE.IDLE);
    }

    //开始游玩
    public void Play(int level_id)
    {
        //玩家开始触摸后，再转换状态
        // STATE   = CONST.GAME_STATE.PLAY;


        m_Level = DataCenter.Instance.Levels.GetLevel(level_id);
        m_Level.Init();
        m_Spawn.Init(m_Level.LevelJSON);

        Debug.Log("========  开始关卡：" + level_id + "  ========");

        //将最新的加成等级应用到Player身上
        InitPlayer();

        m_FSM.Transist(CONST.FSMSTATE.PLAY);


        UICtrl_BattleWindow.Instance.Enter();

        EventManager.SendEvent(new GameEvent(EVENT.ONBATTLESTART));
    }

    //结束游玩
    public void End()
    {
        STATE   = CONST.GAME_STATE.PAUSE;

        Land.Dispose();
        m_Spawn.Dispose();


        BuffBubbles.Dispose();
        Areas.Dispose();


        RemovePlayer();

        UICtrl_BattleWindow.Instance.Exit();

        EventManager.SendEvent(new GameEvent(EVENT.ONBATTLEEND));
    }


    public void Pause()
    {
        STATE   = CONST.GAME_STATE.PAUSE;

        m_Spawn.Pause();
    }

    public void Resume()
    {
        STATE   = CONST.GAME_STATE.PLAY;

        m_Spawn.Resume();
    }

    public void Transist(CONST.FSMSTATE state, params object[] values)
    {
        m_FSM.Transist(state, values);
    }

    public CONST.FSMSTATE GetCurrentFSMState()
    {
        if (m_FSM == null || m_FSM.CurrentState == null) return CONST.FSMSTATE.IDLE;

        return m_FSM.CurrentState.ID;
    }


    void Update()
    {
        if (this.STATE != CONST.GAME_STATE.PLAY) return;

        if (m_FSM != null) m_FSM.Update();
    }

    //自定义刷新
    public void CustomUpdate(float deltaTime)
    {
        m_Player.CustomUpdate(deltaTime);
        m_Spawn.CustomUpdate(deltaTime);
        
        //区域
        Areas.CustomUpdate(deltaTime);
        //Buff
        BuffBubbles.CustomUpdate(deltaTime);
    }

    public CONST.RESULT CheckResult()
    {
        if (m_Spawn.IsClear() == true) return CONST.RESULT.VICTORY;
        if (m_Player.IsDead() == true) return CONST.RESULT.LOSE;
        if (m_Spawn.IsCurrentClear() == true) return CONST.RESULT.UPGRADE;

        return CONST.RESULT.NONE;
    }

    #region 逻辑处理
    //
    public void Heal(Unit unit, int value)
    {
        unit.UpdateHP(value);

        GameFacade.Instance.EffectManager.Load(EFFECT.HEAL, Vector3.zero, unit.gameObject);

        EventManager.SendEvent(new GameEvent(EVENT.ONHPUPDATE));
    }

    //根据当前的击杀进度计算出可以获得多少碎片奖励
    public void GenerateGlassRewards(out int base_value, out int worth_value)
    {
        float kill_rate = m_Spawn.KillProgress;

        base_value = Mathf.FloorToInt(m_Level.LevelJSON.Glass * kill_rate) ;

        //额外倍率
        worth_value = Mathf.CeilToInt(base_value * m_Player.GlassRate.ToNumber()) - base_value;
    }

    //计算道具奖励
    public List<Pear> GeneratePearRewards(CONST.RESULT result)
    {
        List<Pear> pears = new List<Pear>();
        if (string.IsNullOrEmpty(m_Level.LevelJSON.PearLevel)) {
            return pears;
        }

        int[] section   = m_Level.LevelJSON.PearCount.Split("-").Select(int.Parse).ToArray();
        int count       = result == CONST.RESULT.VICTORY ? RandomUtility.Random(section[0], section[1] + 1) : 1;
        int[] levels    = m_Level.LevelJSON.PearLevel.Split("-").Select(int.Parse).ToArray();

        for (int i = 0; i < count; i++)
        {
            var data    = DataCenter.Instance.Backpack.PickPearData();
            int level   = result == CONST.RESULT.VICTORY ? RandomUtility.Random(levels[0], levels[1] + 1) : levels[0];
            Pear pear   =  DataCenter.Instance.Backpack.PushPear(data.ID, level);
            pears.Add(pear);
        }

        return pears;
    }

    public void InitPlayer()
    {
        if (m_Player != null) return;

        int id  = DataCenter.Instance.User.CurrentPlayer.ID;
        m_Player= GameFacade.Instance.AssetManager.LoadPrefab("Prefab/Player/Player_" + id, Vector2.zero, Land.ENTITY_ROOT).GetComponent<Player>();
        m_Player.Init(id, 270);
        m_Player.Sync();
    }

    public void RemovePlayer()
    {
        if (m_Player != null) {
            m_Player.Dispose();
            m_Player = null;
        }
    }

    //子弹
    public Bullet CreateBullet(Unit unit)
    {
        var bullet = GameFacade.Instance.PoolManager.AllocateBullet(unit.BulletTemplate);
        bullet.transform.position = unit.ShootPivot.position;
        bullet.Init(unit);

        m_Bullets.Add(bullet);

        EventManager.SendEvent(new GameEvent(EVENT.ONBULLETCREATE, bullet));

        return bullet;
    }

    public void RecycleBullet(Bullet bullet)
    {
        m_Bullets.Remove(bullet);

        GameFacade.Instance.PoolManager.RecycleBullet(bullet);
    }

    //在场地上生成可拾取的Buff
    public void PushBuffBubble(int id, int value)
    {
        var point   = ToolUtility.FindPointOnCircle(Vector2.zero, CONST.DEFAULT_RADIUS, RandomUtility.Random(0, 360));

        GameFacade.Instance.AssetManager.AsyncLoadPrefab("Prefab/Element/BuffBubble", point, Land.ELEMENT_ROOT, (obj)=>{
            var bubble = obj.GetComponent<BuffBubble>();
            bubble.Init(id, value);

            BuffBubbles.Add(bubble);
        });
    }

    //在场地上生成区域
    public void PushArea(Unit caster, string area_path, Vector2 point, float time, float scale = 1.0f)
    {
        GameFacade.Instance.AssetManager.AsyncLoadPrefab(area_path, point, Land.ELEMENT_ROOT, (obj)=>{
            var area = obj.GetComponent<Area>();
            area.Init(caster, time);
            area.transform.localScale = new Vector3(scale, scale, 1);

            Areas.Add(area);

            EventManager.SendEvent(new GameEvent(EVENT.ONPUSHAREA, area));
        });
    }


    //击中目标
    public bool SettleHit(Hit hit, Unit unit)
    {
        if (this.STATE != CONST.GAME_STATE.PLAY) return false;
        //无敌了
        if (unit.IsInvincible() == true) return false;
        if (unit.IsDead()) return false;
        if (hit.IgnoreUnits.Contains(unit)) return false;
        

        float demage = Mathf.Ceil(hit.ATK.ToNumber(false) * hit.ATK_INC.ToNumber() * unit.VUN_INC.ToNumber());

        //护盾
        if (hit.Type == CONST.HIT_TYPE.NORMAL && unit.GetBuff((int)CONST.BUFF.SHIELD) != null)
        {
            demage = 0;
        }
        else if (hit.Type == CONST.HIT_TYPE.NORMAL && RandomUtility.IsHit((int)unit.ATT.DODGE.ToNumber(), 1000) == true) //闪避了
        {
            //闪避特效
            if (unit == m_Player) GameFacade.Instance.EffectManager.Load(EFFECT.DODGE, hit.Position, Land.ELEMENT_ROOT.gameObject);
            else  GameFacade.Instance.EffectManager.Load(EFFECT.MISS, hit.Position, Land.ELEMENT_ROOT.gameObject);

            demage = 0;

            EventManager.SendEvent(new GameEvent(EVENT.ONDODGE, unit));
        }
        else
        {
            if (RandomUtility.IsHit(hit.KillRate) == true && unit.IsBoss() == false)   //一击必杀
            {
                demage = unit.ATT.HPMAX;

                //必杀特效
                hit.IsHitKill = true;
                GameFacade.Instance.EffectManager.Load(EFFECT.CRITKILL, unit.transform.localPosition, Land.ELEMENT_ROOT.gameObject);
            }
            else
            {
                //对Boss伤害加成
                if (unit.IsBoss()) demage *= hit.BOSS_INC;
                //对健康敌人的伤害加成
                if (unit.IsHPFull()) demage *= hit.HEALTH_INC;
                //对减速敌人的伤害加成
                if (hit.SLOW_INC != 1 && unit.IsSlow()) demage *= hit.SLOW_INC;
                //对受控制敌人的伤害加成
                if (unit.IsControlled()) demage *= hit.CONTROL_INC;
                

                //是否暴击
                hit.IsCrit = RandomUtility.IsHit((int)hit.CP.ToNumber(), 1000);
                if (hit.IsCrit == true)
                {
                    demage  *= hit.CT.ToNumber() / 1000.0f;

                    //暴击特效
                    GameFacade.Instance.EffectManager.Load(EFFECT.CRIT, hit.Position, Land.ELEMENT_ROOT.gameObject);
                }
            }
        }

        hit.FINAL   = Mathf.FloorToInt(demage);
        demage      = hit.FINAL;

        unit.UpdateHP(-(int)demage); 

        EventManager.SendEvent(new GameEvent(EVENT.ONHIT, hit, unit));
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
            EventManager.SendEvent(new GameEvent(EVENT.ONDODGE, player));
            GameFacade.Instance.EffectManager.Load(EFFECT.DODGE, player.transform.localPosition, Land.ELEMENT_ROOT.gameObject);
            return;
        }

        var hit = new Hit(enemy);

        //撞击伤害
        player.UpdateHP(-1);

        EventManager.SendEvent(new GameEvent(EVENT.ONCRASH, enemy, player));
        EventManager.SendEvent(new GameEvent(EVENT.ONHPUPDATE));

        //受击表现
        if (player.IsDead() == true) player.Dead();
        else player.Affected();
    }

    //掉落Buff
    public void OnBuffFall(Hit hit, Enemy enemy)
    {
        //概率动态变化，未触发则+5%概率
        if (!RandomUtility.IsHit((int)m_Player.BlessRate, 1000))
        {
            m_Player.BlessRate = Mathf.Min(m_Player.BlessRate + 50, (int)m_Player.ATT.BLESS.ToNumber());
            return;
        }

        m_Player.BlessRate = 0;

        //掉落Buff逻辑
        List<CONST.BUFF> buffs = new List<CONST.BUFF>()
        {
            CONST.BUFF.ATK_UP, CONST.BUFF.ATK_DOWN,
            CONST.BUFF.ASP_UP, CONST.BUFF.ASP_DOWN, 
            CONST.BUFF.SPEED_UP, CONST.BUFF.SPEED_DOWN,
            CONST.BUFF.CP, CONST.BUFF.DODGE_UP, CONST.BUFF.SPD_MUL
        };

        if (RandomUtility.IsHit((int)Field.Instance.Player.ATT.LUCKY.ToNumber(), 1000) == true)
        {
            buffs.Remove(CONST.BUFF.ATK_DOWN);
            buffs.Remove(CONST.BUFF.ASP_DOWN);
            buffs.Remove(CONST.BUFF.SPEED_DOWN);
        }
        

        int rand    = RandomUtility.Random(0, buffs.Count);
        int buff_id = (int)buffs[rand];
        Field.Instance.PushBuffBubble(buff_id, 1);
    }

    #endregion
}
