using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class State_Upgrade<T> : State<Field>
{
    public State_Upgrade(_C.FSMSTATE id) : base(id) {}

    //生成3个技能供选择
    int[] GenerateSkillsPool()
    {
        int skill_count = 3;
        int skill_order = 0;

        int[] skill_ids = new int[skill_count];


        //全技能池子
        //技能ID:技能权重
        Dictionary<int, int> skills_pool = new Dictionary<int, int>();
        foreach (var sk_id in DataCenter.Instance.User.CurrentPlayer.SkillPools) {
            var skill_data = DataCenter.Instance.League.GetSkillData(sk_id);
            skills_pool.Add(sk_id, skill_data.Weight);
        }

        //收集可以升级的技能
        Dictionary<int, int> can_upgrade_skills = new Dictionary<int, int>();
        foreach (var skill in Field.Instance.Player.Skills) {
            if (!skill.IsLevelMax()) {
                can_upgrade_skills.Add(skill.ID, skill.Data.Weight);
                skills_pool.Remove(skill.ID);
            }
        }
        
        //3个技能中有M个技能是从现有的技能中
        //剩下的N个技能从大池子中随机取
        int m = RandomUtility.Random(0, can_upgrade_skills.Count);
        int n = Mathf.Max(0, skill_count - m);

        if (m > 0)
        {
            for (int i = 0; i < m; i++)
            {
                var id  = RandomUtility.PickByWeight(can_upgrade_skills);
                can_upgrade_skills.Remove(id);

                Debug.Log("添加1 ：　"　+ id);
                skill_ids[skill_order] = id;
                skill_order++;
            }
        }

        if (n > 0)
        {
            for (int i = 0; i < n; i++)
            {
                var id  = RandomUtility.PickByWeight(skills_pool);
                skills_pool.Remove(id);

                Debug.Log("添加2 ：　"　+ id);
                skill_ids[skill_order] = id;
                skill_order++;
            }
        }

        return skill_ids;
    }

    public override void Enter(params object[] values)
    { 
        Debug.Log("进入升级阶段");
        
        int[] skill_ids = GenerateSkillsPool();

        GameFacade.Instance.UIManager.LoadWindowAsync("SkillWindow", UIManager.BOARD, (obj)=>{
            var window = obj.GetComponent<SkillWindow>();
            window.Init(skill_ids);
        });
    }

    public override void Update()
    {

    }

    public override void Exit()
    {

    }
}
