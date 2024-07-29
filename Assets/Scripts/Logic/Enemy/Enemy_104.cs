using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//为周围的同伴恢复生命
public class Enemy_104 : Enemy
{
    private CDTimer m_Timer = new CDTimer(3f);

    public override void Init(MonsterJSON monster_data)
    {
        base.Init(monster_data);

        //设置CD
        m_Timer.Full();
    }

    protected override void Update()
    {
        if (IsDead()) return;
        if (Field.Instance.STATE == _C.GAME_STATE.PAUSE) return;

        m_Timer.Update(Time.deltaTime);
        if (m_Timer.IsFinished() == true) {
            m_Timer.Reset();

            foreach (var e in Field.Instance.Spawn.Enemys)
            {
                if (e.IsDead()) continue;
                if (e == this) continue;


                e.UpdateHP(Mathf.CeilToInt(e.ATT.HPMAX * 0.1f));
                //特效：治疗
                GameFacade.Instance.EffectManager.Load(EFFECT.HEAL, Vector3.zero, e.gameObject);
            }
        }
    }
}
