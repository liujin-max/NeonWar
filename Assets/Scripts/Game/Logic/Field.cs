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
    public GAME_STATE STATE = GAME_STATE.PAUSE;

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
    public CycleList<Area> Areas = new CycleList<Area>((a)=>{ new Event_AreaRemove(){Area = a}.Notify();});
    


    void Awake()
    {
        m_Instance = this;
    }

    void Start()
    {
        m_FSM = new FSM<Field>(this,  new State<Field>[] {
            new State_Idle<Field>(FSMSTATE.IDLE),
            new State_Play<Field>(FSMSTATE.PLAY),
            new State_Upgrade<Field>(FSMSTATE.UPGRADE),
            new State_Result<Field>(FSMSTATE.RESULT),
        });
    }

    public void Enter()
    {
        STATE   = GAME_STATE.PAUSE;

        Land    = new Land();
        m_Spawn = new Spawn();


        m_FSM.Transist(FSMSTATE.IDLE);
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

        m_FSM.Transist(FSMSTATE.PLAY);


        UICtrl_BattleWindow.Instance.Enter();

        new Event_BattleStart().Notify();
    }

    //结束游玩
    public void End()
    {
        STATE   = GAME_STATE.PAUSE;

        Land.Dispose();
        m_Spawn.Dispose();


        BuffBubbles.Dispose();
        Areas.Dispose();


        RemovePlayer();

        UICtrl_BattleWindow.Instance.Exit();

        new Event_BattleEnd().Notify();
    }


    public void Pause()
    {
        STATE   = GAME_STATE.PAUSE;

        m_Spawn.Pause();
    }

    public void Resume()
    {
        STATE   = GAME_STATE.PLAY;

        m_Spawn.Resume();
    }

    public void Transist(FSMSTATE state, params object[] values)
    {
        m_FSM.Transist(state, values);
    }

    public FSMSTATE GetCurrentFSMState()
    {
        if (m_FSM == null || m_FSM.CurrentState == null) return FSMSTATE.IDLE;

        return m_FSM.CurrentState.ID;
    }


    void Update()
    {
        if (this.STATE != GAME_STATE.PLAY) return;

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

    public RESULT CheckResult()
    {
        if (m_Spawn.IsClear() == true) return RESULT.VICTORY;
        if (m_Player.IsDead() == true) return RESULT.LOSE;
        if (m_Spawn.IsCurrentClear() == true) return RESULT.UPGRADE;

        return RESULT.NONE;
    }

    #region 逻辑处理
    //
    public void Heal(Unit unit, int value)
    {
        unit.UpdateHP(value);

        GameFacade.Instance.EffectManager.Load(EFFECT.HEAL, Vector3.zero, unit.gameObject);

        new Event_UpdateHP().Notify();
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
    public List<Pear> GeneratePearRewards(RESULT result)
    {
        List<Pear> pears = new List<Pear>();
        if (!DataCenter.Instance.IsPearUnlock()) return pears;
        if (string.IsNullOrEmpty(m_Level.LevelJSON.PearLevel)) return pears;

        int[] section   = m_Level.LevelJSON.PearCount.Split("-").Select(int.Parse).ToArray();
        int count       = result == RESULT.VICTORY ? RandomUtility.Random(section[0], section[1] + 1) : 1;
        int[] levels    = m_Level.LevelJSON.PearLevel.Split("-").Select(int.Parse).ToArray();

        for (int i = 0; i < count; i++)
        {
            var data    = DataCenter.Instance.Backpack.PickPearData();
            int level   = result == RESULT.VICTORY ? RandomUtility.Random(levels[0], levels[1] + 1) : levels[0];
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

        new Event_BulletCreate(){Bullet = bullet}.Notify();

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

            new Event_AreaPush(){Area = area}.Notify();
        });
    }


    //击中目标
    public bool SettleHit(Hit hit, Unit unit)
    {
        if (this.STATE != GAME_STATE.PLAY) return false;
        //无敌了
        if (unit.IsInvincible() == true) return false;
        if (unit.IsDead()) return false;
        if (hit.IgnoreUnits.Contains(unit)) return false;
        

        float demage = Mathf.Ceil(hit.ATK.ToNumber(false) * hit.ATK_INC.ToNumber() * unit.VUN_INC.ToNumber());

        //护盾
        if (hit.Type == HIT_TYPE.NORMAL && unit.GetBuff((int)BUFF.SHIELD) != null)
        {
            demage = 0;
        }
        else if (hit.Type == HIT_TYPE.NORMAL && RandomUtility.IsHit((int)unit.ATT.DODGE.ToNumber(), 1000) == true) //闪避了
        {
            //闪避特效
            if (unit == m_Player) GameFacade.Instance.EffectManager.Load(EFFECT.DODGE, hit.Position, Land.ELEMENT_ROOT.gameObject);
            else  GameFacade.Instance.EffectManager.Load(EFFECT.MISS, hit.Position, Land.ELEMENT_ROOT.gameObject);

            demage = 0;

            new Event_Dodge(){Unit = unit}.Notify();
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

        new Event_Hit(){Hit = hit, Unit = unit}.Notify();
        new Event_UpdateHP().Notify();

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
            new Event_Dodge(){Unit = player}.Notify();
            GameFacade.Instance.EffectManager.Load(EFFECT.DODGE, player.transform.localPosition, Land.ELEMENT_ROOT.gameObject);
            return;
        }

        var hit = new Hit(enemy);

        //撞击伤害
        player.UpdateHP(-1);

        new Event_Crash(){Caster = enemy, Target = player}.Notify();
        new Event_UpdateHP().Notify();

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
        List<BUFF> buffs = new List<BUFF>()
        {
            BUFF.ATK_UP, BUFF.ATK_DOWN,
            BUFF.ASP_UP, BUFF.ASP_DOWN, 
            BUFF.SPEED_UP, BUFF.SPEED_DOWN,
            BUFF.CP, BUFF.DODGE_UP, BUFF.SPD_MUL
        };

        if (RandomUtility.IsHit((int)Field.Instance.Player.ATT.LUCKY.ToNumber(), 1000) == true)
        {
            buffs.Remove(BUFF.ATK_DOWN);
            buffs.Remove(BUFF.ASP_DOWN);
            buffs.Remove(BUFF.SPEED_DOWN);
        }
        

        int rand    = RandomUtility.Random(0, buffs.Count);
        int buff_id = (int)buffs[rand];
        Field.Instance.PushBuffBubble(buff_id, 1);
    }

    #endregion
}
