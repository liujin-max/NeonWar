using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//沙虫躯干
//无法获得Buff
//死亡时转化成小沙虫
public class Enemy_255 : Enemy
{
    public override void Init(MonsterJSON monster_data)
    {
        base.Init(monster_data);

        ImmuneDisplaceFlag  = true;
        ImmuneControlFlag   = true;

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

        Field.Instance.Spawn.Summon(new MonsterJSON(){ID = 257, HP = Mathf.CeilToInt(ATT.HPMAX * 0.1f)}, transform.localPosition);
    }
}
