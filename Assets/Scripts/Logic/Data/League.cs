using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//技能配置
[System.Serializable]
public class SkillData
{
    public int ID;
    public string Name;
    public _C.SKILL_TYPE Type;
    public int Weight;
    public int LevelMax;
    public int[] Values;
    public int[] Previous;      //前置技能(满足其一即可)
    public int[] PreviousAll;   //前置技能(需要全部满足)
    public int[] Excludes;      //互斥技能
    public int Atlas;           //图集
    public int Icon;            
    public string Description;
}


//负责管理武器和技能的数据
public class League
{
    private Dictionary<int, SkillData> m_SkillDic = new Dictionary<int, SkillData>();

    public void Init()
    {
        InitSkills();
    }


    //技能数据
    void InitSkills()
    {
        List<string[]> list = GameFacade.Instance.CsvManager.GetStringArrays(CsvManager.TableKey_Skill);
        foreach (string[] data in list) {
            SkillData skill = new SkillData()
            {
                ID          = Convert.ToInt32(data[0]),
                Name        = data[1],
                Type        = (_C.SKILL_TYPE)Convert.ToInt32(data[2]),
                Weight      = Convert.ToInt32(data[3]),
                Values      = data[4].Split('|').Where(s => !string.IsNullOrWhiteSpace(s)).Select(int.Parse).ToArray(),
                Previous    = data[5].Split('|').Where(s => !string.IsNullOrWhiteSpace(s)).Select(int.Parse).ToArray(),
                PreviousAll = data[6].Split('|').Where(s => !string.IsNullOrWhiteSpace(s)).Select(int.Parse).ToArray(),
                Excludes    = data[7].Split('|').Where(s => !string.IsNullOrWhiteSpace(s)).Select(int.Parse).ToArray(),
                Atlas       = Convert.ToInt32(data[8]),
                Icon        = Convert.ToInt32(data[9]),
                Description = data[10]
            };
            skill.LevelMax  = skill.Values.Length;

            m_SkillDic[skill.ID]  = skill;
        }
    }

    public SkillData GetSkillData(int id)
    {
        var sk_data = m_SkillDic.TryGetValue(id, out var data) ? data : null;
        if (sk_data == null) Debug.LogError("GetSkillData is nil : " + id);
        return sk_data;
    }

    //重置技能
    // public void ResetSkill(SkillSlotMsg slot)
    // {
    //     if (slot.ID == -1) return;

    //     SkillData skillData = DataCenter.Instance.League.GetSkillData(slot.ID);

    //     int cost_total = 0;

    //     for (int i = 0; i < slot.Level; i++)
    //     {
    //         cost_total += Skill.GetCost(skillData, i);
    //     }

    //     //重置技能
    //     DataCenter.Instance.User.ResetSkill(slot);

    //     //返还资源
    //     DataCenter.Instance.User.UpdateGlass(cost_total);

    //     EventManager.SendEvent(new GameEvent(EVENT.UI_SKILLUPGRADE));
    //     EventManager.SendEvent(new GameEvent(EVENT.ONUPDATEGLASS));
    // }
}
