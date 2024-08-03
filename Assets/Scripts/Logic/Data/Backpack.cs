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
    public string Description;
}

//背包系统
public class Backpack
{
    private List<Pear> m_Pears = new List<Pear>();


    //
    private Dictionary<int, PearData> m_PearDic = new Dictionary<int, PearData>();
    private Dictionary<int, Dictionary<int, PearData>> m_PearLevelPairs = new Dictionary<int, Dictionary<int, PearData>>();


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
                Description = data[5]
            };

            m_PearDic[pear.ID]  = pear;

            if (!m_PearLevelPairs.ContainsKey(pear.Level))
            {
                m_PearLevelPairs[pear.Level] = new Dictionary<int, PearData>();
            }

            m_PearLevelPairs[pear.Level][pear.ID] = pear;
        }
    }

    public PearData GetPearData(int id)
    {
        PearData data;
        if (m_PearDic.TryGetValue(id, out data)) {
            return data;
        }
        return null;
    }



    public void SyncPears(List<PearMsg> pear_msgs)
    {
        m_Pears.Clear();

        pear_msgs.ForEach(pear_msg => {
            Pear pear = Pear.Create(this.GetPearData(pear_msg.ID), pear_msg.Count);
            m_Pears.Add(pear);
        });
    }
}
