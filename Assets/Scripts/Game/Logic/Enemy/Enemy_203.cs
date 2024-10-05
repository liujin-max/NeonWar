using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//死亡时分裂成3只兔子
public class Enemy_203 : Enemy
{
    public override void Dead(Hit hit = default)
    {
        //白兔
        Field.Instance.Spawn.Summon(new MonsterJSON(){ID = 250, HP = Mathf.CeilToInt(ATT.HPMAX * 0.3f)}, transform.localPosition);
        //棕兔
        Field.Instance.Spawn.Summon(new MonsterJSON(){ID = 251, HP = Mathf.CeilToInt(ATT.HPMAX * 0.5f)}, transform.localPosition);
        //绿兔
        Field.Instance.Spawn.Summon(new MonsterJSON(){ID = 252, HP = Mathf.CeilToInt(ATT.HPMAX * 0.8f)}, transform.localPosition);
    }
}
