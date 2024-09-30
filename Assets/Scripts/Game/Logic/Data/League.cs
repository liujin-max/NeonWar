using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;




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
                Values      = data[2].Split('|').Where(s => !string.IsNullOrWhiteSpace(s)).Select(int.Parse).ToArray(),
                Costs       = data[3].Split('|').Where(s => !string.IsNullOrWhiteSpace(s)).Select(int.Parse).ToArray(),
                Atlas       = Convert.ToInt32(data[4]),
                Icon        = Convert.ToInt32(data[5]),
                Description = data[6]
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
            cost_total += skillData.GetCost(i);
        }

        //重置技能
        DataCenter.Instance.User.ResetSkill(slot);

        //返还资源
        DataCenter.Instance.User.UpdateGlass(cost_total);

        EventManager.SendEvent(new GameEvent(EVENT.UI_SKILLUPGRADE));
        EventManager.SendEvent(new GameEvent(EVENT.ONUPDATEGLASS));
    }

    //重置养成及所有技能
    public void ResetWeapon()
    {
        var player = DataCenter.Instance.User.CurrentPlayer;

        //重置技能
        foreach (var slot in player.SkillSlots)
        {
            ResetSkill(slot);
        }

        //重置属性
        int atk_cost = 0;
        int asp_cost = 0;
        int wor_cost = 0;

        for (int i = 1; i < player.ATK; i++) atk_cost += NumericalManager.FML_ATKCost(i);
        for (int i = 1; i < player.ASP; i++) asp_cost += NumericalManager.FML_ASPCost(i);
        for (int i = 1; i < player.WORTH; i++) wor_cost += NumericalManager.FML_WORTHCost(i);
        

        player.ATK = 1;
        player.ASP = 1;
        player.WORTH = 1;

        //返还资源
        DataCenter.Instance.User.UpdateGlass(atk_cost + asp_cost + wor_cost);

        EventManager.SendEvent(new GameEvent(EVENT.UI_SKILLUPGRADE));
        EventManager.SendEvent(new GameEvent(EVENT.ONUPDATEGLASS));
    }
}
