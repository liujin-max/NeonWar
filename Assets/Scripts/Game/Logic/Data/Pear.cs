using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

//道具
public class Pear
{
    public PearData Model;


    public int ID { get => Model.ID;}
    public int UUID;
    public string Name {get => Model.Name;}
    public int Level;

    public Player Belong;
    public List<Property> Properties = new List<Property>();
    public Property SpecialProperty = null;
    public bool IsNew = false;


    //列表排序权重
    public int SortOrder {
        get {
            int base_order = Level * 1000 + UUID;

            if (DataCenter.Instance.User.IsPearEquiped(this)) base_order *= 10000;

            return base_order;
        }
    }


    public string[] ExportPropertiesMsg()
    {
        List<string> results = new List<string>();

        //导出词条数据 ID:Value
        for (int i = 0; i < Properties.Count; i++)
        {
            var property = Properties[i];
            results.Add(string.Format("{0}:{1}", property.ID, property.Value));
        }

        if (SpecialProperty != null)
        {
            results.Add(string.Format("{0}:{1}", SpecialProperty.ID, SpecialProperty.Value));
        }

        return results.ToArray();
    }

    public Pear(int id, int uuid, int level, string[] properties)
    {
        Model   = DataCenter.Instance.Backpack.GetPearData(id);
        UUID    = uuid;
        Level   = level;

        InitProperties(properties);
    }
    
    void InitProperties(string[] properties)
    {
        for (int i = 0; i < properties.Length; i++)
        {
            string[] splits = properties[i].Split(':');
            int id = Convert.ToInt32(splits[0]);
            int value = Convert.ToInt32(splits[1]);

            var property = Property.Create(this, id, value);

            if (property.Type == CONST.PROPERTY.SPECIAL) SpecialProperty = property;
            else
            {
                Properties.Add(property);
            }
        }
    }

    public bool IsLevelMax()
    {
        return Level >= 5;
    }

    public void Equip(Player player)
    {
        Belong = player;

        foreach (var p in Properties) p.Equip();
        
        if (SpecialProperty != null && SpecialProperty.IsValid)
        {
            SpecialProperty.Equip();
        }
    }

    public void UnEquip()
    {
        foreach (var p in Properties) p.UnEquip();

        if (SpecialProperty != null && SpecialProperty.IsValid)
        {
            SpecialProperty.UnEquip();
        }

        Belong = null;
    }

    public void CustomUpdate(float deltaTime)
    {
        foreach (var p in Properties) p.CustomUpdate(deltaTime);

        if (SpecialProperty != null && SpecialProperty.IsValid)
        {
            SpecialProperty.CustomUpdate(deltaTime);
        }
    }

}
