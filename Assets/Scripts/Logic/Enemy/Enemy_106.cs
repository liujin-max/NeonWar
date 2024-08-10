using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy_106 : Enemy
{
    public override void DoAttack()
    {
        int random = RandomUtility.Random(0, 360);
        
        CreateBullet().Shoot(random);
        CreateBullet().Shoot(random + 120);
        CreateBullet().Shoot(random - 120);
    }
}
