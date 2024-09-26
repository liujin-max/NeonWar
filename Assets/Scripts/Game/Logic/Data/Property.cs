using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#region 伤害加成
public class Property_ATK : Property
{
    public override void Equip()
    {
        Pear.Belong.ATT.ATK_INC.PutADD(this, Value / 100f);
    }

    public override void UnEquip()
    {
        Pear.Belong.ATT.ATK_INC.Pop(this);
    }
}
#endregion


#region 攻速百分比
public class Property_ASP : Property
{
    public override void Equip()
    {
        Pear.Belong.CPS.PutAUL(this, Value / 100f);
    }

    public override void UnEquip()
    {
        Pear.Belong.CPS.Pop(this);
    }
}
#endregion


#region 生命
public class Property_HP : Property
{
    public override void Equip()
    {
        Pear.Belong.ATT.HPMAX += Value;
        Pear.Belong.ATT.HP += Value;
    }

    public override void UnEquip()
    {
        Pear.Belong.ATT.HPMAX -= Value;
        Pear.Belong.ATT.HP -= Value;
    }
}
#endregion


#region 暴击率
public class Property_Crit : Property
{
    public override void Equip()
    {
        Pear.Belong.ATT.CP.PutADD(this, Value * 10);
    }

    public override void UnEquip()
    {
        Pear.Belong.ATT.CP.Pop(this);
    }
}
#endregion


#region 暴击伤害
public class Property_CritDemage : Property
{
    public override void Equip()
    {
        Pear.Belong.ATT.CT.PutADD(this, Value * 10);
    }

    public override void UnEquip()
    {
        Pear.Belong.ATT.CT.Pop(this);
    }
}
#endregion


#region 移动速度
public class Property_Speed : Property
{
    public override void Equip()
    {
        Pear.Belong.ATT.SPEED.PutADD(this, Value);
    }

    public override void UnEquip()
    {
        Pear.Belong.ATT.SPEED.Pop(this);
    }
}
#endregion


#region 闪避
// 武器闪避率提高#%
public class Property_Dodge : Property
{
    public override void Equip()
    {
        Pear.Belong.ATT.DODGE.PutADD(this, Value * 10);
    }

    public override void UnEquip()
    {
        Pear.Belong.ATT.DODGE.Pop(this);
    }
}
#endregion


#region 箭矢伤害加成
public class Property_Arrow : Property
{
    public override void Equip()
    {
        EventManager.AddHandler(EVENT.ONBULLETCREATE,   OnBulletCreate);
    }

    public override void UnEquip()
    {
        EventManager.DelHandler(EVENT.ONBULLETCREATE,   OnBulletCreate);
    }

    private void OnBulletCreate(GameEvent @event)
    {
        Bullet bullet = @event.GetParam(0) as Bullet;

        if (bullet.Caster.ID == (int)CONST.PLAYER.BOW)
        {
            bullet.Hit.ATK_INC.PutAUL(this, Value / 100.0f);
        }
    }
}
#endregion


#region 增益时间提升
public class Property_Buff : Property
{
    public override void Equip()
    {
        EventManager.AddHandler(EVENT.ONBUFFADD,    OnBuffAdd);
    }

    public override void UnEquip()
    {
        EventManager.AddHandler(EVENT.ONBUFFADD,    OnBuffAdd);
    }

    private void OnBuffAdd(GameEvent @event)
    {
        Buff buff = @event.GetParam(0) as Buff;

        if (buff.Belong != Pear.Belong) return;

        if (buff.TYPE == CONST.BUFF_TYPE.GAIN)
        {
            buff.Duration.SetDuration(buff.Duration.Duration * (1 + Value / 100.0f));
        }
    }
}
#endregion


#region 头目伤害加成
// 对头目造成的伤害提高#%
public class Property_Boss : Property
{
    public override void Equip()
    {
        Pear.Belong.ATT.BOSS_INC.PutAUL(this, Value / 100.0f);
    }

    public override void UnEquip()
    {
        Pear.Belong.ATT.BOSS_INC.Pop(this);
    }
}
#endregion






#region 陷阱的范围增加#
public class Property_AreaRange : Property
{
    public override void Equip()
    {
        EventManager.AddHandler(EVENT.ONPUSHAREA,   OnPushArea);
    }

    public override void UnEquip()
    {
        EventManager.DelHandler(EVENT.ONPUSHAREA,   OnPushArea);
    }

    private void OnPushArea(GameEvent @event)
    {
        Area area = @event.GetParam(0) as Area;

        if (area.Belong != Pear.Belong) return;

        if (area.transform.GetComponent<Area_Poison>() != null || area.transform.GetComponent<Area_Ice>() != null  || area.transform.GetComponent<Area_Rope>() != null)
        {
            float radius    = 1 + Value / 100.0f;
            area.transform.localScale = new Vector3(radius, radius, 1);
        }
    }
}

#endregion 


#region 陷阱的冷却速度提高#%
public class Property_AreaASP : Property
{
    public override void CustomUpdate(float deltaTime)
    {
        foreach (var sk in Pear.Belong.Skills)
        {
            if (sk.ID == 10210 || sk.ID == 10220 || sk.ID == 10230)
            {
                sk.CustomUpdate(deltaTime * Value / 100.0f);
            }
        }
    }
}
#endregion


#region 投掷陷阱时有#%的概率立即刷新冷却时间
public class Property_AreaReset : Property
{
    public override void Equip()
    {
        EventManager.AddHandler(EVENT.ONPLAYTRAP,   OnPlayTrap);
    }

    public override void UnEquip()
    {
        EventManager.DelHandler(EVENT.ONPLAYTRAP,   OnPlayTrap);
    }

    private void OnPlayTrap(GameEvent @event)
    {
        Skill sk = @event.GetParam(0) as Skill;

        if (sk.Caster != Pear.Belong) return;

        if (!RandomUtility.IsHit(Value)) return;

        sk.FullCD();
    }
}
#endregion


#region 分裂后的小箭矢会自动锁定附近敌人
public class Property_SplitFocus : Property
{
    public override void Equip()
    {
        EventManager.AddHandler(EVENT.ONBULLETSHOOT,  OnBulletShoot);
    }

    public override void UnEquip()
    {
        EventManager.DelHandler(EVENT.ONBULLETSHOOT,  OnBulletShoot);
    }

    //子弹击中目标
    private void OnBulletShoot(GameEvent @event)
    {
        Bullet b = @event.GetParam(0) as Bullet;
        if (b.Caster != Pear.Belong) return;
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


#region 毒气陷阱内的目标将周期性受到#层中毒效果
public class Property_AreaPoison : Property
{
    private Dictionary<Area_Poison, float> m_Records = new Dictionary<Area_Poison, float>();
    private List<Area_Poison> m_Areas = new List<Area_Poison>();
    
    public override void Equip()
    {
        EventManager.AddHandler(EVENT.ONPUSHAREA,   OnPushArea);
        EventManager.AddHandler(EVENT.ONREMOVEAREA, OnRemoveArea);
    }

    public override void UnEquip()
    {
        EventManager.DelHandler(EVENT.ONPUSHAREA,   OnPushArea);
        EventManager.DelHandler(EVENT.ONREMOVEAREA, OnRemoveArea);
    }

    private void OnPushArea(GameEvent @event)
    {
        Area_Poison area = @event.GetParam(0) as Area_Poison;
        
        if (area == null) return;
        if (area.Belong != Pear.Belong) return;

        m_Records.Add(area, 0);
        m_Areas.Add(area);
    }

    private void OnRemoveArea(GameEvent @event)
    {
        Area_Poison area = @event.GetParam(0) as Area_Poison;
        
        if (area == null) return;
        if (area.Belong != Pear.Belong) return;

        m_Records.Remove(area);
        m_Areas.Remove(area);
    }

    public override void CustomUpdate(float deltaTime)
    {
        var caster = Pear.Belong;
        foreach (var area in m_Areas)
        {
            m_Records[area] += deltaTime;

            if (m_Records[area] >= 1f)
            {
                m_Records[area] -= 1f;

                foreach (var t in area.Units)
                {
                    t.Key.AddBuff(caster, (int)CONST.BUFF.POISON, (int)caster.ATT.ATK.GetBase(), 0, Value);
                }
            }
        }
    }
}
#endregion



#region 在低温陷阱内停留超过3秒将被冻结
public class Property_AreaIceFrozen : Property
{
    float ToValue()
    {
        float time = (300 - Value) / 100.0f;
        return time;
    }

    public override void CustomUpdate(float deltaTime)
    {
        float time = ToValue();

        foreach (var area in Field.Instance.Areas.List)
        {
            Area_Ice ice = area as Area_Ice;
            if (ice == null) continue;

            foreach (var t in ice.Units)
            {
                if (t.Value >= time) 
                {
                    t.Key.AddBuff(Pear.Belong, (int)CONST.BUFF.FROZEN, 1, 1.5f);
                }
            }
        }
    }

    public override string GetDescription()
    {
        return Model.Description.Replace("#", ToValue().ToString("F1"));
    }
}
#endregion


#region 目标被拖拽至引力陷阱中心时产生#码范围的爆炸伤害
public class Property_AreaBomb : Property
{
    private Dictionary<Area, HashSet<Unit>> m_Records = new Dictionary<Area, HashSet<Unit>>();

    public override void CustomUpdate(float deltaTime)
    {
        foreach (var area in Field.Instance.Areas.List)
        {
            Area_Rope rope = area as Area_Rope;
            if (rope == null) continue;

            if (!m_Records.ContainsKey(rope)) m_Records[rope] = new HashSet<Unit>();

            Vector2 o_pos = rope.transform.localPosition;
            foreach (var t in rope.Units)
            {
                var unit = t.Key;
                if (m_Records[rope].Contains(unit)) continue;
                if (Vector2.Distance(unit.transform.localPosition, o_pos) <= 0.5f) 
                {
                    m_Records[rope].Add(unit);

                    Bomb bomb = new Bomb(Pear.Belong, o_pos, 1 + Value / 100.0f, 2f, EFFECT.ROPE);
                    bomb.Do();
                }
            }
        }
    }

    public override string GetDescription()
    {
        return Model.Description.Replace("#", (100 + Value).ToString());
    }
}
#endregion


#region 触发暴击时提高#%暴击伤害
public class Property_CritPoint : Property
{
    public override void Equip()
    {
        EventManager.AddHandler(EVENT.ONHIT,   OnHit);
    }

    public override void UnEquip()
    {
        EventManager.DelHandler(EVENT.ONHIT,   OnHit);
    }

    //目标受击
    private void OnHit(GameEvent @event)
    {
        Hit h = @event.GetParam(0) as Hit;
        if (h.Caster != Pear.Belong) return;

        if (h.IsCrit == true)
        {
            var caster = Pear.Belong;
            caster.AddBuff(caster, (int)CONST.BUFF.CRITDEMAGE, Value);
        }
    }
}
#endregion


#region 触发闪避时有#%的概率恢复1点生命
public class Property_DodgeHeal : Property
{
    public override void Equip()
    {
        EventManager.AddHandler(EVENT.ONDODGE,  OnDodge);
    }

    public override void UnEquip()
    {
        EventManager.DelHandler(EVENT.ONDODGE,  OnDodge);
    }

    //目标受击
    private void OnDodge(GameEvent @event)
    {
        Unit u = @event.GetParam(0) as Unit;
        if (u != Pear.Belong) return;

        if (RandomUtility.IsHit(Value) == true)
        {
            Field.Instance.Heal(u, 1);
        }
    }
}
#endregion










//道具词条
public class Property
{
    #region 逻辑配置
    private static Dictionary<int, Func<Property>> m_classDictionary = new Dictionary<int, Func<Property>> {
        { 1,    () => new Property_ATK()},
        { 2,    () => new Property_ASP()},
        { 3,    () => new Property_HP()},
        { 4,    () => new Property_Crit()},
        { 5,    () => new Property_CritDemage()},
        { 6,    () => new Property_Speed()},
        { 7,    () => new Property_Dodge()},
        { 8,    () => new Property_Arrow()},
        { 9,    () => new Property_Buff()},
        {10,    () => new Property_Boss()},


        {100,   () => new Property_AreaRange()},
        {101,   () => new Property_AreaASP()},
        {102,   () => new Property_AreaReset()},
        {103,   () => new Property_SplitFocus()},
        {104,   () => new Property_AreaPoison()},
        {105,   () => new Property_AreaIceFrozen()},
        {106,   () => new Property_AreaBomb()},

        {108,   () => new Property_CritPoint()},
        {109,   () => new Property_DodgeHeal()},
    };
    #endregion

    public PropertyData Model;
    public Pear Pear = null;

    public int ID {get {return Model.ID;}}
    public int Value;
    public string Name {get {return Model.Name;}}
    public CONST.PROPERTY Type {get {return Model.Type;}}

    //是否生效(当携带相同的特殊词条时，只有属性更好的那个会生效)
    public bool IsValid = true;

    public static Property Create(Pear pear,  int id, int value = 0)
    {
        PropertyData property_data = DataCenter.Instance.GetPropertyData(id);
        if (property_data == null) return null;

        Property property;
        if (m_classDictionary.ContainsKey(id)) property = m_classDictionary[id]();
        else 
        {
            property = new Property();

            Debug.LogError("未实现的Property：" + id);
        }

        property.Init(property_data, pear, value);

        return property;
    }


    void Init(PropertyData config, Pear pear, int value)
    {
        Model   = config;
        Pear    = pear;
        Value   = value;
    }


    public virtual void Equip()
    {

    }

    public virtual void UnEquip()
    {

    }

    public virtual void CustomUpdate(float deltaTime)
    {

    }

    public virtual string GetDescription()
    {
        return Model.Description.Replace("#", Value.ToString());
    }
}
