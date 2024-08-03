using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Pear_Crit : Pear
{

}


//宝珠
public class Pear
{
    private PearData m_Data;
    public PearData Data { get => m_Data; set => m_Data = value; }

    public int ID { get => m_Data.ID;}
    public int Count;
    public Unit Caster;

    private static Dictionary<int, Func<Pear>> m_classDictionary = new Dictionary<int, Func<Pear>> {
        {20000, () => new Pear_Crit()}
    };

    

    public static Pear Create(PearData pearData, int count = 1)
    {
        Pear pear;
        if (m_classDictionary.ContainsKey(pearData.ID)) pear = m_classDictionary[pearData.ID]();
        else pear = new Pear();

        pear.Init(pearData, count);

        return pear;
    }

    public void Init(PearData pearData, int count)
    {
        m_Data  = pearData;
        Count   = count;
    }

    public bool IsLevelMax()
    {
        return m_Data.Level >= 5;
    }

    public void Equip(Unit player)
    {
        Caster = player;
    }

    public void UnEquip()
    {

    }

    public virtual void Dispose()
    {

    }
}
