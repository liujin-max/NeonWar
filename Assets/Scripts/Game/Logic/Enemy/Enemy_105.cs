using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//树精：在场地边缘生成藤曼，可减速路过的敌人
public class Enemy_105 : Enemy
{
    private CDTimer m_Timer = new CDTimer(2f);

    public override bool CustomUpdate(float deltaTime)
    {
        if (!base.CustomUpdate(deltaTime)) return false;
        
        m_Timer.Update(deltaTime);
        if (m_Timer.IsFinished() == true) {
            m_Timer.Reset(6f);

            //投掷种子
            Vector2 point = ToolUtility.FindPointOnCircle(Vector2.zero, 4.5f, RandomUtility.Random(0, 360));

            this.CreateProjectile(PROJECTILE.SEED, CONST.TRACE.PARABOLA, point, 0.8f, ()=>{
                Field.Instance.PushArea(this, AREA.THORNS, point, 8f);
            });
        }

        return true;
    }
}
