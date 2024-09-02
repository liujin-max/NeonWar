using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PearData
{
    public int ID;
    public string Name;
    public int Class;
    public int Level;
    public int Value;
    public int Weight;
    public string Description;
}

//背包系统
public class Backpack
{
    private List<Pear> m_Pears = new List<Pear>();
    public List<Pear> Pears {get => m_Pears;}
    private Dictionary<int, Pear> m_PearsDic = new Dictionary<int, Pear>();


    //
    private Dictionary<int, PearData> m_PearDataDic = new Dictionary<int, PearData>();
    private Dictionary<int, List<PearData>> m_PearLevelPairs = new Dictionary<int, List<PearData>>();


    public Dictionary<int, int[]> PearPools = new Dictionary<int, int[]>()
    {
        //根据Level(品质)进行分类
        //关卡最多掉落品质2及以下
        [1] = new int[] {20000, 20010, 20020, 20030, 20040, 20050, 20060, 20070},
        [2] = new int[] {20001, 20011, 20021, 20031, 20041, 20051, 20061, 20071},


        [99] = new int[] {20030}  //特殊：15关的掉落池
    };


    public void Init()
    {
        InitPearDatas();
    }

    //宝珠数据
    void InitPearDatas()
    {
        List<string[]> list = GameFacade.Instance.CsvManager.GetStringArrays(CsvManager.TableKey_Pear);
        foreach (string[] data in list) 
        {
            PearData pear   = new PearData()
            {
                ID          = Convert.ToInt32(data[0]),
                Name        = data[1],
                Class       = Convert.ToInt32(data[2]),
                Level       = Convert.ToInt32(data[3]),
                Value       = Convert.ToInt32(data[4]),
                Weight      = Convert.ToInt32(data[5]),
                Description = data[6]
            };

            m_PearDataDic[pear.ID]  = pear;

            if (!m_PearLevelPairs.ContainsKey(pear.Level))
            {
                m_PearLevelPairs[pear.Level] = new List<PearData>();
            }

            m_PearLevelPairs[pear.Level].Add(pear);
        }
    }

    public PearData GetPearData(int id)
    {
        return m_PearDataDic.TryGetValue(id, out PearData value) ? value : default;
    }

    Pear AddPear(int id, int count)
    {
        Pear pear = Pear.Create(id, count);
        m_Pears.Add(pear);
        m_PearsDic[pear.ID] = pear;

        return pear;
    }

    public Pear GetPear(int id)
    {
        return m_PearsDic.TryGetValue(id, out Pear value) ? value : default;
    }

    public List<PearData> GetPearDatasByLevel(int level)
    {
        return m_PearLevelPairs[level];
    }

    public void SyncPears(List<PearMsg> pear_msgs)
    {
        m_Pears.Clear();

        pear_msgs.ForEach(pear_msg => {
            AddPear(pear_msg.ID, pear_msg.Count);
        });


        //测试代码
        // PushPear(20022);
        //
    }

    public Pear PushPear(int id, int count = 1)
    {
        if (m_PearsDic.ContainsKey(id) == true)
        {
            Pear pear = m_PearsDic[id];
            pear.UpdateCount(count);

            return pear;
        }
        
        return AddPear(id, count);
    }

    public void RemovePear(int id, int count)
    {
        if (!m_PearsDic.ContainsKey(id)) return;

        var pear = m_PearsDic[id];

        pear.UpdateCount(-count);

        if (pear.Count <= 0)
        {
            m_PearsDic[id] = null;
            m_Pears.Remove(pear);
        }
    }


}
