using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//鹿首精：可闪避攻击并发射子弹，周期性治疗自己
public class Enemy_109 : Enemy
{
    //治疗频率
    private CDTimer m_Timer = new CDTimer(5f);

    public override void Init(MonsterJSON monster_data)
    {
        base.Init(monster_data);

        Event_Dodge.OnEvent += OnDodge;
    }

    public override void Dispose()
    {
        base.Dispose();

        Event_Dodge.OnEvent -= OnDodge;
    }

    public override bool CustomUpdate(float deltaTime)
    {
        if (!base.CustomUpdate(deltaTime)) return false;
        
        m_Timer.Update(deltaTime);
        if (m_Timer.IsFinished() == true) {
            m_Timer.Reset();

            GameFacade.Instance.EffectManager.Load(EFFECT.HEAL, Vector3.zero, gameObject);
            this.UpdateHP(Mathf.CeilToInt(ATT.HPMAX * 0.05f)); 
        }

        return true;
    }

    #region 监听事件
    private void OnDodge(Event_Dodge e)
    {
        var unit = e.Unit;

        if (unit != this) return;

        Roar();

        //闪避时发射子弹
        float rand = RandomUtility.Random(0, 360);
        for (int i = 0; i < 6; i++)
        {
            float angle = rand + i * 60f;

            var bullet = Field.Instance.CreateBullet(this);
            bullet.Shoot(angle);
        }
    }
    #endregion
}
