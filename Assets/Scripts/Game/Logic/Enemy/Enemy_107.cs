using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//红蛙：使玩家陷入混乱
public class Enemy_107 : Enemy
{
    private CDTimer m_Timer = new CDTimer(3f);

    public override bool CustomUpdate(float deltaTime)
    {
        if (!base.CustomUpdate(deltaTime)) return false;
        
        m_Timer.Update(deltaTime);
        if (m_Timer.IsFinished() == true) {
            m_Timer.Reset(8f);

            GameFacade.Instance.EffectManager.Load(EFFECT.WAVE, transform.localPosition, Field.Instance.Land.ELEMENT_ROOT.gameObject);

            if (Vector2.Distance(transform.localPosition, Field.Instance.Player.transform.localPosition) <= 5.5f)
            {
                Field.Instance.Player.AddBuff(this, (int)CONST.BUFF.CHAOS, 1, 3f);
            }
        }

        return true;
    }
}
