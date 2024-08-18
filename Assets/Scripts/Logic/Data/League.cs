using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class SkillData
{
    public int ID;
    public string Name;
    public int Belong;
    public int LevelMax;
    public int[] Values;
    public int[] Glass;
    public int Order;
    public string Description;
}

//负责管理武器和技能的数据
public class League
{
    private Dictionary<int, SkillData> m_SkillDic = new Dictionary<int, SkillData>();
    private Dictionary<int, List<SkillData>> m_PlayerSkills = new Dictionary<int, List<SkillData>>();

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
                Belong      = Convert.ToInt32(data[2]),
                LevelMax    = Convert.ToInt32(data[3]),
                Values      = data[4].Split('|').Select(int.Parse).ToArray(),
                Glass       = data[5].Split('|').Select(int.Parse).ToArray(),
                Order       = Convert.ToInt32(data[6]),
                Description = data[7]
            };

            m_SkillDic[skill.ID]  = skill;

            if (!m_PlayerSkills.ContainsKey(skill.Belong))
            {
                m_PlayerSkills[skill.Belong] = new List<SkillData>();
            }

            m_PlayerSkills[skill.Belong].Add(skill);
        }
    }

    public SkillData GetSkillData(int id)
    {
        return m_SkillDic.TryGetValue(id, out var data) ? data : null;
    }

    public List<SkillData> GetPlayerSkills(int player_id)
    {
        List<SkillData> skills = new List<SkillData>();
        if (m_PlayerSkills.TryGetValue(player_id, out skills)) {
            return skills;
        }

        return skills;
    }

    //重置技能
    public void ResetSkill(int order)
    {
        SkillSlotMsg slot = GameFacade.Instance.DataCenter.User.GetSkillSlotMsg(order);

        if (slot.ID == -1) return;

        SkillData skillData = GameFacade.Instance.DataCenter.League.GetSkillData(slot.ID);

        int cost_total = 0;

        for (int i = 1; i <= slot.Level; i++)
        {
            cost_total += Skill.GetCost(skillData, i);
        }

        //重置技能
        GameFacade.Instance.DataCenter.User.ResetSkill(skillData.Order);

        //返还资源
        GameFacade.Instance.DataCenter.User.UpdateGlass(cost_total);

        EventManager.SendEvent(new GameEvent(EVENT.UI_SKILLUPGRADE));
        EventManager.SendEvent(new GameEvent(EVENT.ONUPDATEGLASS));
    }
}
