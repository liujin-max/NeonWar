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

        player.ATT.CP.PutADD(this, m_Data.Value * 10);
    }

    public override void UnEquip()
    {
        base.UnEquip();

        Caster.ATT.CP.Pop(this);
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
        {20000, () => new Pear_Crit()}
    };

    

    public static Pear Create(PearData pearData, int count = 1)
    {
        Pear pear;
        if (m_classDictionary.ContainsKey(pearData.Class)) pear = m_classDictionary[pearData.Class]();
        else pear = new Pear();

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
