using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//沙虫尾巴
//无法获得Buff
//周期性召唤小沙虫
public class Enemy_256 : Enemy
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
