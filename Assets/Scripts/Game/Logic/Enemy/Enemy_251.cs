using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//死亡时发射尖刺
public class Enemy_251 : Enemy
{
    public override void Init(MonsterJSON monster_data)
    {
        base.Init(monster_data);

        Event_KillEnemy.OnEvent += OnKillEnemy;
    }

    public override void Dispose()
    {
        base.Dispose();

        Event_KillEnemy.OnEvent -= OnKillEnemy;
    }

    private void OnKillEnemy(Event_KillEnemy evt)
    {
        Enemy e = evt.Enemy;
        if (e != this) return;

        //死亡时发射子弹
        float base_angle = ToolUtility.VectorToAngle(Field.Instance.Player.transform.localPosition - transform.localPosition);
        Field.Instance.CreateBullet(this).Shoot(base_angle);
        Field.Instance.CreateBullet(this).Shoot(base_angle - 120);
        Field.Instance.CreateBullet(this).Shoot(base_angle + 120);
    }
}
