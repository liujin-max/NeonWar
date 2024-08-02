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
    public int ID;
    public int Level;
    public int Count;
    public Unit Caster;

    private static Dictionary<int, Func<Pear>> m_classDictionary = new Dictionary<int, Func<Pear>> {
        {20001, () => new Pear_Crit()}
    };

    public static Pear Create(int id, int level = 1, int count = 1)
    {
        Pear pear;
        if (m_classDictionary.ContainsKey(id)) pear = m_classDictionary[id]();
        else pear = new Pear();

        pear.Init(id, level, count);

        return pear;
    }

    public void Init(int id, int level, int count)
    {
        ID      = id;
        Level   = level;
        Count   = count;
    }

    public void Equip(Unit player)
    {
        Caster = player;
    }

    public void UnEquip()
    {

    }
}
