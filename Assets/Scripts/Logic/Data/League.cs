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
    public int LevelMax;
    public int[] Values;
    public int[] Glass;
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
                LevelMax    = Convert.ToInt32(data[2]),
                Values      = data[3].Split('|').Select(int.Parse).ToArray(),
                Glass       = data[4].Split('|').Select(int.Parse).ToArray(),
                Description = data[5]
            };

            m_SkillDic[skill.ID]  = skill;
        }
    }

    public SkillData GetSkillData(int id)
    {
        return m_SkillDic.TryGetValue(id, out var data) ? data : null;
    }

    //重置技能
    public void ResetSkill(SkillSlotMsg slot)
    {
        if (slot.ID == -1) return;

        SkillData skillData = DataCenter.Instance.League.GetSkillData(slot.ID);

        int cost_total = 0;

        for (int i = 0; i < slot.Level; i++)
        {
            cost_total += Skill.GetCost(skillData, i);
        }

        //重置技能
        DataCenter.Instance.User.ResetSkill(slot);

        //返还资源
        DataCenter.Instance.User.UpdateGlass(cost_total);

        EventManager.SendEvent(new GameEvent(EVENT.UI_SKILLUPGRADE));
        EventManager.SendEvent(new GameEvent(EVENT.ONUPDATEGLASS));
    }
}
