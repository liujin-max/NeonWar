using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//沙虫尾巴
//无法获得Buff
//周期性召唤小沙虫
public class Enemy_256 : Enemy
{
    private CDTimer m_Timer = new CDTimer(4f);

    public override void Init(MonsterJSON monster_data)
    {
        base.Init(monster_data);

        ImmuneDisplaceFlag  = true;
        ImmuneControlFlag   = true;
    }

    public override bool CustomUpdate(float deltaTime)
    {
        if (!base.CustomUpdate(deltaTime)) return false;
        
        m_Timer.Update(deltaTime);
        if (m_Timer.IsFinished() == true) {
            m_Timer.Reset();

            //召唤
            for (int i = 0; i < 3; i++)
            {
                Field.Instance.Spawn.Summon(new MonsterJSON()
                {
                    ID  = 257,
                    HP  = Mathf.CeilToInt(ATT.HPMAX * 0.1f)
                }, transform.localPosition);
            }
        }

        return true;
    }
}
