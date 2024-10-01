using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SocialPlatforms;




#region 弓：多重射击
public class Skill_10010 : Skill
{
    public override void Equip()
    {
        Caster.SetAttackMode(new Attack_Arrow_Normal(Caster, this.Value));
    }
}

#endregion


#region 弓：分裂箭矢
//击中敌人后分裂出#枚箭矢
public class Skill_10020 : Skill
{
    public override void Equip()
    {
        Caster.SetAttackMode(new Attack_Arrow_Split(Caster, this.Value));
    }
}
#endregion



#region 弓：瞄准射击
//箭矢最多可以追踪场上距离最近的#个敌人
public class Skill_10030 : Skill
{
    public override void Equip()
    {
        Caster.SetAttackMode(new Attack_Arrow_Focus(Caster, this.Value));
    }
}
#endregion



#region 弓：穿透
//箭矢可穿透#次
public class Skill_10060 : Skill
{
    public Skill_10060()
    {
        EventManager.AddHandler(EVENT.ONBULLETCREATE,   OnBulletCreate);
    }

    public override void Dispose()
    {
        EventManager.DelHandler(EVENT.ONBULLETCREATE,   OnBulletCreate);
    }

    //创建子弹
    private void OnBulletCreate(GameEvent @event)
    {
        Bullet b = @event.GetParam(0) as Bullet;
        if (b.Caster != Caster) return;


        b.HitRemaining = 1 + Value;
    }
}
#endregion


#region 弓：反弹
//箭矢可反弹#次
public class Skill_10070 : Skill
{
    public Skill_10070()
    {
        EventManager.AddHandler(EVENT.ONBULLETCREATE,   OnBulletCreate);
    }

    public override void Dispose()
    {
        EventManager.DelHandler(EVENT.ONBULLETCREATE,   OnBulletCreate);
    }

    //创建子弹
    private void OnBulletCreate(GameEvent @event)
    {
        Bullet b = @event.GetParam(0) as Bullet;
        if (b.Caster != Caster) return;


        b.ReboundTimes = Value;
    }
}
#endregion





#region 弓：快速射击
//有#%的概率连续发射箭矢
public class Skill_10110 : Skill
{
    public Skill_10110()
    {
        EventManager.AddHandler(EVENT.ONBULLETSHOOT,    OnBulletShoot);
    }

    public override void Dispose()
    {
        EventManager.DelHandler(EVENT.ONBULLETSHOOT,    OnBulletShoot);
    }

    //发射子弹
    private void OnBulletShoot(GameEvent @event)
    {
        Bullet b = @event.GetParam(0) as Bullet;
        if (b.Caster != Caster) return;

        int rate = Value;
        if (RandomUtility.IsHit(rate))
        {
            //缺少特效
            Caster.ASP.SetCurrent(Caster.ASP.Duration * 0.8f);
        }
    }
}
#endregion


#region 弓：晕眩射击
//被击中的敌人有#%的概率晕眩1秒
public class Skill_10120 : Skill
{
    public Skill_10120()
    {
        EventManager.AddHandler(EVENT.ONBULLETHIT,  OnBulletHit);
    }

    public override void Dispose()
    {
        EventManager.DelHandler(EVENT.ONBULLETHIT,  OnBulletHit);
    }

    //子弹击中目标
    private void OnBulletHit(GameEvent @event)
    {
        Bullet b = @event.GetParam(0) as Bullet;
        if (b.Caster != Caster) return;

        Unit unit = @event.GetParam(1) as Unit;

        if (RandomUtility.IsHit(Value))
        {
            unit.AddBuff(Caster, (int)CONST.BUFF.STUN, 1, 1f);
        }
        
    }
}
#endregion



#region 弓：击退箭矢
//箭矢有#%的概率击退目标
public class Skill_10130 : Skill
{
    public Skill_10130()
    {
        EventManager.AddHandler(EVENT.ONBULLETHIT,  OnBulletHit);
    }

    public override void Dispose()
    {
        EventManager.DelHandler(EVENT.ONBULLETHIT,  OnBulletHit);
    }

    //子弹击中目标
    private void OnBulletHit(GameEvent @event)
    {
        Bullet b = @event.GetParam(0) as Bullet;
        if (b.Caster != Caster) return;

        Enemy unit = @event.GetParam(1) as Enemy;

        if (unit == null) return;

        if (RandomUtility.IsHit(Value))
        {
            unit.Repel(b.Velocity.normalized * 8);
        }
        
    }
}
#endregion




#region 弓：必杀
//被击中的敌人有#%的概率直接死亡(精英、Boss除外)
public class Skill_10160 : Skill
{
    public Skill_10160()
    {
        EventManager.AddHandler(EVENT.ONBULLETCREATE,    OnBulletCreate);
    }

    public override void Dispose()
    {
        EventManager.DelHandler(EVENT.ONBULLETCREATE,    OnBulletCreate);
    }

    //创建子弹
    void OnBulletCreate(GameEvent @event)
    {
        Bullet b = @event.GetParam(0) as Bullet;
        if (b.Caster != Caster) return;

        b.Hit.KillRate = Value;
    }
}

#endregion


#region 弓：标记
//被击中的敌人在3秒内受到的伤害提高#%
public class Skill_10170 : Skill
{
    public Skill_10170()
    {
        EventManager.AddHandler(EVENT.ONHIT,    OnHit);
    }

    public override void Dispose()
    {
        EventManager.DelHandler(EVENT.ONHIT,    OnHit);
    }

    //目标受击
    private void OnHit(GameEvent @event)
    {
        Hit h = @event.GetParam(0) as Hit;
        if (h.Caster != Caster) return;

        Unit unit = @event.GetParam(1) as Unit;

        unit.AddBuff(Caster, (int)CONST.BUFF.YISHANG, Value, 3.0f);
    }
}

#endregion



#region 弓：毒气陷阱
//陷阱范围内的目标持续受到伤害，持续#秒
public class Skill_10210 : Skill
{
    public override void Init(SkillData data, Player caster, int level)
    {
        base.Init(data, caster, level);

        m_Timer.Reset(RandomUtility.Random(300, 600) / 100.0f);
    }

    public override void CustomUpdate(float deltaTime)
    {
        m_Timer.Update(deltaTime);
        if (!m_Timer.IsFinished()) return;

        //投掷陷阱
        Enemy target    = Field.Instance.Spawn.FindEnemyGather(1.5f);

        if (target == null) return;

        m_Timer.Reset(RandomUtility.Random(500, 700) / 100.0f);
        
        Vector2 point = target.transform.localPosition;

        EventManager.SendEvent(new GameEvent(EVENT.ONPLAYTRAP, this));

        Caster.CreateProjectile(PROJECTILE.POISONWATER, CONST.TRACE.PARABOLA, point, 0.4f, ()=>{
            Field.Instance.PushArea(Caster, AREA.POISON, point, Value);
        });
    }
}
#endregion


#region 弓：冰冻陷阱
//陷阱范围内的目标行动缓慢，持续#秒
public class Skill_10220 : Skill
{
    public override void Init(SkillData data, Player caster, int level)
    {
        base.Init(data, caster, level);

        m_Timer.Reset(RandomUtility.Random(400, 700) / 100.0f);
    }

    public override void CustomUpdate(float deltaTime)
    {
        m_Timer.Update(deltaTime);
        if (!m_Timer.IsFinished()) return;

        //投掷陷阱
        Enemy target    = Field.Instance.Spawn.FindEnemyGather(1.5f);

        if (target == null) return;

        m_Timer.Reset(RandomUtility.Random(600, 800) / 100.0f);

        //投掷陷阱
        Vector2 point = target.transform.localPosition;

        EventManager.SendEvent(new GameEvent(EVENT.ONPLAYTRAP, this));

        Caster.CreateProjectile(PROJECTILE.ICEWATER, CONST.TRACE.PARABOLA, point, 0.4f, ()=>{
            Field.Instance.PushArea(Caster, AREA.ICE, point, Value);
        });
    }
}
#endregion


#region 弓：引力陷阱
//将范围#内的目标向陷阱中心拖拽
public class Skill_10230 : Skill
{
    public override void Init(SkillData data, Player caster, int level)
    {
        base.Init(data, caster, level);

        m_Timer.Reset(RandomUtility.Random(300, 600) / 100.0f);
    }

    public override void CustomUpdate(float deltaTime)
    {
        m_Timer.Update(deltaTime);
        if (!m_Timer.IsFinished()) return;

        Enemy target    = Field.Instance.Spawn.FindEnemyGather(1.5f);

        if (target == null) return;

        m_Timer.Reset(RandomUtility.Random(500, 700) / 100.0f);

        var point = target.transform.localPosition;

        EventManager.SendEvent(new GameEvent(EVENT.ONPLAYTRAP, this));

        Caster.CreateProjectile(PROJECTILE.ROPE, CONST.TRACE.PARABOLA, point, 0.4f, ()=>{
            Field.Instance.PushArea(Caster, AREA.ROPE, point, Value);
        });
    }
}
#endregion




#region 弓：疾速
//每击杀一个目标，攻速提高#%
public class Skill_10260 : Skill
{
    public Skill_10260()
    {
        EventManager.AddHandler(EVENT.ONKILLENEMY,  OnKillEnemy);
    }

    public override void Dispose()
    {
        EventManager.DelHandler(EVENT.ONKILLENEMY,  OnKillEnemy);
    }

    //创建子弹
    void OnKillEnemy(GameEvent @event)
    {
        Hit hit = (Hit)@event.GetParam(1);

        if (hit.Caster != Caster) return;

        hit.Caster.AddBuff(Caster, (int)CONST.BUFF.FASTSPD, Value);
    }
}
#endregion


#region 弓：杀意
//每击杀一个目标，攻击提高#%
public class Skill_10270 : Skill
{
    public Skill_10270()
    {
        EventManager.AddHandler(EVENT.ONKILLENEMY,  OnKillEnemy);
    }

    public override void Dispose()
    {
        EventManager.DelHandler(EVENT.ONKILLENEMY,  OnKillEnemy);
    }

    //创建子弹
    void OnKillEnemy(GameEvent @event)
    {
        Hit hit = (Hit)@event.GetParam(1);

        if (hit.Caster != Caster) return;
        
        hit.Caster.AddBuff(Caster, (int)CONST.BUFF.KILL, Value);
    }
}
#endregion

#region 弓：会心
//每击杀一个目标，暴击率提高#%
public class Skill_10280 : Skill
{
    public Skill_10280()
    {
        EventManager.AddHandler(EVENT.ONKILLENEMY,  OnKillEnemy);
    }

    public override void Dispose()
    {
        EventManager.DelHandler(EVENT.ONKILLENEMY,  OnKillEnemy);
    }

    //创建子弹
    void OnKillEnemy(GameEvent @event)
    {
        Hit hit = (Hit)@event.GetParam(1);

        if (hit.Caster != Caster) return;
        
        hit.Caster.AddBuff(Caster, (int)CONST.BUFF.CRIT, Value);
    }
}
#endregion












#region ====================== 通用技能 ======================

// #region 增强体质
// //提高1点生命上限
// public class Skill_99901 : Skill
// {
//     public override void Equip()
//     {
//         Caster.ATT.HPMAX++;
//         Caster.UpdateHP(1);

//         EventManager.SendEvent(new GameEvent(EVENT.ONHPUPDATE));
//     }
// }
// #endregion


// #region 治疗
// //恢复50%的生命值
// public class Skill_99902 : Skill
// {
//     public override void Equip()
//     {
//         int value = Mathf.CeilToInt(Caster.ATT.HPMAX / 2.0f);
//         Caster.UpdateHP(value);

//         EventManager.SendEvent(new GameEvent(EVENT.ONHPUPDATE));
//     }
// }
// #endregion

#endregion



















public class Skill
{
    public SkillData Data;
    public int ID {get {return Data.ID;}}
    public int Level;
    public Player Caster;

    public int Value {get {return Data.GetValue(Level);}}

    protected CDTimer m_Timer = new CDTimer(0);

    private static Dictionary<int, Func<Skill>> m_classDictionary = new Dictionary<int, Func<Skill>> {
        //弓 攻击
        {10010, () => new Skill_10010()},
        {10020, () => new Skill_10020()},
        {10030, () => new Skill_10030()},

        {10060, () => new Skill_10060()},
        {10070, () => new Skill_10070()},
        

        //弓 攻速
        {10110, () => new Skill_10110()},
        {10120, () => new Skill_10120()},
        {10130, () => new Skill_10130()},

        {10160, () => new Skill_10160()},
        {10170, () => new Skill_10170()},
        
        //弓 价值
        {10210, () => new Skill_10210()},
        {10220, () => new Skill_10220()},
        {10230, () => new Skill_10230()},


        {10260, () => new Skill_10260()},
        {10270, () => new Skill_10270()},
        {10280, () => new Skill_10280()},




        //通用
        // {99901, () => new Skill_99901()},
        // {99902, () => new Skill_99902()},
    };


    public static Skill Create(SkillData data, Player caster, int level)
    {
        Skill skill;
        if (m_classDictionary.ContainsKey(data.ID)) skill = m_classDictionary[data.ID]();
        else {
            skill = new Skill();

            Debug.LogError("未实现的技能：" + data.ID);
        }

        skill.Init(data, caster, level);

        return skill;
    }

    public virtual void Dispose()
    {

    }


    public virtual void Init(SkillData data, Player caster, int level)
    {
        Data    = data;
        Caster  = caster;
        Level   = level;
    }

    //穿戴技能
    public virtual void Equip()
    {

    }

    public void FullCD()
    {
        m_Timer.Full();
    }

    public bool IsLevelMax()
    {
        return Level >= Data.LevelMax;
    }

    public virtual void CustomUpdate(float deltaTime)
    {

    }
}
