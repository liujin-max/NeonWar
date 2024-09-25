using System;
using System.Collections.Generic;
using UnityEngine;



#region 技能数据
[System.Serializable]
public class SkillData
{
    public int ID;
    public string Name;
    public int LevelMax;
    public int[] Values;
    public int[] Costs;
    public int Atlas;           //图集
    public int Icon;            
    public string Description;

    public int GetValue(int level)
    {
        return Values[Mathf.Min(LevelMax - 1, level - 1)];
    } 

    public int GetCost(int level)
    {
        int order = Mathf.Min(LevelMax - 1, level);
        return Costs[order];
    }

    public string GetDescription(int level, string color = "<#FFFFFF>")
    {
        int value = GetValue(level);

        return Description.Replace("#", (color + value + "</color>"));
    }

    public string CompareDescription(int o_level, int t_level)
    {
        if (o_level == 0) return this.GetDescription(t_level, CONST.COLOR_GREEN);
        if (o_level == this.LevelMax) return this.GetDescription(o_level, CONST.COLOR_GREEN);

        int o_value = this.GetValue(o_level);
        int t_value = this.GetValue(t_level);

        string str = CONST.COLOR_GREEN + o_value + "</color>->" + CONST.COLOR_GREEN + t_value + "</color>";

        return this.Description.Replace("#", str);
    }
}
#endregion



#region 道具数据
[System.Serializable]
public class PearData
{
    public int ID;
    public string Name;
    public int Weight;
    public int[] Properties;
}
#endregion


#region 属性数据
[System.Serializable]
public class PropertyData
{
    public int ID;
    public string Name;
    public CONST.PROPERTY Type;
    public int Point;
    public int Weight;
    public string Description;
}
#endregion



//全局数据类
public class DataCenter
{
    //账号信息
    public User User;
    public League League;
    public Levels Levels;
    public Backpack Backpack;


    //属性词条数据
    private Dictionary<int, PropertyData> m_Properties = new Dictionary<int, PropertyData>();
    //基础属性词条池
    private Dictionary<PropertyData, int> m_NormalPropertyWeights = new Dictionary<PropertyData, int>();
    //特殊词条池
    private Dictionary<PropertyData, int> m_SpecialPropertyWeights = new Dictionary<PropertyData, int>();


    private static DataCenter m_Instance;
    public static DataCenter Instance {
        get {
            if (m_Instance == null) m_Instance = new DataCenter();

            return m_Instance;
        }
    }


    public void Init()
    {
        //账号数据
        User    = new User();

        League  = new League();
        League.Init();

        Levels  = new Levels();

        Backpack= new Backpack();
        Backpack.Init();

        //词条数据
        InitProperties();
    }

    void InitProperties()
    {
        List<string[]> list = GameFacade.Instance.CsvManager.GetStringArrays(CsvManager.TableKey_Property);
        foreach (string[] config in list) 
        {
            PropertyData pear   = new PropertyData()
            {
                ID      = Convert.ToInt32(config[0]),
                Name    = config[1],
                Type    = (CONST.PROPERTY)Convert.ToInt32(config[2]),
                Point   = Convert.ToInt32(config[3]),
                Weight  = Convert.ToInt32(config[4]),
                Description = config[5]
            };

            m_Properties[pear.ID] = pear;

            if (pear.Type == CONST.PROPERTY.NORMAL) m_NormalPropertyWeights.Add(pear, pear.Weight);
            else m_SpecialPropertyWeights.Add(pear, pear.Weight);
        }
    }

    public void Update(float dt)
    {
        User.Update(dt);
    }

    public bool IsPearUnlock()
    {
        return User.Level >= CONST.PEAR_UNLOCK_LEVEL;
    }

    public string GetLevelString()
    {
        int level   = User.Level + 1;

        return string.Format("关卡 {0}", level);
    }

    //获取属性词条数据
    public PropertyData GetPropertyData(int id)
    {
        return m_Properties.TryGetValue(id, out var p) ? p : null;
    }


    #region 道具属性生成逻辑
    // 道具的词条以及具体数值是随机生成的，道具按品质划分一个基础的属性点，在该属性点上下20%浮动
    // 每种属性词条消耗的属性点不同。
    // 每种品质拥有的属性条数量也不同
    // 生成道具时，先根据品质决定有几条属性词条。
    // 再随机出具体的属性词条，属性词条可重复
    // 最后根据装备的属性点，分配到对应的属性词条上。
    public string[] GeneratePearProperties(int pear_id, int level)
    {
        PearData pear_data = DataCenter.Instance.Backpack.GetPearData(pear_id);

        int base_point      = CONST.LEVEL_POINT_PAIRS[level];
        int property_count  = CONST.LEVEL_PROPERTY_PAIRS[level];

        //平均每条属性可以使用的属性点
        int avg_point       = base_point / property_count;

        //根据权重获取属性词条
        var property_weight = new Dictionary<PropertyData, int>();
        foreach (var id in pear_data.Properties)
        {
            var property_data = this.GetPropertyData(id);
            property_weight.Add(property_data, property_data.Weight);
        }


        Debug.Log("生成道具：" + pear_data.Name);
        //开始随机词条
        List<string> propertys  = new List<string>();
        for (int i = 0; i < property_count; i++)
        {
            PropertyData property_data = RandomUtility.PickByWeight(property_weight);
            int real_point  = RandomUtility.Random((int)(avg_point * 0.8f), (int)(avg_point * 1.2f));
            int value       = Mathf.Max(real_point / property_data.Point, 1);

            propertys.Add(string.Format("{0}:{1}", property_data.ID, value));

            Debug.Log(property_data.Name + ":" + value);
        }

        //特殊词条
        int sp_rate = (level - 1) * 35;
        if (RandomUtility.IsHit(sp_rate) == true)
        {
            PropertyData property_data = RandomUtility.PickByWeight(m_SpecialPropertyWeights);
            int real_point  = RandomUtility.Random((int)(base_point * 0.8f), (int)(base_point * 1.2f));
            int value       = Mathf.Max(real_point / property_data.Point, 1);

            propertys.Add(string.Format("{0}:{1}", property_data.ID, value));
        }

        return propertys.ToArray();
    }
    #endregion
    

    // 合成宝珠
    // 宝珠合成系统，任意相同品质三颗宝珠可以合成更高品质的随机宝珠
    // 三颗相同品质宝珠可以合成下一品质宝珠
    // 如果只花2颗宝珠合成，则成功率只有50%
    public Pear Synthesis(List<Pear> pears)
    {
        if (pears.Count <= 1) return null;

        int success_rate = (pears.Count - 1) * 50;


        User.IsDirty = true;

        //消耗宝珠
        pears.ForEach(pear => {
            // Backpack.RemovePear(pear.ID, 1);
        });


        if (!RandomUtility.IsHit(success_rate)) {
            EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "合成失败"));
            return null;
        }

        bool is_same    = true;
        int last_id     = pears[0].ID;

        for (int i = 0; i < pears.Count; i++)
        {
            if (pears[i].ID != last_id) {
                is_same = false;
                break;
            }
        }


        int next_id;
        if (is_same == true)
        {
            next_id = pears[0].ID + 1;
        }
        else
        {
            // List<PearData> pear_datas = Backpack.GetPearDatasByLevel(pears[0].Level + 1);
            // int rand = RandomUtility.Random(0, pear_datas.Count);
            
            // next_id = pear_datas[rand].ID;
        }

        // Pear pear = Backpack.PushPear(next_id);

        // return pear;
        return null;
    }
}

