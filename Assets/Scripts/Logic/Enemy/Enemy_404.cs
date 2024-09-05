using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//怪眼蜘蛛 : 周期性使2个友方产生蛛网护盾
public class Enemy_404 : Enemy
{
    private CDTimer m_Timer = new CDTimer(5f);

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

            List<object> obj_list = new List<object>();

            foreach (var e in Field.Instance.Spawn.Enemys) {
                if (e.IsValid && e.GetBuff((int)_C.BUFF.SHIELD) == null) obj_list.Add(e);
            }

            //缺少特效
            List<object> random_list = RandomUtility.Pick(2, obj_list);
            foreach (Enemy e in random_list) {
                e.AddBuff(this, (int)_C.BUFF.SHIELD, 2);
            }
        }

        return true;
    }
}
