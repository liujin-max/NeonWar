using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//撞击目标时使目标获得混乱
public class Enemy_201 : Enemy
{
    public override void Init(MonsterJSON monster_data)
    {
        base.Init(monster_data);

        EventManager.AddHandler(EVENT.ONCRASH,  OnCrash);
    }

    public override void Dispose()
    {
        base.Dispose();

        EventManager.DelHandler(EVENT.ONCRASH,  OnCrash);
    }

    private void OnCrash(GameEvent @event)
    {
        Enemy enemy = (Enemy) @event.GetParam(0);

        if (enemy != this) return;

        Player player = (Player) @event.GetParam(1);
        player.AddBuff(this, (int)CONST.BUFF.CHAOS, 1, 5f);
    }
}
