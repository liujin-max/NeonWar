using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//小沙虫
public class Enemy_257 : Enemy
{
    public override void Init(MonsterJSON monster_data)
    {
        base.Init(monster_data);

        this.ShowHPText(false);
    }
}
