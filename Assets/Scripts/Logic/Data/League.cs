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
    public int ATK;
    public int ASP;
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
                ATK         = Convert.ToInt32(data[6]),
                ASP         = Convert.ToInt32(data[7]),
                Description = data[8]
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
        SkillData data;
        if (m_SkillDic.TryGetValue(id, out data)) {
            return data;
        }
        return null;
    }
}
