using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;


#region 暴击率
public class Pear_Crit : Pear
{
    public override void Equip(Player player)
    {
        base.Equip(player);

        Belong.ATT.CP.PutADD(this, m_Data.Value * 10);
    }

    public override void UnEquip()
    {
        Belong.ATT.CP.Pop(this);

        base.UnEquip();
    }
}
#endregion


#region 暴击伤害
public class Pear_CritDemage : Pear
{
    public override void Equip(Player player)
    {
        base.Equip(player);

        Belong.ATT.CT.PutADD(this, m_Data.Value * 10);
    }

    public override void UnEquip()
    {
        Belong.ATT.CT.Pop(this);

        base.UnEquip();
    }
}
#endregion


#region 移动速度
public class Pear_Speed : Pear
{
    public override void Equip(Player player)
    {
        base.Equip(player);

        Belong.ATT.SPEED.PutADD(this, m_Data.Value);
    }

    public override void UnEquip()
    {
        Belong.ATT.SPEED.Pop(this);

        base.UnEquip();
    }
}
#endregion


#region 体力
public class Pear_HP : Pear
{
    public override void Equip(Player player)
    {
        base.Equip(player);

        Belong.ATT.HPMAX += m_Data.Value;
        Belong.ATT.HP += m_Data.Value;
    }

    public override void UnEquip()
    {
        Belong.ATT.HPMAX -= m_Data.Value;
        Belong.ATT.HP -= m_Data.Value;

        base.UnEquip();
    }
}
#endregion


#region 增益时间提升
public class Pear_Buff : Pear
{
    public override void Equip(Player player)
    {
        base.Equip(player);

        EventManager.AddHandler(EVENT.ONBUFFADD,    OnBuffAdd);
    }

    public override void UnEquip()
    {
        EventManager.AddHandler(EVENT.ONBUFFADD,    OnBuffAdd);

        base.UnEquip();
    }

    private void OnBuffAdd(GameEvent @event)
    {
        Buff buff = @event.GetParam(0) as Buff;

        if (buff.TYPE == CONST.BUFF_TYPE.GAIN)
        {
            buff.Duration.SetDuration(buff.Duration.Duration * (1 + m_Data.Value / 100.0f));
        }
    }
}
#endregion


#region 闪避
// 武器闪避率提高#%
public class Pear_Dodge : Pear
{
    public override void Equip(Player player)
    {
        base.Equip(player);

        Belong.ATT.DODGE.PutADD(this, m_Data.Value);
    }

    public override void UnEquip()
    {
        Belong.ATT.DODGE.Pop(this);

        base.UnEquip();
    }
}
#endregion


#region 头目伤害加成
// 对头目造成的伤害提高#%
public class Pear_Boss : Pear
{
    public override void Equip(Player player)
    {
        base.Equip(player);
        
        Belong.ATT.BOSS_INC.PutAUL(this, m_Data.Value / 100.0f);
    }

    public override void UnEquip()
    {
        Belong.ATT.BOSS_INC.Pop(this);
        
        base.UnEquip();
    }
}
#endregion



#region 箭矢伤害加成
public class Pear_Arrow : Pear
{
    public override void Equip(Player player)
    {
        base.Equip(player);
        
        EventManager.AddHandler(EVENT.ONBULLETCREATE,   OnBulletCreate);
    }

    public override void UnEquip()
    {
        EventManager.DelHandler(EVENT.ONBULLETCREATE,   OnBulletCreate);
        
        base.UnEquip();
    }

    private void OnBulletCreate(GameEvent @event)
    {
        Bullet bullet = @event.GetParam(0) as Bullet;

        if (bullet.Caster.ID == (int)CONST.PLAYER.BOW)
        {
            bullet.Hit.ATK_INC.PutAUL(this, m_Data.Value / 100.0f);
        }
    }
}
#endregion


#region 陷阱的范围增加#
public class Pear_AreaRange : Pear
{
    public override void Equip(Player player)
    {
        base.Equip(player);
        
        EventManager.AddHandler(EVENT.ONPUSHAREA,   OnPushArea);
    }

    public override void UnEquip()
    {
        EventManager.DelHandler(EVENT.ONPUSHAREA,   OnPushArea);
        
        base.UnEquip();
    }

    private void OnPushArea(GameEvent @event)
    {
        Area area = @event.GetParam(0) as Area;

        if (area.Belong != Belong) return;

        // if (area.transform.GetComponent<Area_Poison>() != null 
        //     || area.transform.GetComponent<Area_Ice>() != null 
        //     || area.transform.GetComponent<Area_Rope>() != null)
        // {
        float radius    = 1 + m_Data.Value / 100.0f;
        area.transform.localScale = new Vector3(radius, radius, 1);
        // }
    }
}

#endregion 


#region 陷阱的冷却速度提高#%
public class Pear_AreaASP : Pear
{
    public override void CustomUpdate(float deltaTime)
    {
        foreach (var sk in Belong.Skills)
        {
            if (sk.ID == 10210 || sk.ID == 10220 || sk.ID == 10230)
            {
                sk.CustomUpdate(deltaTime * m_Data.Value / 100.0f);
            }
        }
    }
}
#endregion

#region 投掷陷阱时有#%的概率立即刷新冷却时间
public class Pear_AreaReset : Pear
{
    public override void Equip(Player player)
    {
        base.Equip(player);
        
        EventManager.AddHandler(EVENT.ONPLAYTRAP,   OnPlayTrap);
    }

    public override void UnEquip()
    {
        EventManager.DelHandler(EVENT.ONPLAYTRAP,   OnPlayTrap);
        
        base.UnEquip();
    }

    private void OnPlayTrap(GameEvent @event)
    {
        Skill sk = @event.GetParam(0) as Skill;

        if (sk.Caster != Belong) return;

        if (!RandomUtility.IsHit(m_Data.Value)) return;

        sk.FullCD();
    }
}
#endregion









//宝珠
public class Pear
{
    protected PearData m_Data;
    public PearData Data { get => m_Data;}

    public int ID { get => m_Data.ID;}
    public int Level { get => m_Data.Level;}
    public int Class { get => m_Data.Class;}
    
    //列表排序权重
    public int SortOrder{
        get {
            int base_order = Level;

            if (DataCenter.Instance.User.IsPearEquiped(this.ID)) base_order += 10;

            return base_order;
        }
    }

    public int Count;
    public Player Belong;



    #region 逻辑配置
    private static Dictionary<int, Func<Pear>> m_classDictionary = new Dictionary<int, Func<Pear>> {
        {20000, () => new Pear_Crit()},
        {20010, () => new Pear_CritDemage()},
        {20020, () => new Pear_Speed()},
        {20030, () => new Pear_HP()},
        {20040, () => new Pear_Buff()},
        {20050, () => new Pear_Dodge()},
        {20060, () => new Pear_Boss()},
        {20070, () => new Pear_Arrow()}
    };
    #endregion

    public static Pear Create(int id, int count = 1)
    {
        PearData pearData = DataCenter.Instance.Backpack.GetPearData(id);
        if (pearData == null) {
            Debug.LogError("未找到宝珠ID ： " + id);
            return null;
        }

        Pear pear;
        if (m_classDictionary.ContainsKey(pearData.Class)) pear = m_classDictionary[pearData.Class]();
        else {
            pear = new Pear();

            Debug.LogError("未实现的宝珠：" + pearData.Class);
        }

        pear.Init(pearData, count);

        return pear;
    }

    public void Init(PearData pearData, int count)
    {
        m_Data  = pearData;
        Count   = count;
    }

    public void UpdateCount(int value)
    {
        Count += value;
    }

    public bool IsLevelMax()
    {
        return m_Data.Level >= 5;
    }

    public virtual void Equip(Player player)
    {
        Belong = player;
    }

    public virtual void UnEquip()
    {
        Belong = null;
    }

    public virtual void CustomUpdate(float deltaTime)
    {

    }

    public string GetName()
    {
        var data = DataCenter.Instance.Backpack.GetPearData(m_Data.Class);
        return data.Name;
    }

    public string GetDescription()
    {
        var data = DataCenter.Instance.Backpack.GetPearData(m_Data.Class);
        string text = data.Description.Replace("#", CONST.COLOR_GREEN2 + m_Data.Value.ToString() + "</color>");

        return text;
    }
    
}
