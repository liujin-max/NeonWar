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

        EventManager.AddHandler(EVENT.ONKILLENEMY,  OnKillEnemy);
    }

    public override void Dispose()
    {
        base.Dispose();

        EventManager.DelHandler(EVENT.ONKILLENEMY,  OnKillEnemy);
    }

    private void OnKillEnemy(GameEvent @event)
    {
        Enemy e = (Enemy) @event.GetParam(0);
        if (e != this) return;

        //死亡时发射子弹
        float base_angle = ToolUtility.VectorToAngle(Field.Instance.Player.transform.localPosition - transform.localPosition);
        Field.Instance.CreateBullet(this).Shoot(base_angle);
        Field.Instance.CreateBullet(this).Shoot(base_angle - 120);
        Field.Instance.CreateBullet(this).Shoot(base_angle + 120);
    }
}
