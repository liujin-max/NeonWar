using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//石头人：高血量且可发射子弹
public class Enemy_108 : Enemy
{
    public override void DoAttack()
    {
        int random = RandomUtility.Random(0, 360);
        
        Field.Instance.CreateBullet(this).Shoot(random);
        Field.Instance.CreateBullet(this).Shoot(random + 120);
        Field.Instance.CreateBullet(this).Shoot(random - 120);
    }
}
