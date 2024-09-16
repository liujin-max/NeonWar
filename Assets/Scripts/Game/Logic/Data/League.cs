using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

//技能配置
[System.Serializable]
public class SkillData
{
    public int ID;
    public string Name;
    public CONST.SKILL_TYPE Type;
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
                Type        = (CONST.SKILL_TYPE)Convert.ToInt32(data[2]),
                Values      = data[3].Split('|').Where(s => !string.IsNullOrWhiteSpace(s)).Select(int.Parse).ToArray(),
                Costs       = data[4].Split('|').Where(s => !string.IsNullOrWhiteSpace(s)).Select(int.Parse).ToArray(),
                Atlas       = Convert.ToInt32(data[5]),
                Icon        = Convert.ToInt32(data[6]),
                Description = data[7]
            };
            skill.LevelMax  = skill.Values.Length;

            m_SkillDic[skill.ID]  = skill;
        }
    }

    public SkillData GetSkillData(int id)
    {
        var sk_data = m_SkillDic.TryGetValue(id, out var data) ? data : null;
        return sk_data;
    }

    //重置技能
    public void ResetSkill(SkillSlotMsg slot)
    {
        if (slot.ID == -1) return;

        SkillData skillData = DataCenter.Instance.League.GetSkillData(slot.ID);

        int cost_total = 0;

        for (int i = 0; i < slot.Level; i++)
        {
            cost_total += skillData.GetCost(i); //Skill.GetCost(skillData, i);
        }

        //重置技能
        DataCenter.Instance.User.ResetSkill(slot);

        //返还资源
        DataCenter.Instance.User.UpdateGlass(cost_total);

        EventManager.SendEvent(new GameEvent(EVENT.UI_SKILLUPGRADE));
        EventManager.SendEvent(new GameEvent(EVENT.ONUPDATEGLASS));
    }
}
