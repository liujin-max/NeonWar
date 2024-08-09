using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//树精：高血量且可发射子弹
public class Enemy_105 : Enemy
{
    public override void DoAttack()
    {
        for (int i = 0; i < 3; i++)
        {
            float angle = RandomUtility.Random(0, 360);

            var bullet = CreateBullet();
            bullet.Shoot(angle);
        }
    }
}
