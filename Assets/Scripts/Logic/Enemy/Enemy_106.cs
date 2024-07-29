using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_106 : Enemy
{
    public override void Init(MonsterJSON monster_data)
    {
        base.Init(monster_data);

        this.AddBuff((int)_C.BUFF.SHIELD, 2);
    }

}
