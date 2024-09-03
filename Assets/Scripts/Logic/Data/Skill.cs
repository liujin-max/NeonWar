using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SocialPlatforms;




#region 弓：多重射击
public class Skill_10010 : Skill
{
    public override bool OnShoot()
    {
        int BulletCount = this.Value + 1;

        switch (BulletCount)
        {
            case 1:
            {
                var bullet = Field.Instance.CreateBullet(Caster);
                bullet.Shoot(Caster.GetAngle() + 180);
            }
            break;

            case 2:
            {
                var dir = ToolUtility.AngleToVector(Caster.GetAngle());
                Vector2 shoot_point = Caster.ShootPivot.position;

                {
                    var bullet = Field.Instance.CreateBullet(Caster);
                    bullet.transform.position = shoot_point + new Vector2(-dir.y, dir.x) * 0.25f;
                    bullet.Shoot(Caster.GetAngle() + 180);
                }

                {
                    var bullet = Field.Instance.CreateBullet(Caster);
                    bullet.transform.position = shoot_point + new Vector2( dir.y, -dir.x) * 0.25f;
                    bullet.Shoot(Caster.GetAngle() + 180);
                }
            }
            break;

            case 3:
            {
                var dir = ToolUtility.AngleToVector(Caster.GetAngle());
                Vector2 shoot_point = Caster.ShootPivot.position;

                {
                    var bullet = Field.Instance.CreateBullet(Caster);
                    bullet.transform.position = shoot_point;
                    bullet.Shoot(Caster.GetAngle() + 180);
                }

                {
                    var bullet = Field.Instance.CreateBullet(Caster);
                    bullet.transform.position = shoot_point + new Vector2(-dir.y, dir.x) * 0.35f;
                    bullet.Shoot(Caster.GetAngle() + 180);
                }

                {
                    var bullet = Field.Instance.CreateBullet(Caster);
                    bullet.transform.position = shoot_point + new Vector2( dir.y, -dir.x) * 0.35f;
                    bullet.Shoot(Caster.GetAngle() + 180);
                }
            }
            break;

            case 4:
            {
                var dir = ToolUtility.AngleToVector(Caster.GetAngle());
                Vector2 shoot_point = Caster.ShootPivot.position;
                
                {
                    var bullet = Field.Instance.CreateBullet(Caster);
                    bullet.transform.position = shoot_point + new Vector2(-dir.y, dir.x) * 0.25f;
                    bullet.Shoot(Caster.GetAngle() + 180);
                }

                {
                    var bullet = Field.Instance.CreateBullet(Caster);
                    bullet.transform.position = shoot_point + new Vector2(-dir.y, dir.x) * 0.25f;
                    bullet.Shoot(Caster.GetAngle() + 180 - 15);
                }

                {
                    var bullet = Field.Instance.CreateBullet(Caster);
                    bullet.transform.position = shoot_point + new Vector2( dir.y, -dir.x) * 0.25f;
                    bullet.Shoot(Caster.GetAngle() + 180);
                }

                {
                    var bullet = Field.Instance.CreateBullet(Caster);
                    bullet.transform.position = shoot_point + new Vector2( dir.y, -dir.x) * 0.25f;
                    bullet.Shoot(Caster.GetAngle() + 180 + 15);
                }
            }
            break;

            case 5:
            {
                var dir = ToolUtility.AngleToVector(Caster.GetAngle());
                Vector2 shoot_point = Caster.ShootPivot.position;

                {
                    var bullet = Field.Instance.CreateBullet(Caster);
                    bullet.transform.position = shoot_point;
                    bullet.Shoot(Caster.GetAngle() + 180);
                }

                {
                    var bullet = Field.Instance.CreateBullet(Caster);
                    bullet.transform.position = shoot_point + new Vector2(-dir.y, dir.x) * 0.35f;
                    bullet.Shoot(Caster.GetAngle() + 180);
                }

                {
                    var bullet = Field.Instance.CreateBullet(Caster);
                    bullet.transform.position = shoot_point + new Vector2( dir.y, -dir.x) * 0.35f;
                    bullet.Shoot(Caster.GetAngle() + 180);
                }


                {
                    var bullet = Field.Instance.CreateBullet(Caster);
                    bullet.transform.position = shoot_point + new Vector2(-dir.y, dir.x) * 0.35f;
                    bullet.Shoot(Caster.GetAngle() + 180 - 20);
                }

                {
                    var bullet = Field.Instance.CreateBullet(Caster);
                    bullet.transform.position = shoot_point + new Vector2( dir.y, -dir.x) * 0.35f;
                    bullet.Shoot(Caster.GetAngle() + 180 + 20);
                }
            }
            break;
        }


        return true;
    }
}

#endregion

#region 弓：分裂箭矢
//击中敌人后分裂出#枚箭矢
public class Skill_10020 : Skill
{
    public Skill_10020()
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
        if (b.IsSplit == true) return;   //分裂出的箭矢也能继续分裂？

        Unit unit = @event.GetParam(1) as Unit;

        for (int i = 0; i < this.Value; i++)
        {
            var bullet = Field.Instance.CreateBullet(Caster);
            bullet.transform.position = b.transform.position;
            bullet.IsSplit = true;
            bullet.Hit.IgnoreUnits.Add(unit);

            bullet.Shoot(RandomUtility.Random(0, 360));
        }
        
    }
}
#endregion

#region 弓：精准分裂
//分裂出的箭矢会自动瞄准附近的敌人
public class Skill_10021 : Skill
{
    public Skill_10021()
    {
        EventManager.AddHandler(EVENT.ONBULLETSHOOT,  OnBulletShoot);
    }

    public override void Dispose()
    {
        EventManager.DelHandler(EVENT.ONBULLETSHOOT,  OnBulletShoot);
    }

    //子弹击中目标
    private void OnBulletShoot(GameEvent @event)
    {
        Bullet b = @event.GetParam(0) as Bullet;
        if (b.Caster != Caster) return;
        if (b.IsSplit == false) return;   //只影响分裂出的箭矢


        if (Field.Instance.Spawn.Enemys.Count == 0) return;

        List<Enemy> enemys = new List<Enemy>(Field.Instance.Spawn.Enemys);
        foreach (var unit in b.Hit.IgnoreUnits) {
            enemys.Remove(unit as Enemy);
        }
    

        int rand= RandomUtility.Random(0, Field.Instance.Spawn.Enemys.Count);
        Enemy e = Field.Instance.Spawn.Enemys[rand];

        //分裂箭射向其他目标
        b.Turn(ToolUtility.VectorToAngle(e.transform.localPosition - b.transform.localPosition)); 
    }
}
#endregion


#region 弓：瞄准射击
//箭矢最多可以追踪场上距离最近的#个敌人
public class Skill_10030 : Skill
{
    public override bool Useable()
    {
        foreach (var e in Field.Instance.Spawn.Enemys) {
            if (e.IsValid) return true;
        }

        return false;
    }

    public override bool OnShoot()
    {
        List<GameObject> enemys = new List<GameObject>();
        Field.Instance.Spawn.Enemys.ForEach(e => {
            if (e.IsValid) enemys.Add(e.gameObject);
        });

        GameObject[] EnemiesToSort = enemys.ToArray();

        //获取自身的位置
        Vector3 o_pos = Caster.transform.localPosition;
        
        //使用 LINQ 根据距离进行排序
        GameObject[] sortedObjects = EnemiesToSort.OrderBy(obj => Vector3.Distance(o_pos, obj.transform.localPosition)).ToArray();

        
        for (int i = 0; i < Value; i++)
        {
            if (i >= sortedObjects.Length) break;

            Enemy e = sortedObjects[i].GetComponent<Enemy>();

            //向目标发射子弹
            var bullet = Field.Instance.CreateBullet(Caster);
            bullet.Shoot(ToolUtility.VectorToAngle(e.transform.localPosition - Caster.transform.localPosition));
        }


        return true;
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
//被击中的敌人有#%的概率晕眩0.5秒
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
            unit.AddBuff((int)_C.BUFF.STUN, 1);
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
//被击中的敌人在5秒内受到的伤害提高#%
public class Skill_10170 : Skill
{
    public Skill_10170()
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

        unit.AddBuff((int)_C.BUFF.YISHANG, Value);
    }
}

#endregion



#region 弓：毒气陷阱
//陷阱范围内的目标持续受到伤害，持续#秒
public class Skill_10210 : Skill
{
    private CDTimer m_Timer = new CDTimer(RandomUtility.Random(300, 600) / 100.0f);
    public override void CustomUpdate(float deltaTime)
    {
        m_Timer.Update(deltaTime);
        if (!m_Timer.IsFinished()) return;

        //投掷陷阱
        Enemy target    = Field.Instance.Spawn.FindEnemyGather(1.5f);

        if (target == null) return;

        m_Timer.Reset(6f);
        
        Vector2 point = target.transform.localPosition;

        Caster.CreateProjectile(PROJECTILE.POISONWATER, _C.TRACE.PARABOLA, point, 0.4f, ()=>{
            Field.Instance.PushArea(Caster, AREA.POISON, point, Value);
        });
    }
}
#endregion

#region 弓：冰冻陷阱
//陷阱范围内的目标行动缓慢，持续#秒
public class Skill_10220 : Skill
{
    private CDTimer m_Timer = new CDTimer(RandomUtility.Random(300, 600) / 100.0f);
    public override void CustomUpdate(float deltaTime)
    {
        m_Timer.Update(deltaTime);
        if (!m_Timer.IsFinished()) return;

        //投掷陷阱
        Enemy target    = Field.Instance.Spawn.FindEnemyGather(1.5f);

        if (target == null) return;

        m_Timer.Reset(6f);

        //投掷陷阱
        Vector2 point = target.transform.localPosition;

        Caster.CreateProjectile(PROJECTILE.ICEWATER, _C.TRACE.PARABOLA, point, 0.4f, ()=>{
            Field.Instance.PushArea(Caster, AREA.ICE, point, Value);
        });
    }
}
#endregion

#region 弓：引力陷阱
//将范围#内的目标向陷阱中心拖拽
public class Skill_10230 : Skill
{
    private CDTimer m_Timer = new CDTimer(RandomUtility.Random(300, 600) / 100.0f);
    public override void CustomUpdate(float deltaTime)
    {
        m_Timer.Update(deltaTime);
        if (!m_Timer.IsFinished()) return;

        Enemy target    = Field.Instance.Spawn.FindEnemyGather(1.5f);

        if (target == null) return;

        m_Timer.Reset(6f);

        var point = target.transform.localPosition;

        Caster.CreateProjectile(PROJECTILE.ROPE, _C.TRACE.PARABOLA, point, 0.4f, ()=>{
            Field.Instance.PushArea(Caster, AREA.ROPE, point, Value);
        });
    }
}
#endregion

#region 弓：扩大陷阱
//陷阱的范围增加至#
public class Skill_10240 : Skill
{
    public Skill_10240()
    {
        EventManager.AddHandler(EVENT.ONPUSHAREA,   OnPushArea);
    }

    public override void Dispose()
    {
        EventManager.DelHandler(EVENT.ONPUSHAREA,   OnPushArea);
    }

    private void OnPushArea(GameEvent @event)
    {
        Area area = @event.GetParam(0) as Area;

        if (area.transform.GetComponent<Area_Poison>() != null 
            || area.transform.GetComponent<Area_Ice>() != null 
            || area.transform.GetComponent<Area_Rope>() != null)
        {
            float radius    = Value / 100.0f;
            area.transform.localScale = new Vector3(radius, radius, 1);
        }
    }
}
#endregion


#region 弓：急速陷阱
//陷阱的冷却速度提高#%
public class Skill_10250 : Skill
{

    public override void CustomUpdate(float deltaTime)
    {
        foreach (var sk in Caster.Skills)
        {
            if (sk.ID == 10210 || sk.ID == 10220 || sk.ID == 10230)
            {
                sk.CustomUpdate(deltaTime * Value / 100.0f);
            }
        }
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

        hit.Caster.AddBuff((int)_C.BUFF.FASTSPD, Value);
        
        // m_KillCount++;

        // float value = Value / 100.0f;
        // hit.Caster.CPS.PutAUL(this, value * m_KillCount);
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
        
        hit.Caster.AddBuff((int)_C.BUFF.KILL, Value);
    }
}
#endregion














public class Skill
{
    public SkillData Data;
    public int ID {get {return Data.ID;}}
    public int Level;
    public Player Caster;

    public int Value {get {return Skill.ToValue(Data, Level);}}

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
        {10240, () => new Skill_10240()},
        {10250, () => new Skill_10250()},

        {10260, () => new Skill_10260()},
        {10270, () => new Skill_10270()},
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

    //参数
    public static int ToValue(SkillData skillData, int level)
    {
        return skillData.Values[Mathf.Min(skillData.LevelMax - 1, level - 1)];
    }

    public static string GetDescription(SkillData skillData, int level, string color = "<#FFFFFF>")
    {
        int value = Skill.ToValue(skillData, Mathf.Min(skillData.LevelMax, level));

        return skillData.Description.Replace("#", (color + value + "</color>"));
    }

    public static string CompareDescription(SkillData skillData, int o_level, int t_level)
    {
        if (o_level == 0) return GetDescription(skillData, t_level, _C.COLOR_GREEN);
        if (o_level == skillData.LevelMax) return GetDescription(skillData, o_level, _C.COLOR_GREEN);

        int o_value = Skill.ToValue(skillData, Mathf.Min(skillData.LevelMax, o_level));
        int t_value = Skill.ToValue(skillData, Mathf.Min(skillData.LevelMax, t_level));

        string str = _C.COLOR_GREEN + o_value + "</color>->" + _C.COLOR_GREEN + t_value + "</color>";

        return skillData.Description.Replace("#", str);
    }

    public static int GetCost(SkillData skillData, int level)
    {
        // return skillData.Glass[Mathf.Min(skillData.LevelMax - 1, level)];
        return 0;
    }

    public virtual void Dispose()
    {

    }


    public void Init(SkillData data, Player caster, int level)
    {
        Data    = data;
        Caster  = caster;
        Level   = level;
    }

    public virtual void Upgrade(int level)
    {
        Level   = level;
    }

    public bool IsLevelMax()
    {
        return Level >= Data.LevelMax;
    }

    public virtual bool Useable()
    {
        return true;
    }

    public virtual bool OnShoot()
    {
        return false;
    }

    public virtual void CustomUpdate(float deltaTime)
    {

    }
}
