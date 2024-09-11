using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class State_Upgrade<T> : State<Field>
{
    private CDTimer m_DelayTimer;

    public State_Upgrade(_C.FSMSTATE id) : base(id) {}

    //生成3个技能供选择
    public static int[] GenerateSkillsPool()
    {
        int skill_max   = 3;
        int skill_order = 0;

        int[] skill_ids = new int[skill_max];


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
            }
            skills_pool.Remove(skill.ID);
            // Debug.Log("移除：" + skill.ID);

            //将互斥技能从技能池中移除
            foreach (var id in skill.Data.Excludes) {
                skills_pool.Remove(id);
                // Debug.Log("移除互斥：" + id);
            }
        }

        //判断前置技能
        //未学习前置技能的技能从技能池中移除
        List<int> keys = new List<int>(skills_pool.Keys);
        foreach (int key in keys) {
            var skill_data = DataCenter.Instance.League.GetSkillData(key);
            if (skill_data.IsMeet() == false) 
            {
                skills_pool.Remove(key);
                // Debug.Log("当前技能的前置未解锁：" + key);
            }
        }
        
        //3个技能中有M个技能是从现有的技能中
        //剩下的N个技能从大池子中随机取
        int m = RandomUtility.Random(0, Mathf.Min(can_upgrade_skills.Count, skill_max) + 1); 
        int n = Mathf.Max(0, skill_max - m);

        if (m > 0)
        {
            for (int i = 0; i < m; i++)
            {
                var id  = RandomUtility.PickByWeight(can_upgrade_skills);
                can_upgrade_skills.Remove(id);

                skill_ids[skill_order] = id;
                skill_order++;
            }
        }

        if (n > 0)
        {
            for (int i = 0; i < n; i++)
            {
                if (skills_pool.Count == 0) break;

                var id  = RandomUtility.PickByWeight(skills_pool);
                skills_pool.Remove(id);

                skill_ids[skill_order] = id;
                skill_order++;
            }
        }

        return skill_ids;
    }

    public override void Enter(params object[] values)
    { 
        m_DelayTimer = new CDTimer(1);
    }

    public override void Update()
    {
        if (m_DelayTimer == null) return;

        m_DelayTimer.Update(Time.deltaTime);
        if (!m_DelayTimer.IsFinished()) return;

        m_DelayTimer = null;

        int[] skill_ids = GenerateSkillsPool();

        // GameFacade.Instance.UIManager.LoadWindowAsync("SkillWindow", UIManager.BOARD, (obj)=>{
        //     var window = obj.GetComponent<SkillWindow>();
        //     window.Init(skill_ids);
        // });
    }

    public override void Exit()
    {

    }
}
