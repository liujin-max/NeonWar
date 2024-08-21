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
                Weight      = Convert.ToInt32(data[5]),
                Description = data[6]
            };

            m_PearDataDic[pear.ID]  = pear;

            if (!m_PearLevelPairs.ContainsKey(pear.Level))
            {
                m_PearLevelPairs[pear.Level] = new Dictionary<int, PearData>();
            }

            m_PearLevelPairs[pear.Level][pear.ID] = pear;
        }
    }

    public PearData GetPearData(int id)
    {
        return m_PearDataDic.TryGetValue(id, out PearData value) ? value : default;
    }

    Pear AddPear(int id, int count)
    {
        var data    = this.GetPearData(id);
        if (data == null) return null;

        Pear pear = Pear.Create((PearData)data, count);
        m_Pears.Add(pear);
        m_PearsDic[pear.ID] = pear;

        return pear;
    }

    public Pear GetPear(int id)
    {
        return m_PearsDic.TryGetValue(id, out Pear value) ? value : default;
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

    // 合成宝珠
    // 宝珠合成系统，任意相同品质三颗宝珠可以合成更高品质的随机宝珠
    // 三颗相同品质宝珠可以合成下一品质宝珠
    // 如果只花2颗宝珠合成，则成功率只有50%
    public Pear Synthesis(List<Pear> pears)
    {
        if (pears.Count <= 1) return null;

        int success_rate = (pears.Count - 1) * 50;

        //判断合成手续费是否足够
        int cost_coin   = 50;

        if (DataCenter.Instance.User.Coin < cost_coin) {
            EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "<sprite=0>不足"));
            return null;
        }

        //扣除消耗
        DataCenter.Instance.User.UpdateCoin(-cost_coin);

        //消耗宝珠
        pears.ForEach(pear => {
            RemovePear(pear.ID, 1);
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
            Dictionary<int, PearData> keyValuePairs = m_PearLevelPairs[pears[0].Level + 1];

            List<PearData> pear_datas = new List<PearData>(keyValuePairs.Values);
            int rand = RandomUtility.Random(0, pear_datas.Count);
            
            next_id = pear_datas[rand].ID;
        }

        Pear pear = PushPear(next_id);

        return pear;
    }
}
