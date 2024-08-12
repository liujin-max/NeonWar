using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//狼王：召唤数个小狼
public class Enemy_104 : Enemy
{
    private CDTimer m_Timer = new CDTimer(3f);

    public override void Init(MonsterJSON monster_data)
    {
        base.Init(monster_data);

        //设置CD
        m_Timer.Full();
    }

    public override bool CustomUpdate(float deltaTime)
    {
        if (!base.CustomUpdate(deltaTime)) return false;
        
        m_Timer.Update(deltaTime);
        if (m_Timer.IsFinished() == true) {
            m_Timer.Reset();

            //召唤
            for (int i = 0; i < 2; i++)
            {
                MonsterJSON monsterJSON = new MonsterJSON()
                {
                    ID  = 150,
                    HP  = Mathf.CeilToInt(ATT.HPMAX / 4f)
                };

                Field.Instance.Spawn.Summon(monsterJSON, transform.localPosition);
            }
        }

        return true;
    }
}
