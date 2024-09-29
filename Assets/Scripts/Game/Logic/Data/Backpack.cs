using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//背包系统
public class Backpack
{
    private List<Pear> m_Pears = new List<Pear>();
    public List<Pear> Pears {get => m_Pears;}
    private Dictionary<int, Pear> m_PearsDic = new Dictionary<int, Pear>();


    //
    private Dictionary<int, PearData> m_PearDataDic = new Dictionary<int, PearData>();
    private Dictionary<PearData, int> m_PearDataWeights = new Dictionary<PearData, int>();


    public Dictionary<int, int[]> PearPools = new Dictionary<int, int[]>()
    {
        //根据Level(品质)进行分类
        //关卡最多掉落品质2及以下
        [1] = new int[] {20000, 20010, 20020, 20030, 20040, 20050, 20060},
        [2] = new int[] {20001, 20011, 20021, 20031, 20041, 20051, 20061},


        [99] = new int[] {20030}  //特殊：15关的掉落池
    };



    #region 同步数据
    public void SyncPears(List<PearMsg> pear_msgs)
    {
        m_Pears.Clear();

        pear_msgs.ForEach(pear_msg => {
            AddPear(pear_msg.ID, pear_msg.UUID, pear_msg.Level, pear_msg.Properties);
        });
    }
    #endregion



    #region 导出数据
    //同步道具数据
    public List<PearMsg> ExportPears()
    {
        List<PearMsg> msgs = new List<PearMsg>();

        foreach (var pear in m_Pears)
        {
            var msg = new PearMsg() 
            {
                ID      = pear.ID,
                UUID    = pear.UUID,
                Level   = pear.Level,
                Properties = pear.ExportPropertiesMsg()
            };

            msgs.Add(msg);
        }

        return msgs;
    }
    #endregion


    public void Init()
    {
        InitPearDatas();
    }

    //道具数据
    void InitPearDatas()
    {
        List<string[]> list = GameFacade.Instance.CsvManager.GetStringArrays(CsvManager.TableKey_Pear);
        foreach (string[] data in list) 
        {
            PearData pear   = new PearData()
            {
                ID          = Convert.ToInt32(data[0]),
                Name        = data[1],
                Weight      = Convert.ToInt32(data[2]),
                Properties  = data[3].Split(';').Select(int.Parse).ToArray(),
                Specials    = data[4].Split(';').Where(s => !string.IsNullOrWhiteSpace(s)).Select(int.Parse).ToArray(),
            };

            m_PearDataDic.Add(pear.ID, pear);
            m_PearDataWeights.Add(pear, pear.Weight);
        }
    }

    public PearData GetPearData(int id)
    {
        return m_PearDataDic.TryGetValue(id, out PearData value) ? value : default;
    }

    public PearData PickPearData()
    {
        return RandomUtility.PickByWeight(m_PearDataWeights);
    }

    Pear AddPear(int id, int uuid, int level, string[] properties)
    {
        Pear pear = new Pear(id, uuid, level, properties);
        m_Pears.Add(pear);
        m_PearsDic[uuid] = pear;

        return pear;
    }

    public Pear GetPearByUUID(int uuid)
    {
        return m_PearsDic.TryGetValue(uuid, out Pear value) ? value : null;
    }

    public Pear PushPear(int id, int level)
    {   
        int uuid = DataCenter.Instance.User.CreateUUID();

        string[] properties = DataCenter.Instance.GeneratePearProperties(id, level);

        return AddPear(id, uuid, level, properties);
    }

    public void RemovePear(int uuid)
    {
        if (!m_PearsDic.ContainsKey(uuid)) return;

        var pear = m_PearsDic[uuid];

        m_PearsDic[uuid] = null;
        m_Pears.Remove(pear);
    }
}
