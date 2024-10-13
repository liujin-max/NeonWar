using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_403 : Enemy
{
    private int m_BuffValue = 3;

    private CDTimer m_Timer = new CDTimer(4f);

    public override void Init(MonsterJSON monster_data)
    {
        base.Init(monster_data);

        this.AddBuff(this, (int)BUFF.SHIELD, m_BuffValue);
    }

    public override bool CustomUpdate(float deltaTime)
    {
        if (!base.CustomUpdate(deltaTime)) return false;

        m_Timer.Update(deltaTime);
        if (m_Timer.IsFinished() == true) {
            m_Timer.Reset();

            if (this.GetBuff((int)BUFF.SHIELD) == null)
                this.AddBuff(this, (int)BUFF.SHIELD, m_BuffValue);
        }

        return true;
    }
}
