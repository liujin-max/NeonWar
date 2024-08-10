using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_107 : Enemy
{
    private CDTimer m_Timer = new CDTimer(3f);

    public override bool CustomUpdate(float deltaTime)
    {
        if (!base.CustomUpdate(deltaTime)) return false;
        
        m_Timer.Update(deltaTime);
        if (m_Timer.IsFinished() == true) {
            m_Timer.Reset(8f);

            Field.Instance.Player.AddBuff((int)_C.BUFF.CHAOS, 1, 3f);
        }

        return true;
    }
}
