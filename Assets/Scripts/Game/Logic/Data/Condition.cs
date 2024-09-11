using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#region 前置全部解锁
public class Condition_PreviousAll : Condition
{
    public override bool Check()
    {
        foreach (var id in Values) 
        {
            if (Field.Instance.Player.GetSkill(id) == null) 
            {
                return false;
            }
        }

        return true;
    }
}
#endregion


#region 前置任意解锁
public class Condition_Previous : Condition
{
    public override bool Check()
    {
        foreach (var id in Values) 
        {
            if (Field.Instance.Player.GetSkill(id) != null) 
            {
                return true;
            }
        }

        return false;
    }
}
#endregion


#region 血量不满
public class Condition_HPNotFull : Condition
{
    public override bool Check()
    {
        return !Field.Instance.Player.IsHPFull();
    }
}
#endregion



//技能解锁条件
public class Condition
{
    protected int ID;
    protected int[] Values;

    private static Dictionary<int, Func<Condition>> m_classDictionary = new Dictionary<int, Func<Condition>> 
    {
        {101, () => new Condition_PreviousAll()},
        {102, () => new Condition_Previous()},
        {103, () => new Condition_HPNotFull()},
    };


    // condition_str : 101:10210|10220|10230
    public static Condition Create(string condition_str)
    {
        string[] conditions = condition_str.Split(':');
        int condition_id    = Convert.ToInt32(conditions[0]);
        // 将字符串数组转换为 int 数组
        int[] values        = Array.ConvertAll(conditions[1].Split('|'), int.Parse);

        Condition c;
        if (m_classDictionary.ContainsKey(condition_id)) c = m_classDictionary[condition_id]();
        else 
        {
            c = new Condition();
            Debug.LogError("未实现的Condition：" + condition_id);
        }

        c.Init(condition_id, values);

        return c;
    }


    public void Init(int id, int[] values)
    {
        ID      = id;
        Values  = values;
    }

    public virtual bool Check()
    {
        return true;
    }
}
