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
        Event_BulletCreate.OnEvent += OnBulletCreate;
    }

    public override void Dispose()
    {
        Event_BulletCreate.OnEvent -= OnBulletCreate;
    }

    //创建子弹
    private void OnBulletCreate(Event_BulletCreate e)
    {
        Bullet b = e.Bullet;
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
        Event_BulletCreate.OnEvent += OnBulletCreate;
    }

    public override void Dispose()
    {
        Event_BulletCreate.OnEvent -= OnBulletCreate;
    }

    //创建子弹
    private void OnBulletCreate(Event_BulletCreate e)
    {
        Bullet b = e.Bullet;
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
        Event_BulletShoot.OnEvent += OnBulletShoot;
    }

    public override void Dispose()
    {
        Event_BulletShoot.OnEvent -= OnBulletShoot;
    }

    //发射子弹
    private void OnBulletShoot(Event_BulletShoot e)
    {
        Bullet b = e.Bullet;
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
        Event_BulletHit.OnEvent += OnBulletHit;
    }

    public override void Dispose()
    {
        Event_BulletHit.OnEvent -= OnBulletHit;
    }

    //子弹击中目标
    private void OnBulletHit(Event_BulletHit e)
    {
        Bullet b = e.Bullet;
        if (b.Caster != Caster) return;

        Unit unit = e.Target;

        if (RandomUtility.IsHit(Value))
        {
            unit.AddBuff(Caster, (int)BUFF.STUN, 1, 1f);
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
        Event_BulletHit.OnEvent += OnBulletHit;
    }

    public override void Dispose()
    {
        Event_BulletHit.OnEvent -= OnBulletHit;
    }

    //子弹击中目标
    private void OnBulletHit(Event_BulletHit e)
    {
        Bullet b = e.Bullet;
        if (b.Caster != Caster) return;

        Enemy unit = e.Target as Enemy;

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
        Event_BulletCreate.OnEvent += OnBulletCreate;
    }

    public override void Dispose()
    {
        Event_BulletCreate.OnEvent -= OnBulletCreate;
    }

    //创建子弹
    void OnBulletCreate(Event_BulletCreate e)
    {
        Bullet b = e.Bullet;
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
        Event_Hit.OnEvent += OnHit;
    }

    public override void Dispose()
    {
        Event_Hit.OnEvent -= OnHit;
    }

    //目标受击
    private void OnHit(Event_Hit e)
    {
        Hit h = e.Hit;
        if (h.Caster != Caster) return;

        e.Unit.AddBuff(Caster, (int)BUFF.YISHANG, Value, 3.0f);
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

        new Event_PlayTrap(){Skill = this}.Notify();

        Caster.CreateProjectile(PROJECTILE.POISONWATER, TRACE.PARABOLA, point, 0.4f, ()=>{
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

        new Event_PlayTrap(){Skill = this}.Notify();

        Caster.CreateProjectile(PROJECTILE.ICEWATER, TRACE.PARABOLA, point, 0.4f, ()=>{
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

        new Event_PlayTrap(){Skill = this}.Notify();

        Caster.CreateProjectile(PROJECTILE.ROPE, TRACE.PARABOLA, point, 0.4f, ()=>{
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
        Event_KillEnemy.OnEvent += OnKillEnemy;
    }

    public override void Dispose()
    {
        Event_KillEnemy.OnEvent -= OnKillEnemy;
    }

    //创建子弹
    void OnKillEnemy(Event_KillEnemy e)
    {
        Hit hit = e.Hit;

        if (hit.Caster != Caster) return;

        hit.Caster.AddBuff(Caster, (int)BUFF.FASTSPD, Value);
    }
}
#endregion


#region 弓：杀意
//每击杀一个目标，攻击提高#%
public class Skill_10270 : Skill
{
    public Skill_10270()
    {
        Event_KillEnemy.OnEvent += OnKillEnemy;
    }

    public override void Dispose()
    {
        Event_KillEnemy.OnEvent -= OnKillEnemy;
    }

    //创建子弹
    void OnKillEnemy(Event_KillEnemy e)
    {
        Hit hit = e.Hit;

        if (hit.Caster != Caster) return;
        
        hit.Caster.AddBuff(Caster, (int)BUFF.KILL, Value);
    }
}
#endregion

#region 弓：会心
//每击杀一个目标，暴击率提高#%
public class Skill_10280 : Skill
{
    public Skill_10280()
    {
        Event_KillEnemy.OnEvent += OnKillEnemy;
    }

    public override void Dispose()
    {
        Event_KillEnemy.OnEvent -= OnKillEnemy;
    }

    //创建子弹
    void OnKillEnemy(Event_KillEnemy e)
    {
        Hit hit = e.Hit;

        if (hit.Caster != Caster) return;
        
        hit.Caster.AddBuff(Caster, (int)BUFF.CRIT, Value);
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

//         new Event_UpdateHP().Notify();
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

//         new Event_UpdateHP().Notify();
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

    public void FullCD(float rate = 1.0f)
    {
        // m_Timer.Full();
        m_Timer.SetCurrent(m_Timer.Duration * rate);
    }

    public bool IsLevelMax()
    {
        return Level >= Data.LevelMax;
    }

    public virtual void CustomUpdate(float deltaTime)
    {

    }
}
