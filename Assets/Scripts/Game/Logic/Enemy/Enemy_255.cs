using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//沙虫躯干
//无法获得Buff
//死亡时转化成小沙虫
public class Enemy_255 : Enemy
{
    public Enemy Head;
    public override void Init(MonsterJSON monster_data)
    {
        base.Init(monster_data);

        ImmuneDisplaceFlag  = true;
        ImmuneControlFlag   = true;
    }

    public override void Dead(Hit hit = null)
    {
        base.Dead(hit);

        for (int i = 0; i < 5; i++)
        {
            Field.Instance.Spawn.Summon(new MonsterJSON(){ID = 257, HP = 300}, transform.localPosition, e => {
                e.transform.localScale = new Vector3(0.6f, 0.6f, 1);
                e.ShowHPText(false);
            });
        }
    }
}
