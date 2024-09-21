using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

//道具
public class Pear
{
    protected PearData m_Data;
    public PearData Data { get => m_Data;}

    public int ID { get => m_Data.ID;}
    public int UUID;
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

    public Player Belong;
    public List<Property> Properties;


    public void Init(int id, int uuid, string[] properties)
    {
        m_Data  = DataCenter.Instance.Backpack.GetPearData(id);;
        UUID    = uuid;

        Properties = new List<Property>(properties.Length);
    }

    public bool IsLevelMax()
    {
        return m_Data.Level >= 5;
    }

    public void Equip(Player player)
    {
        Belong = player;

        foreach (var p in Properties) p.Equip();
    }

    public void UnEquip()
    {
        foreach (var p in Properties) p.UnEquip();

        Belong = null;
    }

    public void CustomUpdate(float deltaTime)
    {
        foreach (var p in Properties) p.CustomUpdate(deltaTime);
    }

    public string GetName()
    {
        var data = DataCenter.Instance.Backpack.GetPearData(m_Data.Class);
        return data.Name;
    }

    // public string GetDescription()
    // {
    //     var data = DataCenter.Instance.Backpack.GetPearData(m_Data.Class);
    //     string text = data.Description.Replace("#", CONST.COLOR_GREEN2 + m_Data.Value.ToString() + "</color>");

    //     return text;
    // }
    
}
