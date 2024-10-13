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

        Event_Crash.OnEvent += OnCrash;
    }

    public override void Dispose()
    {
        base.Dispose();

        Event_Crash.OnEvent -= OnCrash;
    }

    private void OnCrash(Event_Crash e)
    {
        Enemy enemy = e.Caster;

        if (enemy != this) return;

        Player player = e.Target;
        player.AddBuff(this, (int)BUFF.CHAOS, 1, 5f);
    }
}
