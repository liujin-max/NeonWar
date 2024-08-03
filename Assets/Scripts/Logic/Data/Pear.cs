using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#region 暴击率
public class Pear_Crit : Pear
{
    public override void Equip(Unit player)
    {
        base.Equip(player);

        Caster.ATT.CP.PutADD(this, m_Data.Value * 10);
    }

    public override void UnEquip()
    {
        base.UnEquip();

        Caster.ATT.CP.Pop(this);
    }
}
#endregion


#region 暴击伤害
public class Pear_CritDemage : Pear
{
    public override void Equip(Unit player)
    {
        base.Equip(player);

        Caster.ATT.CT.PutADD(this, m_Data.Value * 10);
    }

    public override void UnEquip()
    {
        Caster.ATT.CT.Pop(this);

        base.UnEquip();
    }
}
#endregion


#region 移动速度
public class Pear_Speed : Pear
{
    public override void Equip(Unit player)
    {
        base.Equip(player);

        Caster.ATT.SPEED.PutADD(this, m_Data.Value);
    }

    public override void UnEquip()
    {
        Caster.ATT.SPEED.Pop(this);

        base.UnEquip();
    }
}
#endregion


#region 体力
public class Pear_HP : Pear
{
    public override void Equip(Unit player)
    {
        base.Equip(player);

        Caster.ATT.HPMAX += m_Data.Value;
        Caster.ATT.HP += m_Data.Value;
    }

    public override void UnEquip()
    {
        Caster.ATT.HPMAX -= m_Data.Value;
        Caster.ATT.HP -= m_Data.Value;

        base.UnEquip();
    }
}
#endregion


#region 增益时间提升
public class Pear_Buff : Pear
{
    public override void Equip(Unit player)
    {
        base.Equip(player);

        EventManager.AddHandler(EVENT.ONADDBUFF,    OnBuffAdd);
    }

    public override void UnEquip()
    {
        EventManager.AddHandler(EVENT.ONADDBUFF,    OnBuffAdd);

        base.UnEquip();
    }

    private void OnBuffAdd(GameEvent @event)
    {
        Buff buff = @event.GetParam(0) as Buff;

        if (buff.TYPE == _C.BUFF_TYPE.GAIN)
        {
            buff.Duration.SetDuration(buff.Duration.Duration * (1 + m_Data.Value / 100.0f));
        }
    }
}
#endregion


#region 头目伤害加成
// 对头目造成的伤害提高#%
public class Pear_Boss : Pear
{
    public override void Equip(Unit player)
    {
        base.Equip(player);
        
        
    }

    public override void UnEquip()
    {
        
        
        base.UnEquip();
    }
}
#endregion






//宝珠
public class Pear
{
    protected PearData m_Data;
    public PearData Data { get => m_Data; set => m_Data = value; }

    public int ID { get => m_Data.ID;}
    public int Level { get => m_Data.Level;}
    public int Class { get => m_Data.Class;}
    public int Count;
    public Unit Caster;

    private static Dictionary<int, Func<Pear>> m_classDictionary = new Dictionary<int, Func<Pear>> {
        {20000, () => new Pear_Crit()},
        {20010, () => new Pear_CritDemage()},
        {20020, () => new Pear_Speed()},
        {20030, () => new Pear_HP()},
        {20040, () => new Pear_Buff()}
    };

    

    public static Pear Create(PearData pearData, int count = 1)
    {
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

    public virtual void Equip(Unit player)
    {
        Caster = player;
    }

    public virtual void UnEquip()
    {
        Caster = null;
    }


}
