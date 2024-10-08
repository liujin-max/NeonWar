using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;




#region 弓：多重射击
public class Skill_10010 : Skill
{
    public override bool OnShoot()
    {
        int BulletCount = Skill.ToValue(Data, Level) + 1;

        switch (BulletCount)
        {
            case 1:
            {
                var bullet = Caster.CreateBullet();
                bullet.Shoot(Caster.GetAngle() + 180);
            }
            break;

            case 2:
            {
                var dir = ToolUtility.AngleToVector(Caster.GetAngle());
                Vector2 shoot_point = Caster.ShootPivot.position;

                var offset1 = shoot_point + new Vector2(-dir.y, dir.x) * 0.25f;
                var offset2 = shoot_point + new Vector2( dir.y, -dir.x) * 0.25f;

                {
                    var bullet = Caster.CreateBullet();
                    bullet.transform.position = offset1;
                    bullet.Shoot(Caster.GetAngle() + 180);
                }

                {
                    var bullet = Caster.CreateBullet();
                    bullet.transform.position = offset2;
                    bullet.Shoot(Caster.GetAngle() + 180);
                }
            }
            break;

            case 3:
            {
                {
                    var bullet = Caster.CreateBullet();
                    bullet.Shoot(Caster.GetAngle() + 180 - 15);
                }

                {
                    var bullet = Caster.CreateBullet();
                    bullet.Shoot(Caster.GetAngle() + 180);
                }

                {
                    var bullet = Caster.CreateBullet();
                    bullet.Shoot(Caster.GetAngle() + 180 + 15);
                }
            }
            break;

            case 4:
            {
                var dir = ToolUtility.AngleToVector(Caster.GetAngle());
                Vector2 shoot_point = Caster.ShootPivot.position;

                var offset1 = shoot_point + new Vector2(-dir.y, dir.x) * 0.15f;
                var offset2 = shoot_point + new Vector2( dir.y, -dir.x) * 0.15f;

                {
                    var bullet = Caster.CreateBullet();
                    bullet.transform.position = offset1;
                    bullet.Shoot(Caster.GetAngle() + 180);
                }

                {
                    var bullet = Caster.CreateBullet();
                    bullet.transform.position = offset2;
                    bullet.Shoot(Caster.GetAngle() + 180);
                }

                {
                    var bullet = Caster.CreateBullet();
                    bullet.transform.position = offset1;
                    bullet.Shoot(Caster.GetAngle() + 180 - 20);
                }

                {
                    var bullet = Caster.CreateBullet();
                    bullet.transform.position = offset2;
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

        List<object> enemy_pool = new List<object>();
        Field.Instance.Spawn.Enemys.ForEach(enemy => {
            if (enemy.gameObject != unit.gameObject) {
                enemy_pool.Add(enemy);
            }
        });
        

        for (int i = 0; i < Skill.ToValue(Data, Level); i++)
        {
            var bullet = Caster.CreateBullet();
            bullet.transform.position = b.transform.position;
            bullet.IsSplit = true;
            bullet.Hit.IgnoreUnits.Add(unit);

            if (enemy_pool.Count > 0)
            {
                Enemy e = RandomUtility.Pick(1, enemy_pool)[0] as Enemy;
                enemy_pool.Remove(e);

                //分裂箭射向其他目标
                bullet.Shoot(ToolUtility.VectorToAngle(e.transform.localPosition - bullet.transform.localPosition));
            }
            else
            {
                bullet.Shoot(RandomUtility.Random(0, 360));
            }
        }
        
    }
}
#endregion

#region 弓：瞄准射击
//箭矢最多可以追踪场上距离最近的#个敌人
public class Skill_10030 : Skill
{
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
        GameObject[] sortedObjects = EnemiesToSort
            .OrderBy(obj => Vector3.Distance(o_pos, obj.transform.localPosition))
            .ToArray();

            
        for (int i = 0; i < Skill.ToValue(Data, Level); i++)
        {
            if (i >= sortedObjects.Length) break;

            Enemy e = sortedObjects[i].GetComponent<Enemy>();

            //向目标发射子弹
            var bullet = Caster.CreateBullet();
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


        b.HitRemaining = 1 + Skill.ToValue(Data, Level);
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


        b.ReboundTimes = Skill.ToValue(Data, Level);
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

        int rate = Skill.ToValue(Data, Level);
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

        if (RandomUtility.IsHit(Skill.ToValue(Data, Level)))
        {
            unit.AddBuff((int)_C.BUFF.STUN, 1);
        }
        
    }
}
#endregion



#region 弓：急速箭矢
//箭矢移动速度提高#%
public class Skill_10130 : Skill
{
    public Skill_10130()
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

        b.Speed.PutAUL(this, Skill.ToValue(Data, Level) / 100.0f);
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

        b.Hit.KillRate = Skill.ToValue(Data, Level);
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

        unit.AddBuff((int)_C.BUFF.YISHANG, Skill.ToValue(Data, Level));
    }
}

#endregion























public class Skill
{
    public SkillData Data;
    public int Level;
    public Unit Caster;

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
    };


    public static Skill Create(SkillData data, Unit caster, int level)
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
        return skillData.Values[level - 1];
    }

    public static string GetDescription(SkillData skillData, int level)
    {
        int value = Skill.ToValue(skillData, level);

        return skillData.Description.Replace("#", value.ToString());
    }

    public static int GetCost(SkillData skillData, int level)
    {
        return skillData.Glass[level - 1];
    }

    public virtual void Dispose()
    {

    }


    public void Init(SkillData data, Unit caster, int level)
    {
        Data    = data;
        Caster  = caster;
        Level   = level;
    }

    public bool IsLevelMax()
    {
        return Level >= Data.LevelMax;
    }

    public virtual bool OnShoot()
    {
        return false;
    }
}
