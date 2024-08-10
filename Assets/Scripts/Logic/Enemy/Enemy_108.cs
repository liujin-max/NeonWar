using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//石头人：高血量且可发射子弹
public class Enemy_108 : Enemy
{
    public override void DoAttack()
    {
        int random = RandomUtility.Random(0, 360);
        
        CreateBullet().Shoot(random);
        CreateBullet().Shoot(random + 120);
        CreateBullet().Shoot(random - 120);
    }
}
