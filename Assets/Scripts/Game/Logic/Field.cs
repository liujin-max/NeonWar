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

    private List<Bullet> m_Bullets = new List<Bullet>();
    public List<Bullet> Bullets {get {return m_Bullets;}}


    public CycleList<BuffBubble> BuffBubbles = new CycleList<BuffBubble>(null);
    public CycleList<Area> Areas = new CycleList<Area>((a)=>{ EventManager.SendEvent(new GameEvent(EVENT.ONREMOVEAREA, a));});
    


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
            new State_Upgrade<Field>(_C.FSMSTATE.UPGRADE),
            new State_Result<Field>(_C.FSMSTATE.RESULT),
        });
    }

    public void Enter()
    {
        STATE   = _C.GAME_STATE.PAUSE;

        Land    = new Land();
        m_Spawn = new Spawn();


        m_FSM.Transist(_C.FSMSTATE.IDLE);
    }

    //开始游玩
    public void Play(int level_id)
    {
        //玩家开始触摸后，再转换状态
        // STATE   = _C.GAME_STATE.PLAY;


        m_Level = DataCenter.Instance.Levels.GetLevel(level_id);
        m_Level.Init();
        m_Spawn.Init(m_Level.LevelJSON);

        Debug.Log("========  开始关卡：" + level_id + "  ========");

        //将最新的加成等级应用到Player身上
        InitPlayer();

        m_FSM.Transist(_C.FSMSTATE.PLAY);


        GameFacade.Instance.UIManager.LoadWindowAsync("BattleWindow", UIManager.BOTTOM, (obj)=>{
            obj.GetComponent<BattleWindow>().Init();
        });

        EventManager.SendEvent(new GameEvent(EVENT.ONBATTLESTART));
    }

    //结束游玩
    public void End()
    {
        STATE   = _C.GAME_STATE.PAUSE;

        Land.Dispose();
        m_Spawn.Dispose();


        BuffBubbles.Dispose();
        Areas.Dispose();


        RemovePlayer();

        GameFacade.Instance.UIManager.UnloadWindow("BattleWindow");

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
        if (m_FSM == null || m_FSM.CurrentState == null) return _C.FSMSTATE.IDLE;

        return m_FSM.CurrentState.ID;
    }

    public void NextWave()
    {
        m_Spawn.NextWave();

        EventManager.SendEvent(new GameEvent(EVENT.ONNEXTWAVE));

        m_FSM.Transist(_C.FSMSTATE.PLAY);
    }


    void Update()
    {
        if (this.STATE != _C.GAME_STATE.PLAY) return;

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

    //每波结束时 做一些战场清理的操作
    public void ClearFieldWhenWaveFinished()
    {
        //释放Buff等
        m_Player.ClearBuffs();

        //
        for (int i = m_Bullets.Count - 1; i >= 0; i--) m_Bullets[i].Dispose();
        m_Bullets.Clear();
        //
        Areas.Clear();
        BuffBubbles.Clear();
    }

    public _C.RESULT CheckResult()
    {
        if (m_Spawn.IsClear() == true) return _C.RESULT.VICTORY;
        if (m_Player.IsDead() == true) return _C.RESULT.LOSE;
        if (m_Spawn.IsCurrentClear() == true) return _C.RESULT.UPGRADE;

        return _C.RESULT.NONE;
    }

    #region 逻辑处理

    
    //根据当前的击杀进度计算出可以获得多少碎片奖励
    public void GenerateGlassRewards(out int base_value, out int worth_value)
    {
        float kill_rate = m_Spawn.KillProgress;

        base_value = Mathf.FloorToInt(m_Level.LevelJSON.Glass * kill_rate) ;

        //额外倍率
        worth_value = Mathf.CeilToInt(base_value * m_Player.GlassRate.ToNumber()) - base_value;
    }

    //计算宝珠奖励
    public void GeneratePearRewards(out Dictionary<int , int> pear_dic)
    {
        pear_dic  = new Dictionary<int, int>();

        if (m_Level.LevelJSON.PearPool == 0) {
            return;
        }

        int[] section   = m_Level.LevelJSON.PearCount.Split("-").Select(int.Parse).ToArray();
        int count       = RandomUtility.Random(section[0], section[1] + 1);
        int[] pool      = DataCenter.Instance.Backpack.PearPools[m_Level.LevelJSON.PearPool];


        for (int i = 0; i < count; i++)
        {
            int rand    = RandomUtility.Random(0, pool.Length);
            int id      = pool[rand];

            if (!pear_dic.ContainsKey(id)) {
                pear_dic.Add(id, 0);
            }
            pear_dic[id] ++;
        }
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
        var point   = ToolUtility.FindPointOnCircle(Vector2.zero, _C.DEFAULT_RADIUS, RandomUtility.Random(0, 360));

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

            // m_Areas.Add(area);
            Areas.Add(area);

            EventManager.SendEvent(new GameEvent(EVENT.ONPUSHAREA, area));
        });
    }

    // public void RemoveArea(Area area)
    // {
    //     area.Dispose();
    //     m_Areas.Remove(area);

    //     EventManager.SendEvent(new GameEvent(EVENT.ONREMOVEAREA, area));
    // }


    //击中目标
    public bool SettleHit(Hit hit, Unit unit)
    {
        if (this.STATE != _C.GAME_STATE.PLAY) return false;
        //无敌了
        if (unit.IsInvincible() == true) return false;
        if (unit.IsDead()) return false;
        if (hit.IgnoreUnits.Contains(unit)) return false;
        

        float demage = Mathf.Ceil(hit.ATK.ToNumber(false) * hit.ATK_INC.ToNumber() * unit.ATT.VUN_INC.ToNumber());

        //护盾
        if (hit.Type == _C.HIT_TYPE.NORMAL && unit.GetBuff((int)_C.BUFF.SHIELD) != null)
        {
            demage = 0;
        }
        else if (hit.Type == _C.HIT_TYPE.NORMAL && RandomUtility.IsHit((int)unit.ATT.DODGE.ToNumber(), 1000) == true) //闪避了
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

                //必杀特效
                hit.IsHitKill = true;
                GameFacade.Instance.EffectManager.Load(EFFECT.CRITKILL, unit.transform.localPosition, Land.ELEMENT_ROOT.gameObject);
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
            //闪避特效
            return;
        }

        var hit = new Hit(enemy);

        //撞击伤害
        player.UpdateHP(-1);

        EventManager.SendEvent(new GameEvent(EVENT.ONHPUPDATE));

        //受击表现
        if (player.IsDead() == true)
        {
            player.Dead();
        }
        else
        {
            player.Affected();
        }
    }

    #endregion
}
