using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//软泥怪：死亡时分裂成2个小软泥
public class Enemy_401 : Enemy
{
    public override void Dead(Hit hit)
    {
        base.Dead(hit);

        //分裂
        for (int i = 0; i < 2; i++)
        {
            MonsterJSON monsterJSON = new MonsterJSON()
            {
                ID  = 450,
                HP  = Mathf.CeilToInt(ATT.HPMAX / 3f)
            };

            Field.Instance.Spawn.SplitEnemy(monsterJSON, transform.localPosition);
        }
    }
}
