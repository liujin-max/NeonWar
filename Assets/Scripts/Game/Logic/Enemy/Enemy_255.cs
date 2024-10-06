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

        ImmuneDisplaceFlag = true;
    }

    public override Buff AddBuff(Unit caster, int buff_id, int value, float time = 0, int count = 1)
    {
        return null;
    }
}
